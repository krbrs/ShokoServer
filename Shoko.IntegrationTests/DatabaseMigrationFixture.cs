#nullable enable
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.Data.SqlClient;
using MySqlConnector;
using Microsoft.Extensions.Hosting;
using Shoko.Server.Server;
using Shoko.Server.Services;
using Shoko.Server.Settings;
using Shoko.Server.Utilities;

namespace Shoko.IntegrationTests;

/// <summary>
/// Starts the full Shoko Server bootstrap against an isolated temp directory,
/// then waits for database initialization to complete.
///
/// Database backend is selected via environment variables that mirror
/// <see cref="DatabaseSettings"/>:
///   DB_TYPE   – SQLite (default), SQLServer, MySQL
///   DB_HOST   – hostname[:port] for SQL Server / MySQL
///   DB_USER   – username
///   DB_PASS   – password
///   DB_NAME   – database / schema name
/// </summary>
public sealed class DatabaseMigrationFixture : IDisposable
{
    public bool Success { get; private set; }
    public string? FailureMessage { get; private set; }

    private readonly string _tempDir;
    private readonly string? _originalDatabaseNameEnvironmentValue;
    private readonly string? _testDatabaseName;
    private IHost? _host;

    public DatabaseMigrationFixture()
    {
        // Isolated data directory so this run doesn't touch a real Shoko install.
        _tempDir = Path.Combine(Path.GetTempPath(), $"shoko-integration-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);

        // SHOKO_HOME controls Utils.ApplicationPath. Must be set before SystemService() reads it.
        // Forward slashes avoid bad JSON escape sequences when the config service parses env vars.
        Environment.SetEnvironmentVariable("SHOKO_HOME", _tempDir.Replace('\\', '/'));
        (_originalDatabaseNameEnvironmentValue, _testDatabaseName) = ConfigureIsolatedDatabaseName();

        // SystemService() bootstraps Utils.SettingsProvider with default settings (FirstRun=true).
        // No settings file yet — defaults are valid and pass schema validation.
        var systemService = new SystemService();

        // Mutate the live settings: disable first-run, inject fake AniDB credentials so the
        // settings custom-validator is satisfied, and move the web port away from 8111 so this
        // doesn't conflict with a real Shoko instance.
        var settings = Utils.SettingsProvider.GetSettings();
        settings.FirstRun = false;
        settings.AniDb.Username = "integration-test";
        settings.AniDb.Password = "integration-test";
        settings.Web.Port = GetAvailableTcpPort();
        Utils.SettingsProvider.SaveSettings(settings);

        var started = new ManualResetEventSlim(false);
        systemService.Started += (_, _) =>
        {
            Success = true;
            started.Set();
        };
        systemService.StartupFailed += (_, args) =>
        {
            Success = false;
            FailureMessage = args.Exception?.Message ?? "Startup failed";
            started.Set();
        };

        // StartAsync builds the full DI container (including all services used by database fixes)
        // and sets Utils.ServiceContainer before LateStart triggers InitializeDatabase.
        _host = systemService.StartAsync().GetAwaiter().GetResult();
        if (_host is null)
        {
            Success = false;
            FailureMessage = systemService.StartupFailedException?.Message ?? "StartAsync returned null host";
            return;
        }

        // LateStart runs InitializeDatabase as a fire-and-forget task; wait for its completion event.
        if (!started.Wait(TimeSpan.FromMinutes(10)))
        {
            Success = false;
            FailureMessage = "Database initialization timed out after 10 minutes";
        }
    }

    public void Dispose()
    {
        try
        {
            _host?.StopAsync(TimeSpan.FromSeconds(30)).GetAwaiter().GetResult();
        }
        catch
        {
            // Best-effort shutdown; don't mask test failures.
        }

        try
        {
            DropIsolatedDatabaseSchema();
        }
        catch
        {
            // Database cleanup is best-effort only.
        }

        try
        {
            Environment.SetEnvironmentVariable("DB_NAME", _originalDatabaseNameEnvironmentValue);
        }
        catch
        {
            // Environment cleanup is best-effort only.
        }

        try
        {
            if (Directory.Exists(_tempDir))
                Directory.Delete(_tempDir, recursive: true);
        }
        catch
        {
            // SQLite connections may still be draining; ignore cleanup errors.
        }
    }

    private static (string? OriginalDatabaseName, string? TestDatabaseName) ConfigureIsolatedDatabaseName()
    {
        var databaseType = Environment.GetEnvironmentVariable("DB_TYPE");
        if (!string.Equals(databaseType, Constants.DatabaseType.MySQL.ToString(), StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(databaseType, Constants.DatabaseType.SQLServer.ToString(), StringComparison.OrdinalIgnoreCase))
            return (null, null);

        var originalDatabaseName = Environment.GetEnvironmentVariable("DB_NAME");
        var baseName = string.IsNullOrWhiteSpace(originalDatabaseName) ? "shoko" : originalDatabaseName;
        var isolatedName = $"{baseName}_{Guid.NewGuid():N}".ToLowerInvariant();
        Environment.SetEnvironmentVariable("DB_NAME", isolatedName);
        return (originalDatabaseName, isolatedName);
    }

    private static ushort GetAvailableTcpPort()
    {
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        return (ushort)((IPEndPoint)listener.LocalEndpoint).Port;
    }

    private void DropIsolatedDatabaseSchema()
    {
        if (_testDatabaseName is null)
            return;

        var settings = Utils.SettingsProvider.GetSettings();
        switch (settings.Database.Type)
        {
            case Constants.DatabaseType.MySQL:
            {
                var connectionString = !string.IsNullOrWhiteSpace(settings.Database.OverrideConnectionString)
                    ? settings.Database.OverrideConnectionString
                    : $"Server={settings.Database.Hostname};Port={settings.Database.Port};Database=information_schema;User ID={settings.Database.Username};Password={settings.Database.Password};Default Command Timeout=3600";

                using var connection = new MySqlConnection(connectionString);
                connection.Open();
                using var command = connection.CreateCommand();
                command.CommandText = $"DROP DATABASE IF EXISTS `{_testDatabaseName}`;";
                command.ExecuteNonQuery();
                break;
            }
            case Constants.DatabaseType.SQLServer:
            {
                var connectionString = !string.IsNullOrWhiteSpace(settings.Database.OverrideConnectionString)
                    ? settings.Database.OverrideConnectionString
                    : $"data source={settings.Database.Hostname},{settings.Database.Port};Initial Catalog=master;user id={settings.Database.Username};password={settings.Database.Password};persist security info=True;MultipleActiveResultSets=True;TrustServerCertificate=True";

                using var connection = new SqlConnection(connectionString);
                connection.Open();
                using var command = connection.CreateCommand();
                command.CommandText = $@"
IF DB_ID(N'{_testDatabaseName}') IS NOT NULL
BEGIN
    ALTER DATABASE [{_testDatabaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [{_testDatabaseName}];
END";
                command.ExecuteNonQuery();
                break;
            }
        }
    }
}
