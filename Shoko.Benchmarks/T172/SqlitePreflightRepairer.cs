using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Microsoft.Data.Sqlite;

namespace Benchmarks.T172;

internal sealed class SqlitePreflightRepairResult
{
    public required bool RepairEnabled { get; init; }
    public required int RepairedObjectsCount { get; init; }
    public required IReadOnlyList<RepairedObject> RepairedIndexes { get; init; }
    public required IReadOnlyList<RepairedObject> RepairedTriggers { get; init; }
    public required IReadOnlyList<RepairedObject> RepairedViews { get; init; }
    public required IReadOnlyList<InvalidSchemaObject> SkippedUnsafeObjects { get; init; }
    public required string? RepairError { get; init; }

    public required bool MalformedIndexRepaired { get; init; }
    public required string? RepairedMalformedIndexName { get; init; }
    public required string? MalformedIndexRepairResult { get; init; }
}

internal sealed class RepairedObject
{
    public required string Name { get; init; }
    public required string Type { get; init; }
    public required string Sql { get; init; }
}

internal static class SqlitePreflightRepairer
{
    public const string RepairEnabledEnvironmentVariable = "SHOKO_BENCH_SQLITE_REPAIR_WORK_COPY";

    public static bool IsRepairEnabled()
    {
        var envValue = Environment.GetEnvironmentVariable(RepairEnabledEnvironmentVariable);
        return bool.TryParse(envValue, out var enabled) && enabled;
    }

    public static SqlitePreflightRepairResult Repair(string databasePath, SqlitePreflightInspectionResult inspection)
    {
        if (!IsRepairEnabled())
        {
            return new SqlitePreflightRepairResult
            {
                RepairEnabled = false,
                RepairedObjectsCount = 0,
                RepairedIndexes = Array.Empty<RepairedObject>(),
                RepairedTriggers = Array.Empty<RepairedObject>(),
                RepairedViews = Array.Empty<RepairedObject>(),
                SkippedUnsafeObjects = Array.Empty<InvalidSchemaObject>(),
                RepairError = null,
                MalformedIndexRepaired = false,
                RepairedMalformedIndexName = null,
                MalformedIndexRepairResult = null
            };
        }

        if (!inspection.DatabaseIsReadable)
        {
            return new SqlitePreflightRepairResult
            {
                RepairEnabled = true,
                RepairedObjectsCount = 0,
                RepairedIndexes = Array.Empty<RepairedObject>(),
                RepairedTriggers = Array.Empty<RepairedObject>(),
                RepairedViews = Array.Empty<RepairedObject>(),
                SkippedUnsafeObjects = Array.Empty<InvalidSchemaObject>(),
                RepairError = $"Cannot repair unreadable database: {inspection.ReadabilityError}",
                MalformedIndexRepaired = false,
                RepairedMalformedIndexName = null,
                MalformedIndexRepairResult = "Cannot repair unreadable database"
            };
        }

        try
        {
            using var connection = new SqliteConnection($"Data Source={databasePath};");
            connection.Open();

            var malformedIndexRepair = RepairMalformedIndex(connection, inspection.MalformedIndexInvestigation);

            var repairedIndexes = RepairSafeObjects(connection, inspection.InvalidIndexes, "DROP INDEX IF EXISTS {0};");
            var repairedTriggers = RepairSafeObjects(connection, inspection.InvalidTriggers, "DROP TRIGGER IF EXISTS {0};");
            var repairedViews = RepairSafeObjects(connection, inspection.InvalidViews, "DROP VIEW IF EXISTS {0};");

            var allUnsafe = inspection.InvalidIndexes
                .Where(i => !i.IsSafeToDrop)
                .Concat(inspection.InvalidTriggers.Where(i => !i.IsSafeToDrop))
                .Concat(inspection.InvalidViews.Where(i => !i.IsSafeToDrop))
                .ToList();

            var totalRepaired = repairedIndexes.Count + repairedTriggers.Count + repairedViews.Count;

            return new SqlitePreflightRepairResult
            {
                RepairEnabled = true,
                RepairedObjectsCount = totalRepaired,
                RepairedIndexes = repairedIndexes,
                RepairedTriggers = repairedTriggers,
                RepairedViews = repairedViews,
                SkippedUnsafeObjects = allUnsafe,
                RepairError = null,
                MalformedIndexRepaired = malformedIndexRepair.Repaired,
                RepairedMalformedIndexName = malformedIndexRepair.IndexName,
                MalformedIndexRepairResult = malformedIndexRepair.Result
            };
        }
        catch (Exception ex)
        {
            return new SqlitePreflightRepairResult
            {
                RepairEnabled = true,
                RepairedObjectsCount = 0,
                RepairedIndexes = Array.Empty<RepairedObject>(),
                RepairedTriggers = Array.Empty<RepairedObject>(),
                RepairedViews = Array.Empty<RepairedObject>(),
                SkippedUnsafeObjects = Array.Empty<InvalidSchemaObject>(),
                RepairError = ex.Message,
                MalformedIndexRepaired = false,
                RepairedMalformedIndexName = inspection.MalformedIndexInvestigation.IndexName,
                MalformedIndexRepairResult = $"Repair failed: {ex.Message}"
            };
        }
    }

    private static IReadOnlyList<RepairedObject> RepairSafeObjects(
        SqliteConnection connection,
        IReadOnlyList<InvalidSchemaObject> invalidObjects,
        string dropSqlTemplate)
    {
        var repaired = new List<RepairedObject>();

        foreach (var obj in invalidObjects.Where(o => o.IsSafeToDrop))
        {
            try
            {
                using var command = connection.CreateCommand();
                command.CommandText = string.Format(dropSqlTemplate, obj.Name);
                command.ExecuteNonQuery();

                repaired.Add(new RepairedObject
                {
                    Name = obj.Name,
                    Type = obj.Type,
                    Sql = obj.Sql
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Failed to drop {obj.Type} '{obj.Name}': {ex.Message}");
            }
        }

        return repaired;
    }

    private static (bool Repaired, string IndexName, string Result) RepairMalformedIndex(
        SqliteConnection connection,
        MalformedIndexInvestigation investigation)
    {
        if (!investigation.RepairIsSafe || string.IsNullOrEmpty(investigation.ProposedRepairStatement))
        {
            return (false, investigation.IndexName, $"Repair not safe or no repair statement: {investigation.CorruptionClassification}");
        }

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = investigation.ProposedRepairStatement;
            var rowsAffected = command.ExecuteNonQuery();

            var result = rowsAffected > 0
                ? $"Successfully repaired malformed index '{investigation.IndexName}' - {investigation.CorruptionClassification}"
                : $"Repair statement executed but no rows affected for '{investigation.IndexName}' - {investigation.CorruptionClassification}";

            return (true, investigation.IndexName, result);
        }
        catch (Exception ex)
        {
            return (false, investigation.IndexName, $"Failed to repair malformed index '{investigation.IndexName}': {ex.Message}");
        }
    }
}