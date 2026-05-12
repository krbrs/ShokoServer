using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shoko.Server.Data;
using Shoko.Server.Data.SchemaComparison;
using Shoko.Server.Models.AniDB;
using Shoko.Server.Models.Legacy;
using Shoko.Server.Models.Release;
using Shoko.Server.Models.Shoko;
using Shoko.Server.Server;
using Shoko.Server.Services;
using Shoko.Server.Utilities;
using Versions = Shoko.Server.Models.Internal.Versions;

namespace Benchmarks.T172;

internal sealed class SqliteBenchmarkDatasetPreparationSettings
{
    public const string SourceDatabasePathEnvironmentVariable = "SHOKO_BENCH_PREP_SOURCE_DB";
    public const string WorkDatabasePathEnvironmentVariable = "SHOKO_BENCH_PREP_WORK_DB";
    public const string KeepWorkDatabaseEnvironmentVariable = "SHOKO_BENCH_PREP_KEEP_WORK_DB";
    public const string ApplyBaselineEnvironmentVariable = "SHOKO_BENCH_PREP_APPLY_BASELINE";

    public required string SourceDatabasePath { get; init; }
    public required string WorkDatabasePath { get; init; }
    public bool KeepWorkDatabase { get; init; } = true;
    public bool ApplyBaselineRegistration { get; init; }

    public static bool IsRequested() => !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(SourceDatabasePathEnvironmentVariable));

    public static SqliteBenchmarkDatasetPreparationSettings LoadFromEnvironment()
    {
        var sourceDatabasePath = Environment.GetEnvironmentVariable(SourceDatabasePathEnvironmentVariable) ?? string.Empty;
        var workDatabasePath = Environment.GetEnvironmentVariable(WorkDatabasePathEnvironmentVariable) ?? string.Empty;
        var keepWorkDatabaseText = Environment.GetEnvironmentVariable(KeepWorkDatabaseEnvironmentVariable);
        var applyBaselineText = Environment.GetEnvironmentVariable(ApplyBaselineEnvironmentVariable);

        if (string.IsNullOrWhiteSpace(sourceDatabasePath))
        {
            throw new InvalidOperationException(
                $"SQLite benchmark dataset preparation requires {SourceDatabasePathEnvironmentVariable} to be set.");
        }

        if (string.IsNullOrWhiteSpace(workDatabasePath))
        {
            throw new InvalidOperationException(
                $"SQLite benchmark dataset preparation requires {WorkDatabasePathEnvironmentVariable} to be set.");
        }

        return new SqliteBenchmarkDatasetPreparationSettings
        {
            SourceDatabasePath = Path.GetFullPath(sourceDatabasePath),
            WorkDatabasePath = Path.GetFullPath(workDatabasePath),
            KeepWorkDatabase = !bool.TryParse(keepWorkDatabaseText, out var keepWorkDatabase) || keepWorkDatabase,
            ApplyBaselineRegistration = bool.TryParse(applyBaselineText, out var applyBaseline) && applyBaseline
        };
    }
}

internal sealed class SqliteBenchmarkDatasetPreparationReport
{
    public required long SourceFileSizeBytesBefore { get; init; }
    public required long SourceFileSizeBytesAfter { get; init; }
    public required string SourceSha256Before { get; init; }
    public required string SourceSha256After { get; init; }
    public required string WorkConnectionString { get; init; }
    public required bool StartupSucceeded { get; init; }
    public required string? StartupFailureMessage { get; init; }
    public required string? DatabaseVersionSummary { get; init; }
    public required bool SchemaMatchesBeforeBaseline { get; init; }
    public required int SchemaErrorsBeforeBaseline { get; init; }
    public required int SchemaWarningsBeforeBaseline { get; init; }
    public required bool BaselineApplied { get; init; }
    public required bool? BaselineSucceeded { get; init; }
    public required string? RegisteredBaselineMigrationId { get; init; }
    public required bool? SchemaMatchesAfterBaseline { get; init; }
    public required int? SchemaErrorsAfterBaseline { get; init; }
    public required int? SchemaWarningsAfterBaseline { get; init; }
    public required IReadOnlyDictionary<string, int> RowCounts { get; init; }
    public required IReadOnlyList<BenchmarkDryRunResult> DryRunResults { get; init; }

    public required bool PreflightDatabaseIsReadable { get; init; }
    public required string? PreflightReadabilityError { get; init; }
    public required string PreflightIntegrityCheckResult { get; init; }
    public required bool PreflightIntegrityCheckPassed { get; init; }
    public required int PreflightForeignKeyViolationCount { get; init; }
    public required int PreflightInvalidIndexCount { get; init; }
    public required int PreflightInvalidTriggerCount { get; init; }
    public required int PreflightInvalidViewCount { get; init; }
    public required int PreflightTotalSchemaObjects { get; init; }
    public required bool PreflightRepairEnabled { get; init; }
    public required int PreflightRepairedObjectsCount { get; init; }
    public required int PreflightSkippedUnsafeObjectsCount { get; init; }
    public required string? PreflightRepairError { get; init; }

    public required string PreflightMalformedIndexName { get; init; }
    public required bool PreflightMalformedIndexExists { get; init; }
    public required string? PreflightMalformedIndexSql { get; init; }
    public required bool PreflightMalformedIndexTableExists { get; init; }
    public required string? PreflightMalformedIndexTableName { get; init; }
    public required string PreflightMalformedIndexClassification { get; init; }
    public required bool PreflightMalformedIndexRepaired { get; init; }
    public required string? PreflightMalformedIndexRepairResult { get; init; }
}

internal static class SqliteBenchmarkDatasetPreparer
{
    public static SqliteBenchmarkDatasetPreparationReport Run()
    {
        var settings = SqliteBenchmarkDatasetPreparationSettings.LoadFromEnvironment();
        ValidateSettings(settings);

        var sourceInfoBefore = new FileInfo(settings.SourceDatabasePath);
        var sourceSha256Before = ComputeSha256(settings.SourceDatabasePath);

        PrepareWorkDatabase(settings);

        var preflightInspection = SqlitePreflightInspector.Inspect(settings.WorkDatabasePath);

        var preflightRepair = SqlitePreflightRepairer.Repair(settings.WorkDatabasePath, preflightInspection);

        using var startupSession = RunLegacyStartup(settings.WorkDatabasePath);
        if (!startupSession.Success)
        {
            return new SqliteBenchmarkDatasetPreparationReport
            {
                SourceFileSizeBytesBefore = sourceInfoBefore.Length,
                SourceFileSizeBytesAfter = new FileInfo(settings.SourceDatabasePath).Length,
                SourceSha256Before = sourceSha256Before,
                SourceSha256After = ComputeSha256(settings.SourceDatabasePath),
                WorkConnectionString = CreateConnectionString(settings.WorkDatabasePath),
                StartupSucceeded = false,
                StartupFailureMessage = startupSession.FailureMessage,
                DatabaseVersionSummary = null,
                SchemaMatchesBeforeBaseline = false,
                SchemaErrorsBeforeBaseline = 0,
                SchemaWarningsBeforeBaseline = 0,
                BaselineApplied = false,
                BaselineSucceeded = null,
                RegisteredBaselineMigrationId = null,
                SchemaMatchesAfterBaseline = null,
                SchemaErrorsAfterBaseline = null,
                SchemaWarningsAfterBaseline = null,
                RowCounts = new Dictionary<string, int>(),
                DryRunResults = [],
                PreflightDatabaseIsReadable = preflightInspection.DatabaseIsReadable,
                PreflightReadabilityError = preflightInspection.ReadabilityError,
                PreflightIntegrityCheckResult = preflightInspection.IntegrityCheckResult,
                PreflightIntegrityCheckPassed = preflightInspection.IntegrityCheckPassed,
                PreflightForeignKeyViolationCount = preflightInspection.ForeignKeyViolations.Count,
                PreflightInvalidIndexCount = preflightInspection.InvalidIndexes.Count,
                PreflightInvalidTriggerCount = preflightInspection.InvalidTriggers.Count,
                PreflightInvalidViewCount = preflightInspection.InvalidViews.Count,
                PreflightTotalSchemaObjects = preflightInspection.TotalSchemaObjects,
                PreflightRepairEnabled = preflightRepair.RepairEnabled,
                PreflightRepairedObjectsCount = preflightRepair.RepairedObjectsCount,
                PreflightSkippedUnsafeObjectsCount = preflightRepair.SkippedUnsafeObjects.Count,
                PreflightRepairError = preflightRepair.RepairError,
                PreflightMalformedIndexName = preflightInspection.MalformedIndexInvestigation.IndexName,
                PreflightMalformedIndexExists = preflightInspection.MalformedIndexInvestigation.IndexExists,
                PreflightMalformedIndexSql = preflightInspection.MalformedIndexInvestigation.IndexSql,
                PreflightMalformedIndexTableExists = preflightInspection.MalformedIndexInvestigation.TableExists,
                PreflightMalformedIndexTableName = preflightInspection.MalformedIndexInvestigation.TableName,
                PreflightMalformedIndexClassification = preflightInspection.MalformedIndexInvestigation.CorruptionClassification,
                PreflightMalformedIndexRepaired = preflightRepair.MalformedIndexRepaired,
                PreflightMalformedIndexRepairResult = preflightRepair.MalformedIndexRepairResult
            };
        }

        var connectionString = CreateConnectionString(settings.WorkDatabasePath);
        using var context = CreateContext(connectionString);

        var versionSummary = GetVersionSummary(context);
        var schemaBefore = new SchemaComparer(context).Compare();

        BaselineRegistrationResult? baselineResult = null;
        SchemaComparisonResult? schemaAfter = null;
        if (settings.ApplyBaselineRegistration)
        {
            baselineResult = new BaselineRegistration(context, baselineMigrationId: "20260509114039_InitialCreate", productVersion: "9.0.0")
                .RegisterBaseline();
            schemaAfter = new SchemaComparer(context).Compare();
        }

        var rowCounts = GetRowCounts(context);

        var harnessSettings = new BenchmarkHarnessSettings
        {
            Provider = BenchmarkProviderType.SQLite,
            ConnectionString = connectionString,
            Mode = BenchmarkMode.Both,
            DryRun = true,
            RequestedScenarios = []
        };

        IReadOnlyList<BenchmarkDryRunResult> dryRunResults;
        using (var harness = new BenchmarkDatabaseHarness(harnessSettings))
        {
            dryRunResults = harness.RunDry();
        }

        var sourceInfoAfter = new FileInfo(settings.SourceDatabasePath);
        var sourceSha256After = ComputeSha256(settings.SourceDatabasePath);

        return new SqliteBenchmarkDatasetPreparationReport
        {
            SourceFileSizeBytesBefore = sourceInfoBefore.Length,
            SourceFileSizeBytesAfter = sourceInfoAfter.Length,
            SourceSha256Before = sourceSha256Before,
            SourceSha256After = sourceSha256After,
            WorkConnectionString = connectionString,
            StartupSucceeded = true,
            StartupFailureMessage = null,
            DatabaseVersionSummary = versionSummary,
            SchemaMatchesBeforeBaseline = schemaBefore.IsValid,
            SchemaErrorsBeforeBaseline = schemaBefore.Errors.Count,
            SchemaWarningsBeforeBaseline = schemaBefore.Warnings.Count,
            BaselineApplied = settings.ApplyBaselineRegistration,
            BaselineSucceeded = baselineResult?.Success,
            RegisteredBaselineMigrationId = baselineResult?.RegisteredMigrationId,
            SchemaMatchesAfterBaseline = schemaAfter?.IsValid,
            SchemaErrorsAfterBaseline = schemaAfter?.Errors.Count,
            SchemaWarningsAfterBaseline = schemaAfter?.Warnings.Count,
            RowCounts = rowCounts,
            DryRunResults = dryRunResults,
            PreflightDatabaseIsReadable = preflightInspection.DatabaseIsReadable,
            PreflightReadabilityError = preflightInspection.ReadabilityError,
            PreflightIntegrityCheckResult = preflightInspection.IntegrityCheckResult,
            PreflightIntegrityCheckPassed = preflightInspection.IntegrityCheckPassed,
            PreflightForeignKeyViolationCount = preflightInspection.ForeignKeyViolations.Count,
            PreflightInvalidIndexCount = preflightInspection.InvalidIndexes.Count,
            PreflightInvalidTriggerCount = preflightInspection.InvalidTriggers.Count,
            PreflightInvalidViewCount = preflightInspection.InvalidViews.Count,
            PreflightTotalSchemaObjects = preflightInspection.TotalSchemaObjects,
            PreflightRepairEnabled = preflightRepair.RepairEnabled,
            PreflightRepairedObjectsCount = preflightRepair.RepairedObjectsCount,
            PreflightSkippedUnsafeObjectsCount = preflightRepair.SkippedUnsafeObjects.Count,
            PreflightRepairError = preflightRepair.RepairError,
            PreflightMalformedIndexName = preflightInspection.MalformedIndexInvestigation.IndexName,
            PreflightMalformedIndexExists = preflightInspection.MalformedIndexInvestigation.IndexExists,
            PreflightMalformedIndexSql = preflightInspection.MalformedIndexInvestigation.IndexSql,
            PreflightMalformedIndexTableExists = preflightInspection.MalformedIndexInvestigation.TableExists,
            PreflightMalformedIndexTableName = preflightInspection.MalformedIndexInvestigation.TableName,
            PreflightMalformedIndexClassification = preflightInspection.MalformedIndexInvestigation.CorruptionClassification,
            PreflightMalformedIndexRepaired = preflightRepair.MalformedIndexRepaired,
            PreflightMalformedIndexRepairResult = preflightRepair.MalformedIndexRepairResult
        };
    }

    private static void ValidateSettings(SqliteBenchmarkDatasetPreparationSettings settings)
    {
        if (!File.Exists(settings.SourceDatabasePath))
        {
            throw new FileNotFoundException("Source SQLite benchmark dataset was not found.", settings.SourceDatabasePath);
        }

        if (string.Equals(settings.SourceDatabasePath, settings.WorkDatabasePath, StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Source and work SQLite database paths must be different.");
        }
    }

    private static void PrepareWorkDatabase(SqliteBenchmarkDatasetPreparationSettings settings)
    {
        var workDirectory = Path.GetDirectoryName(settings.WorkDatabasePath);
        if (!string.IsNullOrWhiteSpace(workDirectory))
        {
            Directory.CreateDirectory(workDirectory);
        }

        File.Copy(settings.SourceDatabasePath, settings.WorkDatabasePath, overwrite: true);
    }

    private static LegacyStartupSession RunLegacyStartup(string workDatabasePath)
    {
        var tempHome = Path.Combine(Path.GetTempPath(), $"shoko-benchmark-prep-{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempHome);

        var originalShokoHome = Environment.GetEnvironmentVariable("SHOKO_HOME");
        var originalDatabaseType = Environment.GetEnvironmentVariable("DB_TYPE");
        var originalDatabaseFilename = Environment.GetEnvironmentVariable("DB_SQLITE_FILENAME");
        var originalDatabaseDirectory = Environment.GetEnvironmentVariable("DB_SQLITE_DIRECTORY");
        var originalDatabaseConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
        var workDatabaseDirectory = Path.GetDirectoryName(workDatabasePath) ?? string.Empty;
        var workDatabaseFilename = Path.GetFileName(workDatabasePath);

        try
        {
            Environment.SetEnvironmentVariable("SHOKO_HOME", tempHome.Replace('\\', '/'));
            Environment.SetEnvironmentVariable("DB_TYPE", Constants.DatabaseType.SQLite.ToString());
            Environment.SetEnvironmentVariable("DB_SQLITE_DIRECTORY", workDatabaseDirectory);
            Environment.SetEnvironmentVariable("DB_SQLITE_FILENAME", workDatabaseFilename);
            Environment.SetEnvironmentVariable("DB_CONNECTION_STRING", null);

            var systemService = new SystemService();
            var settings = Utils.SettingsProvider.GetSettings();
            settings.FirstRun = false;
            settings.AniDb.Username = "benchmark-prep";
            settings.AniDb.Password = "benchmark-prep";
            settings.DumpSettingsOnStart = false;
            settings.Web.Port = checked((ushort)GetAvailablePort());

            var started = new ManualResetEventSlim(false);
            var startupSucceeded = false;
            string? failureMessage = null;

            systemService.Started += (_, _) =>
            {
                startupSucceeded = true;
                started.Set();
            };
            systemService.StartupFailed += (_, args) =>
            {
                startupSucceeded = false;
                failureMessage = args.Exception?.Message ?? "Startup failed";
                started.Set();
            };

            var host = systemService.StartAsync().GetAwaiter().GetResult();
            if (host is null)
            {
                return new LegacyStartupSession(
                    null,
                    false,
                    systemService.StartupFailedException?.Message ?? "StartAsync returned null host",
                    tempHome,
                    originalShokoHome,
                    originalDatabaseType,
                    originalDatabaseFilename,
                    originalDatabaseDirectory,
                    originalDatabaseConnectionString);
            }

            if (!started.Wait(TimeSpan.FromMinutes(10)))
            {
                return new LegacyStartupSession(
                    host,
                    false,
                    "Database initialization timed out after 10 minutes.",
                    tempHome,
                    originalShokoHome,
                    originalDatabaseType,
                    originalDatabaseFilename,
                    originalDatabaseDirectory,
                    originalDatabaseConnectionString);
            }

            return new LegacyStartupSession(
                host,
                startupSucceeded,
                failureMessage,
                tempHome,
                originalShokoHome,
                originalDatabaseType,
                originalDatabaseFilename,
                originalDatabaseDirectory,
                originalDatabaseConnectionString);
        }
        catch
        {
            Environment.SetEnvironmentVariable("SHOKO_HOME", originalShokoHome);
            Environment.SetEnvironmentVariable("DB_TYPE", originalDatabaseType);
            Environment.SetEnvironmentVariable("DB_SQLITE_FILENAME", originalDatabaseFilename);
            Environment.SetEnvironmentVariable("DB_SQLITE_DIRECTORY", originalDatabaseDirectory);
            Environment.SetEnvironmentVariable("DB_CONNECTION_STRING", originalDatabaseConnectionString);

            try
            {
                if (Directory.Exists(tempHome))
                {
                    Directory.Delete(tempHome, recursive: true);
                }
            }
            catch
            {
                // best-effort cleanup only
            }

            throw;
        }
    }

    private static int GetAvailablePort()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        try
        {
            return ((IPEndPoint)listener.LocalEndpoint).Port;
        }
        finally
        {
            listener.Stop();
        }
    }

    private static string CreateConnectionString(string workDatabasePath)
        => $"Data Source={workDatabasePath};";

    private static ShokoDbContext CreateContext(string connectionString)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ShokoDbContext>();
        optionsBuilder.ConfigureShokoDbContext(EFCoreDatabaseProvider.SQLite, connectionString);
        return new ShokoDbContext(optionsBuilder.Options);
    }

    private static string GetVersionSummary(ShokoDbContext context)
    {
        var databaseVersions = context.Set<Versions>()
            .Where(v => v.VersionType == Constants.DatabaseTypeKey)
            .AsEnumerable()
            .Select(v =>
            {
                var versionValue = int.TryParse(v.VersionValue, out var parsedValue) ? parsedValue : -1;
                var revisionValue = int.TryParse(v.VersionRevision, out var parsedRevision) ? parsedRevision : -1;
                return new
                {
                    Version = v,
                    Value = versionValue,
                    Revision = revisionValue
                };
            })
            .OrderByDescending(v => v.Value)
            .ThenByDescending(v => v.Revision)
            .FirstOrDefault();

        if (databaseVersions is null)
        {
            return "No database version rows found";
        }

        return $"{databaseVersions.Version.VersionValue}.{databaseVersions.Version.VersionRevision}";
    }

    private static IReadOnlyDictionary<string, int> GetRowCounts(ShokoDbContext context)
    {
        return new Dictionary<string, int>
        {
            ["AnimeSeries"] = context.Set<AnimeSeries>().Count(),
            ["AnimeEpisode"] = context.Set<AnimeEpisode>().Count(),
            ["VideoLocal"] = context.Set<VideoLocal>().Count(),
            ["VideoLocal_Place"] = context.Set<VideoLocal_Place>().Count(),
            ["CrossRef_File_Episode"] = context.Set<Shoko.Server.Models.CrossReference.CrossRef_File_Episode>().Count(),
            ["StoredReleaseInfo"] = context.Set<StoredReleaseInfo>().Count(),
            ["AniDB_Anime"] = context.Set<AniDB_Anime>().Count(),
            ["AniDB_Episode"] = context.Set<AniDB_Episode>().Count(),
            ["ScanFile"] = context.Set<ScanFile>().Count(),
            ["AniDB_Anime_Relation"] = context.Set<AniDB_Anime_Relation>().Count()
        };
    }

    private static string ComputeSha256(string path)
    {
        using var stream = File.OpenRead(path);
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hash = sha256.ComputeHash(stream);
        return Convert.ToHexString(hash);
    }

    private sealed class LegacyStartupSession(
        IHost? host,
        bool success,
        string? failureMessage,
        string tempHome,
        string? originalShokoHome,
        string? originalDatabaseType,
        string? originalDatabaseFilename,
        string? originalDatabaseDirectory,
        string? originalDatabaseConnectionString) : IDisposable
    {
        public bool Success { get; } = success;
        public string? FailureMessage { get; } = failureMessage;

        public void Dispose()
        {
            try
            {
                host?.StopAsync(TimeSpan.FromSeconds(30)).GetAwaiter().GetResult();
            }
            catch
            {
                // best-effort shutdown only
            }

            try
            {
                host?.Dispose();
            }
            catch
            {
                // best-effort disposal only
            }

            Environment.SetEnvironmentVariable("SHOKO_HOME", originalShokoHome);
            Environment.SetEnvironmentVariable("DB_TYPE", originalDatabaseType);
            Environment.SetEnvironmentVariable("DB_SQLITE_FILENAME", originalDatabaseFilename);
            Environment.SetEnvironmentVariable("DB_SQLITE_DIRECTORY", originalDatabaseDirectory);
            Environment.SetEnvironmentVariable("DB_CONNECTION_STRING", originalDatabaseConnectionString);

            try
            {
                if (Directory.Exists(tempHome))
                {
                    Directory.Delete(tempHome, recursive: true);
                }
            }
            catch
            {
                // best-effort cleanup only
            }
        }
    }
}
