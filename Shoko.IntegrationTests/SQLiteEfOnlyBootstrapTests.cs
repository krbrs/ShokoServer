using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shoko.Abstractions.Video.Enums;
using Shoko.Server.Data;
using Shoko.Server.Data.SchemaComparison;
using Shoko.Server.Databases;
using Shoko.Server.Models.Shoko;
using Shoko.Server.Repositories;
using Shoko.Server.Services;
using Shoko.Server.Utilities;
using Xunit;

#nullable enable

namespace Shoko.IntegrationTests;

[Collection("Database")]
public class SQLiteEfOnlyBootstrapTests
{
    [Fact]
    public async Task SQLite_EfOnlyBootstrap_PopulatesInitialData_And_SurvivesRestart()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"shoko-efonly-{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDir);
        var succeeded = false;

        var originalShokoHome = Environment.GetEnvironmentVariable("SHOKO_HOME");
        try
        {
            Environment.SetEnvironmentVariable("SHOKO_HOME", tempDir.Replace('\\', '/'));
            SQLite.UseEfOnlyBootstrapForTests = true;
            SQLite.ResetTestState();
            SQLite.ThrowOnSessionFactoryCreateForTests = true;
            Assert.Equal(0, RepoFactory.EfOnlyPopulateSessionCount);
            Assert.Equal(0, RepoFactory.EfOnlySkippedRepairPassCount);
            Assert.Equal(0, SQLite.SessionFactoryCreateCallCount);

            var firstHost = await StartServiceAsync(waitForStartupComplete: true);
            try
            {
                Assert.True(RepoFactory.EfOnlyPopulateSessionCount > 0);
                Assert.Equal(0, RepoFactory.EfOnlySkippedRepairPassCount);
                Assert.Equal(0, SQLite.SessionFactoryCreateCallCount);
                Assert.NotNull(RepoFactory.JMMUser.GetByUsername("Default"));
                Assert.NotEmpty(RepoFactory.FilterPreset.GetAll());

                using (var scope = Utils.ServiceContainer.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ShokoDbContext>();
                    var activationService = new EfStartupActivationService(context);
                    var activationResult = await activationService.ActivateAsync();

                    Assert.True(activationResult.Success, string.Join(Environment.NewLine, activationResult.Errors));
                    Assert.Empty(activationResult.AppliedMigrations);
                }
            }
            finally
            {
                await firstHost.StopAsync(TimeSpan.FromSeconds(30));
            }

            var secondHost = await StartServiceAsync(waitForStartupComplete: true);
            try
            {
                Assert.True(RepoFactory.EfOnlyPopulateSessionCount > 0);
                Assert.Equal(0, RepoFactory.EfOnlySkippedRepairPassCount);
                Assert.Equal(0, SQLite.SessionFactoryCreateCallCount);
                Assert.NotNull(RepoFactory.JMMUser.GetByUsername("Default"));
                Assert.NotEmpty(RepoFactory.FilterPreset.GetAll());
            }
            finally
            {
                await secondHost.StopAsync(TimeSpan.FromSeconds(30));
            }

            succeeded = true;
        }
        finally
        {
            SQLite.ResetTestState();
            SQLite.UseEfOnlyBootstrapForTests = false;
            Environment.SetEnvironmentVariable("SHOKO_HOME", originalShokoHome);

            if (succeeded && Directory.Exists(tempDir))
                Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task SQLite_EfOnlyBootstrap_RunOnStart_ScansManagedFolderWithoutNhSessionFactory()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"shoko-efonly-runonstart-{Guid.NewGuid():N}");
        var importDir = Path.Combine(tempDir, "import");
        Directory.CreateDirectory(importDir);
        var relativePath = "startup-runonstart.mkv";
        await File.WriteAllBytesAsync(Path.Combine(importDir, relativePath), new byte[4096]);

        var originalShokoHome = Environment.GetEnvironmentVariable("SHOKO_HOME");
        try
        {
            Environment.SetEnvironmentVariable("SHOKO_HOME", tempDir.Replace('\\', '/'));
            SQLite.UseEfOnlyBootstrapForTests = true;
            SQLite.ResetTestState();
            SQLite.ThrowOnSessionFactoryCreateForTests = true;

            var seedHost = await StartServiceAsync(waitForStartupComplete: true);
            int folderId;
            try
            {
                var folder = new ShokoManagedFolder
                {
                    Name = $"RunOnStart-{Guid.NewGuid():N}",
                    Path = importDir,
                    IsWatched = true
                };
                RepoFactory.ShokoManagedFolder.Save(folder);
                folderId = folder.ID;
            }
            finally
            {
                await seedHost.StopAsync(TimeSpan.FromSeconds(30));
            }

            SQLite.ResetTestState();
            SQLite.ThrowOnSessionFactoryCreateForTests = true;

            var host = await StartServiceAsync(
                waitForStartupComplete: true,
                configureSettings: settings =>
                {
                    settings.Import.RunOnStart = true;
                    settings.Import.ScanDropFoldersOnStart = false;
                    settings.Import.FileLockChecking = false;
                    settings.Import.AggressiveFileLockChecking = false;
                });
            try
            {
                await WaitForManagedFolderPlaceOrNhTouchAsync(folderId, relativePath, TimeSpan.FromSeconds(60));
                Assert.Equal(0, SQLite.SessionFactoryCreateCallCount);
            }
            finally
            {
                await host.StopAsync(TimeSpan.FromSeconds(30));
            }
        }
        finally
        {
            SQLite.ResetTestState();
            SQLite.UseEfOnlyBootstrapForTests = false;
            Environment.SetEnvironmentVariable("SHOKO_HOME", originalShokoHome);
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, recursive: true);
        }
    }

    [Fact]
    public async Task SQLite_EfOnlyBootstrap_ScanDropFoldersOnStart_ScansSourceFolderWithoutNhSessionFactory()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"shoko-efonly-scandrop-{Guid.NewGuid():N}");
        var importDir = Path.Combine(tempDir, "drop-source");
        Directory.CreateDirectory(importDir);
        var relativePath = "startup-scandrop.mkv";
        await File.WriteAllBytesAsync(Path.Combine(importDir, relativePath), new byte[4096]);

        var originalShokoHome = Environment.GetEnvironmentVariable("SHOKO_HOME");
        try
        {
            Environment.SetEnvironmentVariable("SHOKO_HOME", tempDir.Replace('\\', '/'));
            SQLite.UseEfOnlyBootstrapForTests = true;
            SQLite.ResetTestState();
            SQLite.ThrowOnSessionFactoryCreateForTests = true;

            var seedHost = await StartServiceAsync(waitForStartupComplete: true);
            int folderId;
            try
            {
                var folder = new ShokoManagedFolder
                {
                    Name = $"ScanDrop-{Guid.NewGuid():N}",
                    Path = importDir,
                    IsWatched = false,
                    DropFolderType = DropFolderType.Source
                };
                RepoFactory.ShokoManagedFolder.Save(folder);
                folderId = folder.ID;
            }
            finally
            {
                await seedHost.StopAsync(TimeSpan.FromSeconds(30));
            }

            SQLite.ResetTestState();
            SQLite.ThrowOnSessionFactoryCreateForTests = true;

            var host = await StartServiceAsync(
                waitForStartupComplete: true,
                configureSettings: settings =>
                {
                    settings.Import.RunOnStart = false;
                    settings.Import.ScanDropFoldersOnStart = true;
                    settings.Import.FileLockChecking = false;
                    settings.Import.AggressiveFileLockChecking = false;
                });
            try
            {
                await WaitForManagedFolderPlaceOrNhTouchAsync(folderId, relativePath, TimeSpan.FromSeconds(60));
                Assert.Equal(0, SQLite.SessionFactoryCreateCallCount);
            }
            finally
            {
                await host.StopAsync(TimeSpan.FromSeconds(30));
            }
        }
        finally
        {
            SQLite.ResetTestState();
            SQLite.UseEfOnlyBootstrapForTests = false;
            Environment.SetEnvironmentVariable("SHOKO_HOME", originalShokoHome);
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, recursive: true);
        }
    }

    private static async Task<IHost> StartServiceAsync(bool waitForStartupComplete = false, Action<Shoko.Server.Settings.IServerSettings>? configureSettings = null)
    {
        var (host, _, _) = await StartServiceUntilAboutToStartAsync(waitForStartupComplete, configureSettings);
        return host;
    }

    private static async Task<(IHost Host, SystemService SystemService, TaskCompletionSource AboutToStart)> StartServiceUntilAboutToStartAsync(
        bool waitForStartupComplete = false,
        Action<Shoko.Server.Settings.IServerSettings>? configureSettings = null)
    {
        var systemService = new SystemService();
        var settings = Utils.SettingsProvider.GetSettings();
        settings.FirstRun = false;
        settings.AniDb.Username = "integration-test";
        settings.AniDb.Password = "integration-test";
        settings.Import.ScanDropFoldersOnStart = false;
        settings.Import.RunOnStart = false;
        settings.Web.Port = GetAvailableTcpPort();
        configureSettings?.Invoke(settings);
        Utils.SettingsProvider.SaveSettings(settings);

        var aboutToStart = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        systemService.AboutToStart += (_, _) => aboutToStart.TrySetResult();

        var host = await systemService.StartAsync();
        Assert.NotNull(host);

        try
        {
            await aboutToStart.Task.WaitAsync(TimeSpan.FromMinutes(10));
            if (waitForStartupComplete)
                await systemService.WaitForStartupAsync().WaitAsync(TimeSpan.FromMinutes(10));
        }
        catch (Exception ex)
        {
            var startupFailure = systemService.StartupFailedException;
            var failureText = startupFailure?.ToString();
            if (startupFailure?.InnerException is not null)
                failureText += Environment.NewLine + startupFailure.InnerException;
            if (startupFailure?.InnerException?.InnerException is not null)
                failureText += Environment.NewLine + startupFailure.InnerException.InnerException;
            throw new InvalidOperationException(failureText ?? ex.ToString(), ex);
        }

        return (host!, systemService, aboutToStart);
    }

    private static async Task WaitForManagedFolderPlaceOrNhTouchAsync(int folderId, string relativePath, TimeSpan timeout)
    {
        using var cancellationTokenSource = new CancellationTokenSource(timeout);
        while (true)
        {
            if (SQLite.SessionFactoryCreateCallCount > 0)
            {
                return;
            }

            using (var scope = Utils.ServiceContainer.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ShokoDbContext>();
                var exists = await context.VideoLocal_Place.AsNoTracking()
                    .AnyAsync(place => place.ManagedFolderID == folderId && place.RelativePath == relativePath, cancellationTokenSource.Token);
                if (exists)
                {
                    return;
                }
            }

            cancellationTokenSource.Token.ThrowIfCancellationRequested();
            await Task.Delay(250, cancellationTokenSource.Token);
        }
    }

    private static ushort GetAvailableTcpPort()
    {
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        return (ushort)((IPEndPoint)listener.LocalEndpoint).Port;
    }
}
