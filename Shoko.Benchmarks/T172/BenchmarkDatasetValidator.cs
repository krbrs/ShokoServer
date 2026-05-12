using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Shoko.Server.Data;
using Shoko.Server.Data.SchemaComparison;

namespace Benchmarks.T172;

internal sealed class BenchmarkDatasetValidationSettings
{
    public const string ValidateDatasetEnvironmentVariable = "SHOKO_BENCH_VALIDATE_DATASET";
    public const string ApplyBaselineEnvironmentVariable = "SHOKO_BENCH_VALIDATE_APPLY_BASELINE";

    public bool ApplyBaselineRegistration { get; init; }

    public static bool IsRequested()
        => bool.TryParse(Environment.GetEnvironmentVariable(ValidateDatasetEnvironmentVariable), out var requested) && requested;

    public static BenchmarkDatasetValidationSettings LoadFromEnvironment()
        => new()
        {
            ApplyBaselineRegistration = bool.TryParse(Environment.GetEnvironmentVariable(ApplyBaselineEnvironmentVariable), out var applyBaseline) && applyBaseline
        };
}

internal sealed class BenchmarkDatasetValidationReport
{
    public required string ProviderName { get; init; }
    public required string InitialCreateMigrationId { get; init; }
    public required bool MigrationsHistoryExistsBeforeBaseline { get; init; }
    public required int BaseTableCountBeforeBaseline { get; init; }
    public required bool SchemaMatchesBeforeBaseline { get; init; }
    public required int SchemaErrorsBeforeBaseline { get; init; }
    public required int SchemaWarningsBeforeBaseline { get; init; }
    public required bool BaselineApplied { get; init; }
    public required bool? BaselineSucceeded { get; init; }
    public required string? RegisteredBaselineMigrationId { get; init; }
    public required bool? MigrationsHistoryExistsAfterBaseline { get; init; }
    public required int? BaseTableCountAfterBaseline { get; init; }
    public required bool? SchemaMatchesAfterBaseline { get; init; }
    public required int? SchemaErrorsAfterBaseline { get; init; }
    public required int? SchemaWarningsAfterBaseline { get; init; }
    public required bool DryRunSucceeded { get; init; }
    public required string? DryRunFailureMessage { get; init; }
    public required IReadOnlyList<BenchmarkDryRunResult> DryRunResults { get; init; }
}

internal static class BenchmarkDatasetValidator
{
    public static BenchmarkDatasetValidationReport Run()
    {
        var validationSettings = BenchmarkDatasetValidationSettings.LoadFromEnvironment();
        var harnessSettings = BenchmarkHarnessSettings.LoadFromEnvironment();
        harnessSettings.ValidateForDatabaseBenchmarks();

        BenchmarkBootstrapper.Initialize(harnessSettings);

        using var context = CreateContext(harnessSettings);
        var providerName = context.Database.ProviderName ?? harnessSettings.Provider.ToString();
        var initialCreateMigrationId = context.Database.GetMigrations()
            .Single(migrationId => migrationId.EndsWith("_InitialCreate", StringComparison.Ordinal));

        var schemaBefore = new SchemaComparer(context).Compare();
        var historyExistsBefore = MigrationsHistoryExists(context);
        var tableCountBefore = GetBaseTableCount(context);

        BaselineRegistrationResult? baselineResult = null;
        bool? historyExistsAfter = null;
        int? tableCountAfter = null;
        SchemaComparisonResult? schemaAfter = null;

        if (validationSettings.ApplyBaselineRegistration)
        {
            baselineResult = new BaselineRegistration(context, initialCreateMigrationId, "9.0.0").RegisterBaseline();
            historyExistsAfter = MigrationsHistoryExists(context);
            tableCountAfter = GetBaseTableCount(context);
            schemaAfter = new SchemaComparer(context).Compare();
        }

        var dryRunSettings = new BenchmarkHarnessSettings
        {
            Provider = harnessSettings.Provider,
            ConnectionString = harnessSettings.ConnectionString,
            Mode = harnessSettings.Mode,
            DryRun = true,
            RequestedScenarios = harnessSettings.RequestedScenarios
        };

        IReadOnlyList<BenchmarkDryRunResult> dryRunResults;
        string? dryRunFailureMessage = null;
        var dryRunSucceeded = false;
        using (var harness = new BenchmarkDatabaseHarness(dryRunSettings))
        {
            try
            {
                dryRunResults = harness.RunDry();
                dryRunSucceeded = true;
            }
            catch (Exception ex)
            {
                dryRunResults = [];
                dryRunFailureMessage = ex.GetBaseException().Message;
            }
        }

        return new BenchmarkDatasetValidationReport
        {
            ProviderName = providerName,
            InitialCreateMigrationId = initialCreateMigrationId,
            MigrationsHistoryExistsBeforeBaseline = historyExistsBefore,
            BaseTableCountBeforeBaseline = tableCountBefore,
            SchemaMatchesBeforeBaseline = schemaBefore.IsValid,
            SchemaErrorsBeforeBaseline = schemaBefore.Errors.Count,
            SchemaWarningsBeforeBaseline = schemaBefore.Warnings.Count,
            BaselineApplied = validationSettings.ApplyBaselineRegistration,
            BaselineSucceeded = baselineResult?.Success,
            RegisteredBaselineMigrationId = baselineResult?.RegisteredMigrationId,
            MigrationsHistoryExistsAfterBaseline = historyExistsAfter,
            BaseTableCountAfterBaseline = tableCountAfter,
            SchemaMatchesAfterBaseline = schemaAfter?.IsValid,
            SchemaErrorsAfterBaseline = schemaAfter?.Errors.Count,
            SchemaWarningsAfterBaseline = schemaAfter?.Warnings.Count,
            DryRunSucceeded = dryRunSucceeded,
            DryRunFailureMessage = dryRunFailureMessage,
            DryRunResults = dryRunResults
        };
    }

    private static ShokoDbContext CreateContext(BenchmarkHarnessSettings settings)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ShokoDbContext>();
        optionsBuilder.ConfigureShokoDbContext(settings.Provider switch
        {
            BenchmarkProviderType.SQLite => EFCoreDatabaseProvider.SQLite,
            BenchmarkProviderType.MariaDB => EFCoreDatabaseProvider.MariaDB,
            BenchmarkProviderType.SQLServer => EFCoreDatabaseProvider.SQLServer,
            _ => throw new InvalidOperationException($"Unsupported benchmark provider {settings.Provider}")
        }, settings.ConnectionString);
        return new ShokoDbContext(optionsBuilder.Options);
    }

    private static bool MigrationsHistoryExists(DbContext context)
    {
        using var connection = CreateProviderConnection(context);
        using var command = connection.CreateCommand();
        command.CommandText = context.Database.ProviderName?.Contains("SqlServer", StringComparison.OrdinalIgnoreCase) == true
            ? """
              SELECT COUNT(*)
              FROM INFORMATION_SCHEMA.TABLES
              WHERE TABLE_NAME = '__EFMigrationsHistory';
              """
            : throw new InvalidOperationException($"Unsupported provider {context.Database.ProviderName} for benchmark dataset validation.");

        var result = command.ExecuteScalar();
        return Convert.ToInt32(result) > 0;
    }

    private static int GetBaseTableCount(DbContext context)
    {
        using var connection = CreateProviderConnection(context);
        using var command = connection.CreateCommand();
        command.CommandText = context.Database.ProviderName?.Contains("SqlServer", StringComparison.OrdinalIgnoreCase) == true
            ? """
              SELECT COUNT(*)
              FROM INFORMATION_SCHEMA.TABLES
              WHERE TABLE_TYPE = 'BASE TABLE';
              """
            : throw new InvalidOperationException($"Unsupported provider {context.Database.ProviderName} for benchmark dataset validation.");

        var result = command.ExecuteScalar();
        return Convert.ToInt32(result);
    }

    private static DbConnection CreateProviderConnection(DbContext context)
    {
        var connectionString = context.Database.GetConnectionString()
            ?? throw new InvalidOperationException("Benchmark dataset validation requires a configured connection string.");
        var connection = context.Database.GetDbConnection();
        var clonedConnection = connection switch
        {
            Microsoft.Data.SqlClient.SqlConnection => new Microsoft.Data.SqlClient.SqlConnection(connectionString),
            _ => throw new InvalidOperationException($"Unsupported provider connection {connection.GetType().FullName} for benchmark dataset validation.")
        };

        clonedConnection.Open();
        return clonedConnection;
    }
}
