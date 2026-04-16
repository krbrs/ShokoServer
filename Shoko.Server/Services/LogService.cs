using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using System.Timers;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Config;
using NLog.Filters;
using NLog.Targets;
using NLog.Targets.Wrappers;
using Quartz.Logging;
using Shoko.Abstractions.Logging.Models;
using Shoko.Abstractions.Logging.Services;
using Shoko.Abstractions.Plugin;
using Shoko.Server.API.SignalR.NLog;
using Shoko.Server.Settings;

#nullable enable
namespace Shoko.Server.Services;

public class LogService(ILogger<LogService> logger, IApplicationPaths applicationPaths, ISettingsProvider settingsProvider) : ILogService
{
    private readonly Timer _timer = new(86400000) { AutoReset = true };

    public string GetCurrentLogFilePath()
    {
        return LogManager.Configuration?.FindTargetByName("file") is not FileTarget fileTarget
            ? string.Empty
            : Path.GetFullPath(fileTarget.FileName.Render(new LogEventInfo { Level = NLog.LogLevel.Info }));
    }

    public void StartMaintenance()
    {
        _timer.Elapsed -= HandleTimerElapsed;
        _timer.Elapsed += HandleTimerElapsed;
        RunRotationMaintenance();
        _timer.Start();
    }

    public void RunRotationMaintenance()
    {
        DeleteLogs();
        CompressLogs();
    }

    public IEnumerable<LogFileInfo> ListFiles()
    {
        var directory = EnsureLogDirectory();
        return directory.GetFiles()
            .OrderByDescending(file => file.LastWriteTimeUtc)
            .Select(ToLogFileInfo)
            .ToList();
    }

    public LogReadResult ReadCurrent(int offset = 0, int limit = 100)
    {
        var currentPath = GetCurrentLogFilePath();
        if (string.IsNullOrWhiteSpace(currentPath) || !File.Exists(currentPath))
            return new() { NextOffset = offset, Entries = [] };
        var info = ToLogFileInfo(new FileInfo(currentPath));
        return ReadLogFile(info, offset, limit, null, null);
    }

    public LogReadResult ReadFile(string fileId, int offset = 0, int limit = 100)
    {
        var file = ResolveFile(fileId);
        return ReadLogFile(file, offset, limit, null, null);
    }

    public LogReadResult ReadRange(DateTime? from = null, DateTime? to = null, int offset = 0, int limit = 100)
    {
        if (limit <= 0)
            return new() { NextOffset = offset, Entries = [] };

        var files = ListFiles()
            .Where(file => file.Format is LogFileFormat.JsonL)
            .OrderBy(file => file.LastModifiedAt)
            .ToList();

        var skipped = 0;
        var nextOffset = offset;
        var entries = new List<LogEntry>();
        foreach (var file in files)
        {
            foreach (var entry in ReadEntries(file, from, to))
            {
                if (skipped < offset)
                {
                    skipped++;
                    nextOffset++;
                    continue;
                }
                if (entries.Count >= limit)
                    return new() { NextOffset = nextOffset, Entries = entries };
                entries.Add(entry);
                nextOffset++;
            }
        }
        return new() { NextOffset = nextOffset, Entries = entries };
    }

    public LogDownloadResult OpenDownload(string fileId, bool decompress = false)
    {
        var file = ResolveFile(fileId);
        if (decompress && file.IsCompressed)
        {
            if (file.Format is LogFileFormat.JsonL)
            {
                var stream = OpenGZipStream(file.FullPath);
                return new()
                {
                    ContentType = "application/x-ndjson",
                    FileName = Path.GetFileNameWithoutExtension(file.FileName),
                    Stream = stream,
                };
            }
            else
            {
                var stream = OpenZipEntryStream(file.FullPath, out var entryName);
                return new()
                {
                    ContentType = "text/plain",
                    FileName = entryName,
                    Stream = stream,
                };
            }
        }

        var fileStream = File.Open(file.FullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        return new()
        {
            ContentType = (file.Format, file.IsCompressed) switch
            {
                (LogFileFormat.JsonL, true) => "application/gzip",
                (LogFileFormat.JsonL, false) => "application/x-ndjson",
                (LogFileFormat.Legacy, true) => "application/zip",
                (LogFileFormat.Legacy, false) => "text/plain",
                _ => "application/octet-stream",
            },
            FileName = file.FileName,
            Stream = fileStream,
        };
    }

    private LogReadResult ReadLogFile(LogFileInfo file, int offset, int limit, DateTime? from, DateTime? to)
    {
        if (file.Format is not LogFileFormat.JsonL)
            throw new InvalidOperationException("Only JSONL logs support reading.");
        if (limit <= 0)
            return new() { NextOffset = offset, Entries = [] };

        var skipped = 0;
        var nextOffset = offset;
        var entries = new List<LogEntry>();
        foreach (var entry in ReadEntries(file, from, to))
        {
            if (skipped < offset)
            {
                skipped++;
                nextOffset++;
                continue;
            }
            if (entries.Count >= limit)
                break;
            entries.Add(entry);
            nextOffset++;
        }
        return new() { NextOffset = nextOffset, Entries = entries };
    }

    private IEnumerable<LogEntry> ReadEntries(LogFileInfo file, DateTime? from, DateTime? to)
    {
        if (file.IsCompressed && file.FileName.EndsWith(".jsonl.gz", StringComparison.OrdinalIgnoreCase))
        {
            using var stream = OpenGZipStream(file.FullPath);
            using var reader = new StreamReader(stream);
            foreach (var entry in ParseLogEntries(reader, from, to))
                yield return entry;
            yield break;
        }

        using var fileStream = File.Open(file.FullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var fileReader = new StreamReader(fileStream);
        foreach (var entry in ParseLogEntries(fileReader, from, to))
            yield return entry;
    }

    private IEnumerable<LogEntry> ParseLogEntries(StreamReader reader, DateTime? from, DateTime? to)
    {
        while (reader.ReadLine() is { } line)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;
            if (!TryParseLogEntry(line, out var entry))
                continue;
            if (from.HasValue && entry.Timestamp < from.Value)
                continue;
            if (to.HasValue && entry.Timestamp > to.Value)
                continue;
            yield return entry;
        }
    }

    private bool TryParseLogEntry(string line, out LogEntry entry)
    {
        entry = null!;
        try
        {
            using var document = JsonDocument.Parse(line);
            var root = document.RootElement;

            var timestamp = root.TryGetProperty("timestamp", out var timestampElement) &&
                DateTime.TryParse(timestampElement.GetString(), out var ts)
                ? ts.ToUniversalTime()
                : DateTime.UtcNow;

            var threadId = root.TryGetProperty("threadId", out var threadIdElement) &&
                threadIdElement.GetString() is { } threadIdString && int.TryParse(threadIdString, out var parsedThreadId)
                ? parsedThreadId
                : 0;

            var processId = root.TryGetProperty("processId", out var processIdElement) &&
                processIdElement.GetString() is { } processIdString && int.TryParse(processIdString, out var parsedProcessId)
                ? parsedProcessId
                : 0;

            entry = new()
            {
                Timestamp = timestamp,
                Level = root.TryGetProperty("level", out var levelElement) ? levelElement.GetString() ?? string.Empty : string.Empty,
                Logger = root.TryGetProperty("logger", out var loggerElement) ? loggerElement.GetString() ?? string.Empty : string.Empty,
                Caller = root.TryGetProperty("caller", out var callerElement) ? callerElement.GetString() ?? string.Empty : string.Empty,
                ThreadId = threadId,
                ProcessId = processId,
                Message = root.TryGetProperty("message", out var messageElement) ? messageElement.GetString() ?? string.Empty : string.Empty,
                Exception = root.TryGetProperty("exception", out var exceptionElement) ? exceptionElement.GetString() : null,
            };
            return true;
        }
        catch (Exception ex)
        {
            logger.LogDebug(ex, "Skipped malformed log line.");
            return false;
        }
    }

    private LogFileInfo ResolveFile(string fileId)
        => ListFiles().FirstOrDefault(f => string.Equals(f.Id, fileId, StringComparison.OrdinalIgnoreCase)) ??
            throw new FileNotFoundException($"Unable to locate log file '{fileId}'.");

    private static Stream OpenGZipStream(string fullPath)
        => new GZipStream(
            File.Open(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite),
            CompressionMode.Decompress
        );

    private static Stream OpenZipEntryStream(string fullPath, out string entryName)
    {
        var zipStream = File.Open(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        var archive = new ZipArchive(zipStream, ZipArchiveMode.Read);
        var entry = archive.Entries
            .OrderByDescending(e => e.LastWriteTime)
            .FirstOrDefault(e => !string.IsNullOrEmpty(e.Name));
        if (entry is null)
        {
            archive.Dispose();
            zipStream.Dispose();
            throw new InvalidOperationException("The archive does not contain a readable entry.");
        }
        entryName = entry.Name;
        return new WrappedZipStream(archive, entry.Open(), zipStream);
    }

    private void HandleTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        RunRotationMaintenance();
    }

    private DirectoryInfo EnsureLogDirectory()
    {
        var currentLog = GetCurrentLogFilePath();
        var directory = string.IsNullOrWhiteSpace(currentLog)
            ? applicationPaths.LogsPath
            : Path.GetDirectoryName(currentLog) ?? applicationPaths.LogsPath;
        var info = new DirectoryInfo(directory);
        if (!info.Exists)
            info.Create();
        return info;
    }

    private void CompressLogs()
    {
        var settings = settingsProvider.GetSettings().LogRotator;
        if (!settings.Zip)
            return;
        var currentLog = GetCurrentLogFilePath();
        foreach (var file in EnsureLogDirectory().GetFiles("*.jsonl")
                     .Where(file => !string.Equals(file.FullName, currentLog, StringComparison.OrdinalIgnoreCase)))
        {
            var destination = file.FullName + ".gz";
            using var source = File.Open(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var destinationStream = File.Open(destination, FileMode.Create, FileAccess.Write, FileShare.Read);
            using var gzip = new GZipStream(destinationStream, CompressionLevel.Optimal);
            source.CopyTo(gzip);
            file.Delete();
        }
    }

    private void DeleteLogs()
    {
        var settings = settingsProvider.GetSettings().LogRotator;
        if (!settings.Delete || string.IsNullOrEmpty(settings.Delete_Days) || !int.TryParse(settings.Delete_Days, out var days))
            return;
        var threshold = DateTime.UtcNow.AddDays(-days);
        foreach (var file in EnsureLogDirectory().GetFiles()
                     .Where(file => file.LastWriteTimeUtc < threshold)
                     .Where(file => file.Name.EndsWith(".jsonl.gz", StringComparison.OrdinalIgnoreCase) || file.Extension.Equals(".zip", StringComparison.OrdinalIgnoreCase)))
        {
            file.Delete();
        }
    }

    private static LogFileInfo ToLogFileInfo(FileInfo file)
    {
        var format = DetermineFormat(file);
        return new()
        {
            Id = file.Name,
            FileName = file.Name,
            FullPath = file.FullName,
            Size = file.Length,
            LastModifiedAt = file.LastWriteTimeUtc,
            IsCompressed = file.Name.EndsWith(".jsonl.gz", StringComparison.OrdinalIgnoreCase) || file.Extension.Equals(".zip", StringComparison.OrdinalIgnoreCase),
            Format = format,
        };
    }

    private static LogFileFormat DetermineFormat(FileInfo file)
    {
        if (file.Extension.Equals(".jsonl", StringComparison.OrdinalIgnoreCase))
            return LogFileFormat.JsonL;
        if (file.Name.EndsWith(".jsonl.gz", StringComparison.OrdinalIgnoreCase))
            return LogFileFormat.JsonL;
        return LogFileFormat.Legacy;
    }

    private sealed class WrappedZipStream(ZipArchive archive, Stream inner, Stream fileStream) : Stream
    {
        public override bool CanRead => inner.CanRead;
        public override bool CanSeek => inner.CanSeek;
        public override bool CanWrite => inner.CanWrite;
        public override long Length => inner.Length;
        public override long Position { get => inner.Position; set => inner.Position = value; }
        public override void Flush() => inner.Flush();
        public override int Read(byte[] buffer, int offset, int count) => inner.Read(buffer, offset, count);
        public override long Seek(long offset, SeekOrigin origin) => inner.Seek(offset, origin);
        public override void SetLength(long value) => inner.SetLength(value);
        public override void Write(byte[] buffer, int offset, int count) => inner.Write(buffer, offset, count);

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                inner.Dispose();
                archive.Dispose();
                fileStream.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    #region Static Helper

    public static void InitLogger(IApplicationPaths applicationPaths)
    {
        var config = LogManager.Configuration;
        if (config is null)
            return;

        var target = config.FindTargetByName<FileTarget>("file");
        if (target != null)
        {
            target.FileName = Path.Join(applicationPaths.LogsPath, "${shortdate}.jsonl")!;
        }

#if LOGWEB
            // Disable blackhole http info logs
            config.LoggingRules.FirstOrDefault(r => r.LoggerNamePattern.StartsWith("Microsoft.AspNetCore"))?.DisableLoggingForLevel(LogLevel.Info);
            config.LoggingRules.FirstOrDefault(r => r.LoggerNamePattern.StartsWith("Shoko.Server.API.Authentication"))?.DisableLoggingForLevel(LogLevel.Info);
#endif
#if DEBUG
        // Enable debug logging
        config.LoggingRules.FirstOrDefault(a => target != null && a.Targets.Contains(target))
            ?.EnableLoggingForLevel(NLog.LogLevel.Debug);
#endif

        var signalrTarget =
            new AsyncTargetWrapper(
                new SignalRTarget { Name = "signalr", MaxLogsCount = 5000, Layout = "${message}${onexception:\\: ${exception:format=tostring}}" }, 50,
                AsyncTargetWrapperOverflowAction.Discard);
        config.AddTarget("signalr", signalrTarget);
        config.LoggingRules.Add(new LoggingRule("*", NLog.LogLevel.Trace, signalrTarget));
        var consoleTarget = config.FindTargetByName<ColoredConsoleTarget>("console");
        consoleTarget?.Layout = "${date:format=HH\\:mm\\:ss}| ${logger:shortname=true} --- ${message}${onexception:\\: ${exception:format=tostring}}";

        foreach (var loggingRule in config.LoggingRules)
        {
            var hasFileTarget = target != null && loggingRule.Targets.Contains(target);
            var hasConsoleTarget = consoleTarget != null && loggingRule.Targets.Contains(consoleTarget);
            if (hasFileTarget || hasConsoleTarget || loggingRule.Targets.Contains(signalrTarget))
            {
                loggingRule.FilterDefaultAction = FilterResult.Log;
                loggingRule.Filters.Add(new ConditionBasedFilter()
                {
                    Action = FilterResult.Ignore,
                    Condition = "(contains(message, 'password') or contains(message, 'token') or contains(message, 'key')) and starts-with(message, 'Settings.')"
                });
            }
        }

        LogProvider.SetLogProvider(new NLog.Extensions.Logging.NLogLoggerFactory());

        LogManager.ReconfigExistingLoggers();
    }

    public static void SetTraceLogging(bool enabled)
    {
        var config = LogManager.Configuration;
        if (config == null)
            return;
        var fileRule = config.LoggingRules.FirstOrDefault(a => a.Targets.Any(b => b is FileTarget));
        var signalrRule = config.LoggingRules.FirstOrDefault(a => a.Targets.Any(b => b is SignalRTarget));
        if (enabled)
        {
            fileRule?.EnableLoggingForLevels(NLog.LogLevel.Trace, NLog.LogLevel.Debug);
            signalrRule?.EnableLoggingForLevels(NLog.LogLevel.Trace, NLog.LogLevel.Debug);
        }
        else
        {
            fileRule?.DisableLoggingForLevels(NLog.LogLevel.Trace, NLog.LogLevel.Debug);
            signalrRule?.DisableLoggingForLevels(NLog.LogLevel.Trace, NLog.LogLevel.Debug);
        }

        LogManager.ReconfigExistingLoggers();
    }

    #endregion
}
