using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shoko.Server.Data;
using Shoko.Server.Data.SchemaComparison;
using Shoko.Server.Databases;
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
            Assert.Equal(0, RepoFactory.EfOnlyPopulateSessionCount);

            var firstHost = await StartServiceAsync(waitForStartupComplete: true);
            try
            {
                Assert.True(RepoFactory.EfOnlyPopulateSessionCount > 0);
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
            SQLite.UseEfOnlyBootstrapForTests = false;
            Environment.SetEnvironmentVariable("SHOKO_HOME", originalShokoHome);

            if (succeeded && Directory.Exists(tempDir))
                Directory.Delete(tempDir, recursive: true);
        }
    }

    private static async Task<IHost> StartServiceAsync(bool waitForStartupComplete = false)
    {
        var (host, _, _) = await StartServiceUntilAboutToStartAsync(waitForStartupComplete);
        return host;
    }

    private static async Task<(IHost Host, SystemService SystemService, TaskCompletionSource AboutToStart)> StartServiceUntilAboutToStartAsync(bool waitForStartupComplete = false)
    {
        var systemService = new SystemService();
        var settings = Utils.SettingsProvider.GetSettings();
        settings.FirstRun = false;
        settings.AniDb.Username = "integration-test";
        settings.AniDb.Password = "integration-test";
        settings.Import.ScanDropFoldersOnStart = false;
        settings.Import.RunOnStart = false;
        settings.Web.Port = GetAvailableTcpPort();
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

    private static ushort GetAvailableTcpPort()
    {
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        return (ushort)((IPEndPoint)listener.LocalEndpoint).Port;
    }
}
