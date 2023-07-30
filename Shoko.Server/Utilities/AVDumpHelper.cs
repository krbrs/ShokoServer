using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NLog;
using SharpCompress.Common;
using SharpCompress.Readers;
using Shoko.Commons.Utils;
using Shoko.Plugin.Abstractions;
using Shoko.Server.Models;
using Shoko.Server.Repositories;
using Shoko.Server.Utilities;

#nullable enable
namespace Shoko.Server;

public static class AVDumpHelper
{
    #region Private Variables

    private static readonly string WorkingDirectory = Path.Combine(Utils.ApplicationPath, "AVDump");

    private static readonly string ArchivePath = Path.Combine(Utils.ApplicationPath, "avdump.zip");

    private const string AVDumpURL = @"https://cdn.anidb.net/client/avdump3/avdump3_8293_stable.zip";

    private static readonly string AVDumpExecutable = Path.Combine(WorkingDirectory,  Utils.IsRunningOnLinuxOrMac() ? "AVDump3CL.dll" : "AVDump3CL.exe");

    private static ConcurrentDictionary<int, AVDumpSession> ActiveSessions = new();

    private static Regex ProgressRegex = new Regex(@"^\s*(?<currentFiles>\d+)\/(?<totalFiles>\d+)\s+Files\s+\|\s+(?<currentBytes>\d+)\/(?<totalBytes>\d+)\s+\w{1,4}\s+\|", RegexOptions.Compiled);

    private static Regex SummaryRegex = new Regex(@"^\s*Total\s+\[(?<progress>[\s#]+)\]\s+(?<speed1>\d+)\s+(?<speed2>\d+)\s+(?<speed2>\d+)[A-Za-z]{1,3}/s\s*$", RegexOptions.Compiled);

    private static Regex SeperatorRegex = new Regex(@"^\s*\-+\s*$", RegexOptions.Compiled);

    private static Regex InvalidCredentialsRegex = new Regex(@"\s+\(WrongUsernameOrApiKey\)$", RegexOptions.Compiled);

    private static Regex AnidbCreqRegex = new Regex(@"^\s*ACreq\(Done:\s+(?<done>\d+)\s+Failed:\s+(?<failed>\d+)\s+Pending:\s+(?<pending>\d+)\)\s*$", RegexOptions.Compiled);

    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    private static object _prepareLock = new();

    #endregion
    #region Public Variables

    /// <summary>
    /// The currently expected AVDump version to use.
    /// </summary>
    public const string AVDumpVersion = @"8293";

    /// <summary>
    /// Checks if the AVDump component is installed.
    /// </summary>
    /// <value>True if avdump is installed, otherwise false.</value>
    public static bool IsAVDumpInstalled =>
        File.Exists(AVDumpExecutable);

    /// <summary>
    /// Get the version of the installed AVDump componet, provided a compatible
    /// AVDump executable is installed on the system, otherwise returns null.
    /// </summary>
    /// <value>The version number, or null.</value>
    public static string? InstalledAVDumpVersion
    {
        get
        {
            if (!IsAVDumpInstalled)
                return null;

            var result = string.Empty;
            using var subProcess = GetSubProcessForOS("--Version");
            subProcess.Start();
            Task.WaitAll(
                subProcess.StandardOutput.ReadToEndAsync().ContinueWith(task => result = task.Result),
                subProcess.WaitForExitAsync()
            );

            // Assumption is the mother of all f*ck ups, but idc. Assuming the position of the
            // version is faster then actually checking.
            return result
                .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .FirstOrDefault()
                ?.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .LastOrDefault();
        }
    }

    #endregion
    #region Public Methods

    public static IReadOnlyList<AVDumpSession> GetActiveSessions() =>
        ActiveSessions.Values.ToList();

    /// <summary>
    /// Update the installed AVDump component.
    /// </summary>
    public static bool UpdateAVDump() =>
        PrepareAVDump(true);

    /// <summary>
    /// Run AVDump for a file, streaming events from the process, and also
    /// storing some results in the database if successful.
    /// </summary>
    /// <param name="filePath">The absolute path to the file to dump.</param>
    /// <param name="video">The assosiated video for the file.</param>
    /// <param name="commandId">The command id if this operation was ran from
    /// the queue.</param>
    /// <returns>The dump results for v1 compatibility.</returns>
    public static string DumpFile(string filePath, SVR_VideoLocal video, int? commandId = null)
    {
        if (string.IsNullOrEmpty(filePath))
            return "Unable to get file location for VideoLocal with id: " + video.VideoLocalID;

        if (!File.Exists(filePath))
            return "Could not find Video File: " + filePath;

        // The sub-routine takes care of sending the avdump event if it fails,
        // the return message is for v1 compatibility.
        if (!PrepareAVDump())
            return "Failed to install or update the AVDump component";

        var settings = Utils.SettingsProvider.GetSettings();
        if (string.IsNullOrWhiteSpace(settings.AniDb.AVDumpKey))
        {
            var message = "Missing AVDump API Key in the settings.";
            ShokoEventHandler.Instance.OnAVDumpMessage(AVDumpEventType.MissingApiKey);
            logger.Warn(message);
            return message;
        }

        // Ignore invalid command id values.
        if (commandId.HasValue && commandId <= 0)
            commandId = null;

        var videoId = video.VideoLocalID;
        if (ActiveSessions.TryGetValue(video.VideoLocalID, out var session) ||
            !ActiveSessions.TryAdd(videoId, session = new(filePath, video.VideoLocalID, commandId)))
        {
            var message = "Unable to run on the same VideoLocal concurrently.";
            logger.Warn(message);
            return message;
        }

        ShokoEventHandler.Instance.OnAVDumpStart(filePath, videoId, commandId, session.StartedAt);

        try
        {
            // Prepare the sub-process and attach the event handler.
            var stdOutBuilder = new StringBuilder();
            var stdErrBuilder = new StringBuilder();
            DataReceivedEventHandler onStdOutData = (sender, eventArgs) =>
            {
                // Last event (when the stream is closing) will send `null`.
                if (eventArgs.Data == null)
                    return;

                // Ignore empty lines but append them to the output.
                if (string.IsNullOrWhiteSpace(eventArgs.Data))
                {
                    stdOutBuilder.Append("\n");
                    return;
                }

                // Don't display seperators, the summary, or creq updates in the output for now.
                if (SeperatorRegex.IsMatch(eventArgs.Data) || SummaryRegex.IsMatch(eventArgs.Data) || AnidbCreqRegex.IsMatch(eventArgs.Data))
                    return;

                // Calculate progress.
                var result = ProgressRegex.Match(eventArgs.Data);
                if (result.Success)
                {
                    var currentBytes = (double)int.Parse(result.Groups["currentBytes"].Value);
                    var totalBytes = (double)int.Parse(result.Groups["totalBytes"].Value);
                    var currentProgress = currentBytes / totalBytes * 100;
                    if (currentProgress <= session.Progress)
                        return;

                    session.Progress = currentProgress;
                    ShokoEventHandler.Instance.OnAVDumpMessage(videoId, commandId, AVDumpEventType.Progress, null, session.Progress);
                    return;
                }

                // Emit an invalid credentials event if we couldn't authenticate with AniDB.
                if (InvalidCredentialsRegex.IsMatch(eventArgs.Data))
                    ShokoEventHandler.Instance.OnAVDumpMessage(AVDumpEventType.InvalidCredentials);

                // Append everything else to the outputs. We use \r\n for v1 compatibility.
                stdOutBuilder.Append(eventArgs.Data + "\n");
                ShokoEventHandler.Instance.OnAVDumpMessage(videoId, commandId, AVDumpEventType.Message, eventArgs.Data);
            };
            DataReceivedEventHandler onStdErrData = (sender, eventArgs) =>
            {
                // Last event (when the stream is closing) will send `null`.
                // Also ignore any empty lines sent to standard error.
                if (string.IsNullOrWhiteSpace(eventArgs.Data))
                    return;

                // Emit an invalid credentials event if we couldn't authenticate with AniDB.
                //
                // This check is repeated for both std out and std err since they changed it
                // in avdump3 to print on std out, but they _might_ change it back to print
                // on std err for all we know, so now we have a mostly unneeded check.
                if (InvalidCredentialsRegex.IsMatch(eventArgs.Data))
                    ShokoEventHandler.Instance.OnAVDumpMessage(AVDumpEventType.InvalidCredentials);

                stdErrBuilder.Append(eventArgs.Data.Trim() + "\n");
                ShokoEventHandler.Instance.OnAVDumpMessage(videoId, commandId, AVDumpEventType.Error, eventArgs.Data);
            };

            // Prepare the sub-process.
            using var subProcess = GetSubProcessForOS(
                "--HideBuffers=true",
                "--HideFileProgress=true",
                "--Consumers=ED2K",
                $"--Auth={settings.AniDb.Username.Trim()}:{settings.AniDb.AVDumpKey?.Trim()}",
                $"--LPort={settings.AniDb.AVDumpClientPort}",
                "--PrintEd2kLink=true",
                filePath
            );
            subProcess.OutputDataReceived += onStdOutData;
            subProcess.ErrorDataReceived += onStdErrData;

            // Start dumping.
            logger.Info($"Dumping File with AVDump: \"{filePath}\"");
            subProcess.Start();
            subProcess.BeginOutputReadLine();
            subProcess.BeginErrorReadLine();
            subProcess.WaitForExit();

            // Post process the output.
            var stdOut = stdOutBuilder.ToString();
            var stdErr = stdErrBuilder.ToString();
            bool success = string.IsNullOrEmpty(stdErr) && stdOut.Contains("ed2k://");
            var endedAt = DateTime.UtcNow;
            if (success) {
                video.LastAVDumped = endedAt;
                video.LastAVDumpVersion = AVDumpVersion;
                RepoFactory.VideoLocal.Save(video);
            }
            // Print errors to log file if it was unsuccessful.
            else
            {
                logger.Warn($"The dumping of \"{filePath}\" was not successful.\nStandard Output:\n{stdOut}{(stdErr.Length > 0 ? $"\nStandard Error:\n{stdErr}" : "")}");
            }

            // Report the results.
            ShokoEventHandler.Instance.OnAVDumpEnd(filePath, videoId, commandId, session.StartedAt, endedAt, success, stdOut, stdErr);

            // Return the output as a single string for API v1 consumption.
            return stdOut;
        }
        catch (Exception ex)
        {
            ShokoEventHandler.Instance.OnAVDumpGenericException(filePath, videoId, commandId, session.StartedAt, ex);
            var message = $"An error occurred while AVDumping the file \"{filePath}\":\n{ex}";
            logger.Error(message);
            return message;
        }
        finally
        {
            ActiveSessions.TryRemove(videoId, out session);
        }
    }

    #endregion
    #region Private Methods

    /// <summary>
    /// Prepare AVDump for use.
    /// </summary>
    private static bool PrepareAVDump(bool force = false)
    {
        lock (_prepareLock)
        {
            // Automatically update the installed avdump version if we expect
            // a newer version.
            if (!force && InstalledAVDumpVersion != AVDumpVersion)
                force = true;

            if (!force && File.Exists(AVDumpExecutable)) {
                return true;
            }

            ShokoEventHandler.Instance.OnAVDumpMessage(AVDumpEventType.InstallingAVDump);

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
                    ShokoEventHandler.Instance.OnAVDumpInstallException( ex);
                    logger.Error(ex);
                    return false;
                }
            }

            // Extract the archive.
            try
            {
                // First clear out the existing version.
                if (Directory.Exists(WorkingDirectory))
                    Directory.Delete(WorkingDirectory, true);

                // Then add the new version.
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
            }
            catch (Exception ex)
            {
                ShokoEventHandler.Instance.OnAVDumpInstallException(ex);
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

            ShokoEventHandler.Instance.OnAVDumpMessage(AVDumpEventType.InstalledAVDump);
            return true;
        }
    }

    /// <summary>
    /// Get a sub-process to run AVDump for the current OS, with the argument
    /// list appended to the process argument list.
    /// </summary>
    /// <param name="argumentList">Arguments to append to the start info for the
    /// process.</param>
    /// <returns>A new process to run AVDump for the current OS.</returns>
    private static Process GetSubProcessForOS(params string[] argumentList)
    {
        var startInfo = new ProcessStartInfo(AVDumpExecutable)
        {
            UseShellExecute = false,
            WindowStyle = ProcessWindowStyle.Hidden,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        if (Utils.IsRunningOnLinuxOrMac())
        {
            startInfo.FileName = "dotnet";
            startInfo.ArgumentList.Add(AVDumpExecutable);
        }

        foreach (var arg in argumentList)
            startInfo.ArgumentList.Add(arg);

        return new Process { StartInfo = startInfo };
    }

    #endregion
    #region Public Classes

    public class AVDumpSession
    {
        public string Path { get; }

        public int VideoID { get; }

        public int? CommandID { get; }

        public DateTime StartedAt { get; }

        public double Progress { get; set; }

        public AVDumpSession(string filePath, int videoId, int? commandId)
        {
            Path = filePath;
            VideoID = videoId;
            CommandID = commandId;
            StartedAt = DateTime.UtcNow;
            Progress = 0;
        }
    }

    #endregion
}
