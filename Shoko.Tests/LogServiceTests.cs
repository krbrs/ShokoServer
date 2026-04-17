using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Shoko.Abstractions.Plugin;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NLog;
using NLog.Config;
using NLog.Targets;
using Shoko.Server.Services;
using Shoko.Server.Settings;
using Xunit;

#nullable enable
namespace Shoko.Tests;

public class LogServiceTests : IDisposable
{
    private readonly string _tempDirectory = Path.Combine(Path.GetTempPath(), $"shoko-logservice-tests-{Guid.NewGuid():N}");
    private readonly LoggingConfiguration? _previousNlogConfiguration;

    public LogServiceTests()
    {
        Directory.CreateDirectory(_tempDirectory);
        _previousNlogConfiguration = LogManager.Configuration;
        var config = new LoggingConfiguration();
        var fileTarget = new FileTarget("file")
        {
            FileName = Path.Combine(_tempDirectory, "current.jsonl"),
        };
        config.AddRuleForAllLevels(fileTarget);
        LogManager.Configuration = config;
    }

    [Fact]
    public void ReadLogFile_ShouldApplyOffsetAndLimit_ForUncompressedJsonl()
    {
        var path = Path.Combine(_tempDirectory, "sample.jsonl");
        File.WriteAllLines(path,
        [
            MakeLine("a", DateTime.UtcNow.AddMinutes(-3)),
            MakeLine("b", DateTime.UtcNow.AddMinutes(-2)),
            MakeLine("c", DateTime.UtcNow.AddMinutes(-1)),
        ]);

        var service = CreateService();
        var file = GetFileByPath(service, path);
        var result = service.ReadLogFile(file, 1, 1);

        Assert.Single(result.Entries);
        Assert.Equal("b", result.Entries[0].Message);
        Assert.Equal<uint?>(3, result.NextOffset);
    }

    [Fact]
    public void ReadLogFile_ShouldApplyOffsetAndLimit_ForCompressedJsonl()
    {
        var gzipPath = Path.Combine(_tempDirectory, "sample.jsonl.gz");
        using (var stream = File.Open(gzipPath, FileMode.Create))
        using (var gzip = new GZipStream(stream, CompressionLevel.Optimal))
        using (var writer = new StreamWriter(gzip))
        {
            writer.WriteLine(MakeLine("one", DateTime.UtcNow.AddMinutes(-2)));
            writer.WriteLine(MakeLine("two", DateTime.UtcNow.AddMinutes(-1)));
        }

        var service = CreateService();
        var file = GetFileByPath(service, gzipPath);
        var result = service.ReadLogFile(file, 1, 1);

        Assert.Single(result.Entries);
        Assert.Equal("two", result.Entries[0].Message);
        Assert.Equal<uint?>(3, result.NextOffset);
    }

    [Fact]
    public void ReadLogFile_ShouldReturnDescending_ForUncompressedJsonl()
    {
        var t1 = DateTime.UtcNow.AddMinutes(-3);
        var t2 = DateTime.UtcNow.AddMinutes(-2);
        var t3 = DateTime.UtcNow.AddMinutes(-1);
        var path = Path.Combine(_tempDirectory, "descending.jsonl");
        File.WriteAllLines(path,
        [
            MakeLine("a", t1),
            MakeLine("b", t2),
            MakeLine("c", t3),
        ]);

        var service = CreateService();
        var file = GetFileByPath(service, path);
        var result = service.ReadLogFile(file, 0, 0, true);

        Assert.Equal(new[] { "c", "b", "a" }, result.Entries.Select(entry => entry.Message).ToArray());
    }

    [Fact]
    public void ReadLogFile_ShouldReturnDescending_ForCompressedJsonl()
    {
        var gzipPath = Path.Combine(_tempDirectory, "descending-compressed.jsonl.gz");
        using (var stream = File.Open(gzipPath, FileMode.Create))
        using (var gzip = new GZipStream(stream, CompressionLevel.Optimal))
        using (var writer = new StreamWriter(gzip))
        {
            writer.WriteLine(MakeLine("one", DateTime.UtcNow.AddMinutes(-3)));
            writer.WriteLine(MakeLine("two", DateTime.UtcNow.AddMinutes(-2)));
            writer.WriteLine(MakeLine("three", DateTime.UtcNow.AddMinutes(-1)));
        }

        var service = CreateService();
        var file = GetFileByPath(service, gzipPath);
        var result = service.ReadLogFile(file, 0, 0, true);

        Assert.Equal(new[] { "three", "two", "one" }, result.Entries.Select(entry => entry.Message).ToArray());
    }

    [Fact]
    public void ReadLogFile_ShouldHandleMixedNewlines_WhenDescending()
    {
        var t1 = DateTime.UtcNow.AddMinutes(-3);
        var t2 = DateTime.UtcNow.AddMinutes(-2);
        var t3 = DateTime.UtcNow.AddMinutes(-1);
        var path = Path.Combine(_tempDirectory, "mixed-newlines.jsonl");
        var content = $"{MakeLine("first", t1)}\r\n{MakeLine("second", t2)}\n{MakeLine("third", t3)}";
        File.WriteAllText(path, content);

        var service = CreateService();
        var file = GetFileByPath(service, path);
        var result = service.ReadLogFile(file, 0, 0, true);

        Assert.Equal(new[] { "third", "second", "first" }, result.Entries.Select(entry => entry.Message).ToArray());
    }

    [Fact]
    public void ReadRange_ShouldReturnNewestFirst_ByDefault()
    {
        var t1 = DateTime.UtcNow.AddHours(-3);
        var t2 = DateTime.UtcNow.AddHours(-2);
        var t3 = DateTime.UtcNow.AddHours(-1);

        var plainPath = Path.Combine(_tempDirectory, "plain.jsonl");
        File.WriteAllLines(plainPath,
        [
            MakeLine("old", t1),
            MakeLine("middle", t2),
        ]);
        File.SetLastWriteTimeUtc(plainPath, t2);

        var compressedPath = Path.Combine(_tempDirectory, "compressed.jsonl.gz");
        using (var stream = File.Open(compressedPath, FileMode.Create))
        using (var gzip = new GZipStream(stream, CompressionLevel.Optimal))
        using (var writer = new StreamWriter(gzip))
        {
            writer.WriteLine(MakeLine("new", t3));
        }
        File.SetLastWriteTimeUtc(compressedPath, t3);

        var service = CreateService();
        var result = service.ReadRange(t1.AddMinutes(-1), t3.AddMinutes(1), 0, 0);

        Assert.Equal(new[] { "new", "middle", "old" }, result.Entries.Select(entry => entry.Message).ToArray());
    }

    [Fact]
    public void ReadRange_ShouldReturnOldestFirst_WhenDescendingIsFalse()
    {
        var t1 = DateTime.UtcNow.AddHours(-3);
        var t2 = DateTime.UtcNow.AddHours(-2);
        var t3 = DateTime.UtcNow.AddHours(-1);

        var plainPath = Path.Combine(_tempDirectory, "plain-range.jsonl");
        File.WriteAllLines(plainPath,
        [
            MakeLine("old", t1),
            MakeLine("middle", t2),
        ]);
        File.SetLastWriteTimeUtc(plainPath, t2);

        var compressedPath = Path.Combine(_tempDirectory, "compressed-range.jsonl.gz");
        using (var stream = File.Open(compressedPath, FileMode.Create))
        using (var gzip = new GZipStream(stream, CompressionLevel.Optimal))
        using (var writer = new StreamWriter(gzip))
        {
            writer.WriteLine(MakeLine("new", t3));
        }
        File.SetLastWriteTimeUtc(compressedPath, t3);

        var service = CreateService();
        var result = service.ReadRange(t1.AddMinutes(-1), t3.AddMinutes(1), 0, 0, false);

        Assert.Equal(new[] { "old", "middle", "new" }, result.Entries.Select(entry => entry.Message).ToArray());
    }

    public void Dispose()
    {
        LogManager.Configuration = _previousNlogConfiguration;
        if (Directory.Exists(_tempDirectory))
            Directory.Delete(_tempDirectory, true);
    }

    private LogService CreateService()
    {
        var settings = new ServerSettings();
        settings.LogRotator.Enabled = false;
        settings.LogRotator.Zip = false;
        settings.LogRotator.Delete = false;

        var settingsProvider = new Mock<ISettingsProvider>();
        settingsProvider.Setup(provider => provider.GetSettings(It.IsAny<bool>())).Returns(settings);
        var appPaths = new Mock<IApplicationPaths>();
        appPaths.SetupGet(paths => paths.LogsPath).Returns(_tempDirectory);

        return new LogService(NullLogger<LogService>.Instance, appPaths.Object, settingsProvider.Object);
    }

    private static string MakeLine(string message, DateTime timestamp)
        => "{\"timestamp\":\"" + timestamp.ToUniversalTime().ToString("O") +
           "\",\"level\":\"Info\",\"logger\":\"Test\",\"caller\":\"Test::Method\",\"threadId\":\"1\",\"processId\":\"1\",\"message\":\"" +
           message + "\",\"context\":{\"source\":\"test\"}}";

    private static Shoko.Abstractions.Logging.Models.LogFileInfo GetFileByPath(LogService service, string path)
        => Assert.Single(service.GetAllLogFiles(), file => string.Equals(file.FullPath, path, StringComparison.OrdinalIgnoreCase));
}
