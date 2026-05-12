using System;
using System.Collections.Generic;
using System.Linq;

namespace Benchmarks.T172;

public enum BenchmarkProviderType
{
    SQLite,
    MariaDB,
    SQLServer,
}

public enum BenchmarkMode
{
    NHibernate,
    EFCore,
    Both,
}

public sealed class BenchmarkHarnessSettings
{
    public const string ProviderEnvironmentVariable = "SHOKO_BENCH_PROVIDER";
    public const string ConnectionStringEnvironmentVariable = "SHOKO_BENCH_CONNECTION_STRING";
    public const string ModeEnvironmentVariable = "SHOKO_BENCH_MODE";
    public const string ScenarioEnvironmentVariable = "SHOKO_BENCH_SCENARIOS";
    public const string DryRunEnvironmentVariable = "SHOKO_BENCH_DRY_RUN";

    public BenchmarkProviderType Provider { get; init; } = BenchmarkProviderType.SQLite;
    public string ConnectionString { get; init; } = string.Empty;
    public BenchmarkMode Mode { get; init; } = BenchmarkMode.Both;
    public bool DryRun { get; init; }
    public IReadOnlyList<string> RequestedScenarios { get; init; } = [];

    public bool HasDatabaseConfiguration => !string.IsNullOrWhiteSpace(ConnectionString);

    public IReadOnlyList<string> ResolveScenarioIds()
    {
        if (RequestedScenarios.Count == 0)
        {
            return BenchmarkScenarioRegistry.AllScenarioIds;
        }

        if (RequestedScenarios.Count == 1 && RequestedScenarios[0] == "*")
        {
            return BenchmarkScenarioRegistry.AllScenarioIds;
        }

        return RequestedScenarios;
    }

    public void ValidateForDatabaseBenchmarks()
    {
        if (!HasDatabaseConfiguration)
        {
            throw new InvalidOperationException(
                $"Database benchmark harness requires {ConnectionStringEnvironmentVariable} to be set.");
        }

        var unknownScenarioIds = ResolveScenarioIds()
            .Except(BenchmarkScenarioRegistry.AllScenarioIds, StringComparer.OrdinalIgnoreCase)
            .ToList();
        if (unknownScenarioIds.Count > 0)
        {
            throw new InvalidOperationException(
                $"Unknown benchmark scenario id(s): {string.Join(", ", unknownScenarioIds)}");
        }
    }

    public static BenchmarkHarnessSettings LoadFromEnvironment()
    {
        var providerText = Environment.GetEnvironmentVariable(ProviderEnvironmentVariable);
        var modeText = Environment.GetEnvironmentVariable(ModeEnvironmentVariable);
        var scenariosText = Environment.GetEnvironmentVariable(ScenarioEnvironmentVariable);
        var dryRunText = Environment.GetEnvironmentVariable(DryRunEnvironmentVariable);

        return new BenchmarkHarnessSettings
        {
            Provider = ParseProvider(providerText),
            ConnectionString = Environment.GetEnvironmentVariable(ConnectionStringEnvironmentVariable) ?? string.Empty,
            Mode = ParseMode(modeText),
            DryRun = bool.TryParse(dryRunText, out var dryRun) && dryRun,
            RequestedScenarios = ParseScenarios(scenariosText),
        };
    }

    private static BenchmarkProviderType ParseProvider(string? providerText)
    {
        if (string.IsNullOrWhiteSpace(providerText))
        {
            return BenchmarkProviderType.SQLite;
        }

        return providerText.Trim().ToLowerInvariant() switch
        {
            "sqlite" => BenchmarkProviderType.SQLite,
            "mariadb" => BenchmarkProviderType.MariaDB,
            "mysql" => BenchmarkProviderType.MariaDB,
            "sqlserver" => BenchmarkProviderType.SQLServer,
            "sql-server" => BenchmarkProviderType.SQLServer,
            _ => throw new InvalidOperationException(
                $"Unsupported benchmark provider '{providerText}'. Expected SQLite, MariaDB/MySQL, or SQLServer."),
        };
    }

    private static BenchmarkMode ParseMode(string? modeText)
    {
        if (string.IsNullOrWhiteSpace(modeText))
        {
            return BenchmarkMode.Both;
        }

        return modeText.Trim().ToLowerInvariant() switch
        {
            "nh" => BenchmarkMode.NHibernate,
            "nhibernate" => BenchmarkMode.NHibernate,
            "ef" => BenchmarkMode.EFCore,
            "efcore" => BenchmarkMode.EFCore,
            "both" => BenchmarkMode.Both,
            _ => throw new InvalidOperationException(
                $"Unsupported benchmark mode '{modeText}'. Expected NHibernate, EFCore, or Both."),
        };
    }

    private static IReadOnlyList<string> ParseScenarios(string? scenariosText)
    {
        if (string.IsNullOrWhiteSpace(scenariosText))
        {
            return [];
        }

        return scenariosText
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .ToList();
    }
}
