using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Mono.Unix;
using NLog;
using SharpCompress.Common;
using SharpCompress.Readers;
using Shoko.Commons.Utils;
using Shoko.Plugin.Abstractions;
using Shoko.Server.Models;
using Shoko.Server.Repositories;
using Shoko.Server.Utilities;

using ISettingsProvider = Shoko.Server.Settings.ISettingsProvider;

namespace Shoko.Server;

public static class AVDumpHelper
{
    public static readonly string WorkingDirectory = Path.Combine(Utils.ApplicationPath, "AVDump");

    public static readonly string ArchivePath = Path.Combine(Utils.ApplicationPath, "avdump.zip");

    public const string AVDumpURL = @"https://cdn.anidb.net/client/avdump3/avdump3_8293_stable.zip";

    public static readonly string AVDumpExecutable = Path.Combine(WorkingDirectory,  Utils.IsRunningOnLinuxOrMac() ? "AVDump3CL" : "AVDump3CL.exe");

    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    private static object _prepareLock = new();

    /// <summary>
    /// Prepare AVDump for use.
    /// </summary>
    public static bool PrepareAVDump(string filePath, int videoId, int? commandId, DateTime startedAt)
    {
        lock (_prepareLock)
        {
            if (File.Exists(AVDumpExecutable)) {
                return true;
            }

            ShokoEventHandler.Instance.OnAVDumpMessage(filePath, videoId, commandId, startedAt, AVDumpMessageType.InstallingAVDump, "Installing AVDump component to server…", 0);

            // Download the archive if it's not available locally.
            if (!File.Exists(ArchivePath))
            {
                try
                {
                    using (var stream = Misc.DownloadWebBinary(AVDumpURL))
                    {
                        if (stream == null)
                            return false;

                        using (var fileStream = File.Create(ArchivePath))
                            stream.CopyTo(fileStream);
                    }
                }
                catch (Exception ex)
                {
                    ShokoEventHandler.Instance.OnAVDumpGenericException(filePath, videoId, commandId, startedAt, ex);
                    logger.Error(ex);
                    return false;
                }
            }

            try
            {
                // First clear out the existing one.
                if (Directory.Exists(WorkingDirectory))
                    Directory.Delete(WorkingDirectory, true);

                // Then create a new one.
                Directory.CreateDirectory(WorkingDirectory);
                using (Stream stream = File.OpenRead(ArchivePath))
                using (var reader = ReaderFactory.Open(stream))
                {
                    while (reader.MoveToNextEntry())
                    {
                        if (!reader.Entry.IsDirectory)
                        {
                            reader.WriteEntryToDirectory(WorkingDirectory, new ExtractionOptions
                            {
                                // This may have serious problems in the future, but for now, AVDump is flat
                                ExtractFullPath = false,
                                Overwrite = true,
                            });
                        }
                    }
                }

                // Mark the executable as runnable on linux.
                if (Utils.IsRunningOnLinuxOrMac())
                {
                    var fileInfo = new UnixFileInfo(AVDumpExecutable);
                    fileInfo.FileAccessPermissions |= FileAccessPermissions.UserExecute;
                    fileInfo.Refresh();
                }
            }
            catch (Exception ex)
            {
                ShokoEventHandler.Instance.OnAVDumpGenericException(filePath, videoId, commandId, startedAt, ex);
                logger.Error(ex);
                return false;
            }

            try
            {
                File.Delete(ArchivePath);
            }
            catch
            {
                // eh we tried
            }

            ShokoEventHandler.Instance.OnAVDumpMessage(filePath, videoId, commandId, startedAt, AVDumpMessageType.InstalledAVDump, "Installing AVDump component to server…", 0);
            return true;
        }
    }

    public static ConcurrentDictionary<int, (string filePath, int videoId, int? commandId, DateTime startedAt, double progress)> ActiveSessions = new();

    public static IReadOnlyList<(string filePath, int videoId, int? commandId, DateTime startedAt, double progress)> GetActiveSessions() =>
        ActiveSessions.Values.ToList();

    private static Regex ETA = new Regex(@"^\s*(?<currentFiles>\d+)\/(?<totalFiles>\d+)\s+Files\s+\|\s+(?<currentBytes>\d+)\/(?<totalBytes>\d+)\s+\w{1,4}\s+\|", RegexOptions.Compiled);

    // TODO: This regex is wrong. Fix it.
    private static Regex Total = new Regex(@"^\s*Total\s+\[[\s#]+\] (\d) (\d) (\d)[A-Za-z]{1,3}/s\s*$", RegexOptions.Compiled);

    private static Regex Dashes = new Regex(@"^\s*\-+\s*$", RegexOptions.Compiled);

    public static string DumpFile(string filePath, SVR_VideoLocal video, int? commandId = null)
    {
        if (string.IsNullOrEmpty(filePath))
            return "Unable to get file location for VideoLocal with id: " + video.VideoLocalID;

        if (!File.Exists(filePath))
            return "Could not find Video File: " + filePath;

        // Ignore invalid command id values.
        if (commandId.HasValue && commandId <= 0)
            commandId = null;

        var videoId = video.VideoLocalID;
        var startedAt = DateTime.UtcNow;
        if (ActiveSessions.TryGetValue(video.VideoLocalID, out var tuple) ||
            !ActiveSessions.TryAdd(videoId, tuple = (filePath, video.VideoLocalID, commandId, startedAt, 0D)))
            return "Unable to run on the same VideoLocal concurrently.";
        ShokoEventHandler.Instance.OnAVDumpStart(filePath, videoId, commandId, startedAt);

        var settings = Utils.ServiceContainer.GetRequiredService<ISettingsProvider>().GetSettings();
        if (string.IsNullOrWhiteSpace(settings.AniDb.AVDumpKey))
        {
            var message = "Missing AVDump API Key";
            ShokoEventHandler.Instance.OnAVDumpMessage(filePath, videoId, commandId, startedAt, AVDumpMessageType.MissingApiKey, message);
            return message;
        }

        // The sub-routine takes care of sending the final events if it fails.
        if (!PrepareAVDump(filePath, videoId, commandId, startedAt))
            return "Could not find or download AvDump CLI";

        try
        {
            var lastTotalLine = string.Empty;
            // Prepare the sub-process and attach the event handler.
            var stdOut = string.Empty;
            var stdErr = string.Empty;
            var startInfo = GetExecutableAndArgListForOS(filePath);
            using var subProcess = new Process { StartInfo = startInfo };
            DataReceivedEventHandler onStdOutData = (sender, eventArgs) =>
            {
                // Last event (when the stream is closing) will send `null`.
                if (eventArgs.Data == null)
                    return;

                if (string.IsNullOrEmpty(eventArgs.Data))
                {
                    stdOut += "\n";
                    return;
                }

                // Don't display the seperators in the output.
                if (Dashes.IsMatch(eventArgs.Data))
                    return;

                // Reduce the number of summary lines in the logs.
                if (Total.IsMatch(eventArgs.Data))
                {
                    if (string.Equals(lastTotalLine, eventArgs.Data))
                        return;

                    lastTotalLine = eventArgs.Data;
                }

                stdOut += eventArgs.Data.Trim() + "\n";
                var result = ETA.Match(eventArgs.Data);
                if (!result.Success)
                {
                    ShokoEventHandler.Instance.OnAVDumpMessage(filePath, videoId, commandId, DateTime.UtcNow, AVDumpMessageType.Message, eventArgs.Data, tuple.progress);
                    return;
                }

                var currentBytes = (double)int.Parse(result.Groups["currentBytes"].Value);
                var totalBytes = (double)int.Parse(result.Groups["totalBytes"].Value);
                var currentProgress = currentBytes / totalBytes * 100;
                if (currentProgress <= tuple.progress)
                    return;

                tuple.progress = currentProgress;
                ShokoEventHandler.Instance.OnAVDumpMessage(filePath, videoId, commandId, DateTime.UtcNow, AVDumpMessageType.Progress, eventArgs.Data, tuple.progress);
            };
            DataReceivedEventHandler onStdErrData = (sender, eventArgs) =>
            {
                // Last event (when the stream is closing) will send `null`.
                if (eventArgs.Data == null)
                    return;

                // stdErr += eventArgs.Data.Trim() + "\n";
                if (!string.IsNullOrEmpty(eventArgs.Data))
                    ShokoEventHandler.Instance.OnAVDumpMessage(filePath, videoId, commandId, DateTime.UtcNow, AVDumpMessageType.Error, eventArgs.Data, tuple.progress);
            };

            // Logs only the last argument to AVDump, the filename
            logger.Info($"Dumping File with AVDump: \"{startInfo.ArgumentList[^1]}\"");

            // Run the process.
            subProcess.OutputDataReceived += onStdOutData;
            subProcess.ErrorDataReceived += onStdErrData;
            subProcess.Start();

            subProcess.BeginOutputReadLine();
            subProcess.BeginErrorReadLine();
            subProcess.WaitForExit();

            subProcess.OutputDataReceived -= onStdOutData;
            subProcess.ErrorDataReceived -= onStdErrData;
            subProcess.CancelOutputRead();
            subProcess.CancelErrorRead();

            // Post process the output.
            // TODO: Edit the conditions for what is considered a "success".
            bool success = string.IsNullOrEmpty(stdErr) && stdOut.Contains("SUCCESS");
            var endedAt = DateTime.UtcNow;
            if (success) {
                // TODO: Do we want to store the last stdout/stderr in the database?
                video.LastAVDumped = endedAt;
                RepoFactory.VideoLocal.Save(video);
            }

            // Report the results.
            ShokoEventHandler.Instance.OnAVDumpEnd(filePath, videoId, commandId, startedAt, endedAt, success, stdOut);
            ActiveSessions.TryRemove(videoId, out tuple);

            // Return the output as a single string for API v1 consumption.
            return stdOut;
        }
        catch (Exception ex)
        {
            ShokoEventHandler.Instance.OnAVDumpGenericException(filePath, videoId, commandId, startedAt, ex);
            ActiveSessions.TryRemove(videoId, out tuple);

            var message = $"An error occurred while AVDumping the file \"{filePath}\":\n{ex}";
            logger.Error(message);
            return message;
        }
    }

    private static ProcessStartInfo GetExecutableAndArgListForOS(string file)
    {
        var startInfo = new ProcessStartInfo(AVDumpExecutable)
        {
            UseShellExecute = false,
            WindowStyle = ProcessWindowStyle.Hidden,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        var settings = Utils.SettingsProvider.GetSettings();

        startInfo.ArgumentList.Add("--HideBuffers=true");
        startInfo.ArgumentList.Add("--HideFileProgress=true");
        startInfo.ArgumentList.Add("--Consumers=ED2K");
        startInfo.ArgumentList.Add($"--Auth={settings.AniDb.Username.Trim()}:{settings.AniDb.AVDumpKey?.Trim()}");
        startInfo.ArgumentList.Add($"--LPort={settings.AniDb.AVDumpClientPort}");
        startInfo.ArgumentList.Add(file);

        return startInfo;
    }
}
