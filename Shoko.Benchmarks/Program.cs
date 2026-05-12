using BenchmarkDotNet.Running;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using Benchmarks;
using Benchmarks.T172;

if (BenchmarkDatasetValidationSettings.IsRequested())
{
    var report = BenchmarkDatasetValidator.Run();

    Console.WriteLine($"VALIDATE provider: {report.ProviderName}");
    Console.WriteLine($"VALIDATE initial-create: {report.InitialCreateMigrationId}");
    Console.WriteLine($"VALIDATE history-before: {report.MigrationsHistoryExistsBeforeBaseline}");
    Console.WriteLine($"VALIDATE tables-before: {report.BaseTableCountBeforeBaseline}");
    Console.WriteLine($"VALIDATE schema-before: valid={report.SchemaMatchesBeforeBaseline} errors={report.SchemaErrorsBeforeBaseline} warnings={report.SchemaWarningsBeforeBaseline}");
    Console.WriteLine($"VALIDATE baseline: applied={report.BaselineApplied} success={report.BaselineSucceeded?.ToString() ?? "n/a"} migration={report.RegisteredBaselineMigrationId ?? "n/a"}");
    Console.WriteLine($"VALIDATE history-after: {report.MigrationsHistoryExistsAfterBaseline?.ToString() ?? "n/a"}");
    Console.WriteLine($"VALIDATE tables-after: {report.BaseTableCountAfterBaseline?.ToString() ?? "n/a"}");
    Console.WriteLine($"VALIDATE schema-after: valid={report.SchemaMatchesAfterBaseline?.ToString() ?? "n/a"} errors={report.SchemaErrorsAfterBaseline?.ToString() ?? "n/a"} warnings={report.SchemaWarningsAfterBaseline?.ToString() ?? "n/a"}");
    Console.WriteLine($"VALIDATE dry-run: success={report.DryRunSucceeded} failure={report.DryRunFailureMessage ?? "n/a"}");

    foreach (var result in report.DryRunResults)
    {
        Console.WriteLine($"DRY {result.Mode,-10} {result.ScenarioId}: {result.ResultCount}");
    }

    return;
}

if (SqliteBenchmarkDatasetPreparationSettings.IsRequested())
{
    var report = SqliteBenchmarkDatasetPreparer.Run();

    Console.WriteLine($"PREP startup: {(report.StartupSucceeded ? "success" : "failed")}");
    Console.WriteLine($"PREP source-unchanged: {report.SourceFileSizeBytesBefore == report.SourceFileSizeBytesAfter && string.Equals(report.SourceSha256Before, report.SourceSha256After, StringComparison.OrdinalIgnoreCase)}");
    Console.WriteLine($"PREP preflight-readable: {report.PreflightDatabaseIsReadable}");
    Console.WriteLine($"PREP preflight-integrity: {report.PreflightIntegrityCheckResult}");
    Console.WriteLine($"PREP preflight-fk-violations: {report.PreflightForeignKeyViolationCount}");
    Console.WriteLine($"PREP preflight-invalid-indexes: {report.PreflightInvalidIndexCount}");
    Console.WriteLine($"PREP preflight-invalid-triggers: {report.PreflightInvalidTriggerCount}");
    Console.WriteLine($"PREP preflight-invalid-views: {report.PreflightInvalidViewCount}");
    Console.WriteLine($"PREP preflight-total-objects: {report.PreflightTotalSchemaObjects}");
    Console.WriteLine($"PREP preflight-repair-enabled: {report.PreflightRepairEnabled}");
    Console.WriteLine($"PREP preflight-repaired: {report.PreflightRepairedObjectsCount}");
    Console.WriteLine($"PREP preflight-skipped-unsafe: {report.PreflightSkippedUnsafeObjectsCount}");
    Console.WriteLine($"PREP malformed-index: {report.PreflightMalformedIndexName}");
    Console.WriteLine($"PREP malformed-index-exists: {report.PreflightMalformedIndexExists}");
    Console.WriteLine($"PREP malformed-index-table: {report.PreflightMalformedIndexTableName ?? "n/a"}");
    Console.WriteLine($"PREP malformed-index-table-exists: {report.PreflightMalformedIndexTableExists}");
    Console.WriteLine($"PREP malformed-index-classification: {report.PreflightMalformedIndexClassification}");
    Console.WriteLine($"PREP malformed-index-repaired: {report.PreflightMalformedIndexRepaired}");
    Console.WriteLine($"PREP malformed-index-repair-result: {report.PreflightMalformedIndexRepairResult ?? "n/a"}");
    Console.WriteLine($"PREP version: {report.DatabaseVersionSummary ?? "n/a"}");
    Console.WriteLine($"PREP schema-before: valid={report.SchemaMatchesBeforeBaseline} errors={report.SchemaErrorsBeforeBaseline} warnings={report.SchemaWarningsBeforeBaseline}");
    Console.WriteLine($"PREP baseline: applied={report.BaselineApplied} success={report.BaselineSucceeded?.ToString() ?? "n/a"} migration={report.RegisteredBaselineMigrationId ?? "n/a"}");
    Console.WriteLine($"PREP schema-after: valid={report.SchemaMatchesAfterBaseline?.ToString() ?? "n/a"} errors={report.SchemaErrorsAfterBaseline?.ToString() ?? "n/a"} warnings={report.SchemaWarningsAfterBaseline?.ToString() ?? "n/a"}");

    foreach (var pair in report.RowCounts.OrderBy(pair => pair.Key, StringComparer.Ordinal))
    {
        Console.WriteLine($"COUNT {pair.Key}={pair.Value}");
    }

    foreach (var result in report.DryRunResults)
    {
        Console.WriteLine($"DRY {result.Mode,-10} {result.ScenarioId}: {result.ResultCount}");
    }

    return;
}

var settings = BenchmarkHarnessSettings.LoadFromEnvironment();

if (!settings.HasDatabaseConfiguration)
{
    BenchmarkRunner.Run<AniDB_AnimeBenchmarks>();
    return;
}

settings.ValidateForDatabaseBenchmarks();

if (settings.DryRun)
{
    using var harness = new BenchmarkDatabaseHarness(settings);
    foreach (var result in harness.RunDry())
    {
        Console.WriteLine($"{result.Mode,-10} {result.ScenarioId}: {result.ResultCount}");
    }

    return;
}

var config = DefaultConfig.Instance
    .WithArtifactsPath($"BenchmarkDotNet.Artifacts/results/{GetProviderDirectoryName(settings.Provider)}");

switch (settings.Mode)
{
    case BenchmarkMode.EFCore:
        BenchmarkRunner.Run<T172EfCoreBenchmarks>(config);
        break;
    case BenchmarkMode.NHibernate:
        BenchmarkRunner.Run<T172NhBenchmarks>(config);
        break;
    case BenchmarkMode.Both:
        BenchmarkRunner.Run<T172EfCoreBenchmarks>(config);
        BenchmarkRunner.Run<T172NhBenchmarks>(config);
        break;
    default:
        throw new InvalidOperationException($"Unsupported benchmark mode {settings.Mode}");
}

static string GetProviderDirectoryName(BenchmarkProviderType provider)
{
    return provider switch
    {
        BenchmarkProviderType.SQLServer => "sqlserver",
        BenchmarkProviderType.SQLite => "sqlite",
        BenchmarkProviderType.MariaDB => "mariadb",
        _ => throw new InvalidOperationException($"Unsupported provider type: {provider}"),
    };
}
