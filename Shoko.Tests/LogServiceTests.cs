using System;
using System.IO;
using System.IO.Compression;
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
    public void ReadFile_ShouldApplyOffsetAndLimit_ForUncompressedJsonl()
    {
        var path = Path.Combine(_tempDirectory, "sample.jsonl");
        File.WriteAllLines(path,
        [
            MakeLine("a", DateTime.UtcNow.AddMinutes(-3)),
            MakeLine("b", DateTime.UtcNow.AddMinutes(-2)),
            MakeLine("c", DateTime.UtcNow.AddMinutes(-1)),
        ]);

        var service = CreateService();
        var result = service.ReadFile("sample.jsonl", 1, 1);

        Assert.Single(result.Entries);
        Assert.Equal("b", result.Entries[0].Message);
        Assert.Equal(3, result.NextOffset);
    }

    [Fact]
    public void ReadFile_ShouldApplyOffsetAndLimit_ForCompressedJsonl()
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
        var result = service.ReadFile("sample.jsonl.gz", 1, 1);

        Assert.Single(result.Entries);
        Assert.Equal("two", result.Entries[0].Message);
        Assert.Equal(3, result.NextOffset);
    }

    [Fact]
    public void ReadRange_ShouldFilterByFromAndTo_AcrossMixedJsonlFiles()
    {
        var t1 = DateTime.UtcNow.AddHours(-3);
        var t2 = DateTime.UtcNow.AddHours(-2);
        var t3 = DateTime.UtcNow.AddHours(-1);

        File.WriteAllLines(Path.Combine(_tempDirectory, "plain.jsonl"),
        [
            MakeLine("old", t1),
            MakeLine("middle", t2),
        ]);

        using (var stream = File.Open(Path.Combine(_tempDirectory, "compressed.jsonl.gz"), FileMode.Create))
        using (var gzip = new GZipStream(stream, CompressionLevel.Optimal))
        using (var writer = new StreamWriter(gzip))
        {
            writer.WriteLine(MakeLine("new", t3));
        }

        var service = CreateService();
        var result = service.ReadRange(t2.AddMinutes(-1), t3.AddMinutes(-1), 0, 10);

        Assert.Single(result.Entries);
        Assert.Equal("middle", result.Entries[0].Message);
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
           "\",\"level\":\"Info\",\"logger\":\"Test\",\"caller\":\"Test::Method\",\"threadId\":1,\"message\":\"" +
           message + "\",\"context\":{\"source\":\"test\"}}";
}
