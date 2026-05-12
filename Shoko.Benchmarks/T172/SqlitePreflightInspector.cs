using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Data.Sqlite;

namespace Benchmarks.T172;

internal sealed class SqlitePreflightInspectionResult
{
    public required bool DatabaseIsReadable { get; init; }
    public required string? ReadabilityError { get; init; }
    public required string IntegrityCheckResult { get; init; }
    public required bool IntegrityCheckPassed { get; init; }
    public required IReadOnlyList<string> ForeignKeyViolations { get; init; }
    public required bool ForeignKeyCheckPassed { get; init; }
    public required IReadOnlyList<InvalidSchemaObject> InvalidIndexes { get; init; }
    public required IReadOnlyList<InvalidSchemaObject> InvalidTriggers { get; init; }
    public required IReadOnlyList<InvalidSchemaObject> InvalidViews { get; init; }
    public required string? DatabaseVersion { get; init; }
    public required IReadOnlyList<string> ExistingTableNames { get; init; }
    public required int TotalSchemaObjects { get; init; }

    public required MalformedIndexInvestigation MalformedIndexInvestigation { get; init; }
}

internal sealed class MalformedIndexInvestigation
{
    public required string IndexName { get; init; }
    public required bool IndexExists { get; init; }
    public required string? IndexSql { get; init; }
    public required bool TableExists { get; init; }
    public required string? TableName { get; init; }
    public required IReadOnlyList<string> RelatedTriggers { get; init; }
    public required IReadOnlyList<string> RelatedViews { get; init; }
    public required string CorruptionClassification { get; init; }
    public required string? ProposedRepairStatement { get; init; }
    public required bool RepairIsSafe { get; init; }
}

internal sealed class InvalidSchemaObject
{
    public required string Name { get; init; }
    public required string Type { get; init; }
    public required string Sql { get; init; }
    public required string MissingTable { get; init; }
    public required bool IsSafeToDrop { get; init; }
    public required string SafetyReason { get; init; }
}

internal static class SqlitePreflightInspector
{
    public static SqlitePreflightInspectionResult Inspect(string databasePath)
    {
        if (!File.Exists(databasePath))
        {
            return new SqlitePreflightInspectionResult
            {
                DatabaseIsReadable = false,
                ReadabilityError = $"Database file not found: {databasePath}",
                IntegrityCheckResult = string.Empty,
                IntegrityCheckPassed = false,
                ForeignKeyViolations = Array.Empty<string>(),
                ForeignKeyCheckPassed = false,
                InvalidIndexes = Array.Empty<InvalidSchemaObject>(),
                InvalidTriggers = Array.Empty<InvalidSchemaObject>(),
                InvalidViews = Array.Empty<InvalidSchemaObject>(),
                DatabaseVersion = null,
                ExistingTableNames = Array.Empty<string>(),
                TotalSchemaObjects = 0,
                MalformedIndexInvestigation = new MalformedIndexInvestigation
                {
                    IndexName = "IX_AniDB_Episode_EpisodeType",
                    IndexExists = false,
                    IndexSql = null,
                    TableExists = false,
                    TableName = null,
                    RelatedTriggers = Array.Empty<string>(),
                    RelatedViews = Array.Empty<string>(),
                    CorruptionClassification = "Database file not found - cannot investigate",
                    ProposedRepairStatement = null,
                    RepairIsSafe = false
                }
            };
        }

        try
        {
            using var connection = new SqliteConnection($"Data Source={databasePath};");
            connection.Open();

            var integrityCheck = RunIntegrityCheck(connection);
            var foreignKeyViolations = RunForeignKeyCheck(connection);
            var invalidIndexes = FindInvalidIndexes(connection);
            var invalidTriggers = FindInvalidTriggers(connection);
            var invalidViews = FindInvalidViews(connection);
            var databaseVersion = GetDatabaseVersion(connection);
            var existingTables = GetExistingTables(connection);
            var totalSchemaObjects = GetTotalSchemaObjects(connection);
            var malformedIndexInvestigation = InvestigateMalformedIndex(connection, "IX_AniDB_Episode_EpisodeType");

            return new SqlitePreflightInspectionResult
            {
                DatabaseIsReadable = true,
                ReadabilityError = null,
                IntegrityCheckResult = integrityCheck.Result,
                IntegrityCheckPassed = integrityCheck.Passed,
                ForeignKeyViolations = foreignKeyViolations,
                ForeignKeyCheckPassed = foreignKeyViolations.Count == 0,
                InvalidIndexes = invalidIndexes,
                InvalidTriggers = invalidTriggers,
                InvalidViews = invalidViews,
                DatabaseVersion = databaseVersion,
                ExistingTableNames = existingTables,
                TotalSchemaObjects = totalSchemaObjects,
                MalformedIndexInvestigation = malformedIndexInvestigation
            };
        }
        catch (Exception ex)
        {
            return new SqlitePreflightInspectionResult
            {
                DatabaseIsReadable = false,
                ReadabilityError = ex.Message,
                IntegrityCheckResult = string.Empty,
                IntegrityCheckPassed = false,
                ForeignKeyViolations = Array.Empty<string>(),
                ForeignKeyCheckPassed = false,
                InvalidIndexes = Array.Empty<InvalidSchemaObject>(),
                InvalidTriggers = Array.Empty<InvalidSchemaObject>(),
                InvalidViews = Array.Empty<InvalidSchemaObject>(),
                DatabaseVersion = null,
                ExistingTableNames = Array.Empty<string>(),
                TotalSchemaObjects = 0,
                MalformedIndexInvestigation = new MalformedIndexInvestigation
                {
                    IndexName = "IX_AniDB_Episode_EpisodeType",
                    IndexExists = false,
                    IndexSql = null,
                    TableExists = false,
                    TableName = null,
                    RelatedTriggers = Array.Empty<string>(),
                    RelatedViews = Array.Empty<string>(),
                    CorruptionClassification = $"Exception during investigation: {ex.Message}",
                    ProposedRepairStatement = null,
                    RepairIsSafe = false
                }
            };
        }
    }

    private static (string Result, bool Passed) RunIntegrityCheck(SqliteConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = "PRAGMA integrity_check;";

        var result = command.ExecuteScalar() as string ?? string.Empty;

        return (result, result.Equals("ok", StringComparison.OrdinalIgnoreCase));
    }

    private static IReadOnlyList<string> RunForeignKeyCheck(SqliteConnection connection)
    {
        var violations = new List<string>();

        using var command = connection.CreateCommand();
        command.CommandText = "PRAGMA foreign_key_check;";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var table = reader.GetString("table");
            var rowid = reader.GetInt64("rowid");
            var parent = reader.GetString("parent");
            var fkid = reader.GetInt32("fkid");

            violations.Add($"Table '{table}', row {rowid}, references '{parent}' via FK #{fkid}");
        }

        return violations;
    }

    private static IReadOnlyList<InvalidSchemaObject> FindInvalidIndexes(SqliteConnection connection)
    {
        var invalidIndexes = new List<InvalidSchemaObject>();
        var existingTables = GetExistingTables(connection);

        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT name, sql, tbl_name
            FROM sqlite_master
            WHERE type = 'index'
            AND name NOT LIKE 'sqlite_%'
            AND sql IS NOT NULL;";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var indexName = reader.GetString("name");
            var sql = reader.GetString("sql");
            var tableName = reader.GetString("tbl_name");

            if (!existingTables.Contains(tableName, StringComparer.OrdinalIgnoreCase))
            {
                var (isSafe, reason) = ClassifySafety(indexName, "index", tableName, sql);
                invalidIndexes.Add(new InvalidSchemaObject
                {
                    Name = indexName,
                    Type = "index",
                    Sql = sql,
                    MissingTable = tableName,
                    IsSafeToDrop = isSafe,
                    SafetyReason = reason
                });
            }
        }

        return invalidIndexes;
    }

    private static IReadOnlyList<InvalidSchemaObject> FindInvalidTriggers(SqliteConnection connection)
    {
        var invalidTriggers = new List<InvalidSchemaObject>();
        var existingTables = GetExistingTables(connection);

        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT name, sql, tbl_name
            FROM sqlite_master
            WHERE type = 'trigger'
            AND sql IS NOT NULL;";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var triggerName = reader.GetString("name");
            var sql = reader.GetString("sql");
            var tableName = reader.GetString("tbl_name");

            if (!existingTables.Contains(tableName, StringComparer.OrdinalIgnoreCase))
            {
                var (isSafe, reason) = ClassifySafety(triggerName, "trigger", tableName, sql);
                invalidTriggers.Add(new InvalidSchemaObject
                {
                    Name = triggerName,
                    Type = "trigger",
                    Sql = sql,
                    MissingTable = tableName,
                    IsSafeToDrop = isSafe,
                    SafetyReason = reason
                });
            }
        }

        return invalidTriggers;
    }

    private static IReadOnlyList<InvalidSchemaObject> FindInvalidViews(SqliteConnection connection)
    {
        var invalidViews = new List<InvalidSchemaObject>();
        var existingTables = GetExistingTables(connection);

        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT name, sql
            FROM sqlite_master
            WHERE type = 'view'
            AND sql IS NOT NULL;";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var viewName = reader.GetString("name");
            var sql = reader.GetString("sql");

            var missingTable = FindMissingTableInView(sql, existingTables);
            if (!string.IsNullOrEmpty(missingTable))
            {
                var (isSafe, reason) = ClassifySafety(viewName, "view", missingTable, sql);
                invalidViews.Add(new InvalidSchemaObject
                {
                    Name = viewName,
                    Type = "view",
                    Sql = sql,
                    MissingTable = missingTable,
                    IsSafeToDrop = isSafe,
                    SafetyReason = reason
                });
            }
        }

        return invalidViews;
    }

    private static string? FindMissingTableInView(string viewSql, IReadOnlyList<string> existingTables)
    {
        var upperSql = viewSql.ToUpperInvariant();

        foreach (var table in existingTables)
        {
            upperSql = upperSql.Replace(table.ToUpperInvariant(), string.Empty);
        }

        var matches = System.Text.RegularExpressions.Regex.Matches(upperSql, @"\bFROM\s+([A-Z_][A-Z0-9_]*)\b|\bJOIN\s+([A-Z_][A-Z0-9_]*)\b");

        foreach (System.Text.RegularExpressions.Match match in matches)
        {
            var tableRef = match.Groups[1].Success ? match.Groups[1].Value : match.Groups[2].Value;
            if (!string.IsNullOrWhiteSpace(tableRef))
            {
                return tableRef;
            }
        }

        return null;
    }

    private static (bool IsSafe, string Reason) ClassifySafety(string objectName, string objectType, string missingTable, string sql)
    {
        var upperName = objectName.ToUpperInvariant();
        var upperMissingTable = missingTable.ToUpperInvariant();

        if (objectType == "index")
        {
            if (upperMissingTable.StartsWith("ANIDB_") || upperMissingTable.StartsWith("TMDB_"))
            {
                return (true, $"Index '{objectName}' references missing metadata table '{missingTable}' - safe to drop as it can be recreated");
            }

            if (upperName.Contains("_EPI_", StringComparison.OrdinalIgnoreCase) ||
                upperName.Contains("_CHAP_", StringComparison.OrdinalIgnoreCase) ||
                upperName.Contains("_REL_", StringComparison.OrdinalIgnoreCase))
            {
                return (true, $"Index '{objectName}' references missing table '{missingTable}' - appears to be a stale index, safe to drop");
            }
        }

        if (objectType == "trigger")
        {
            if (upperMissingTable.StartsWith("ANIDB_") || upperMissingTable.StartsWith("TMDB_"))
            {
                return (true, $"Trigger '{objectName}' references missing metadata table '{missingTable}' - safe to drop as metadata triggers are non-critical");
            }

            if (upperName.Contains("UPDATE_", StringComparison.OrdinalIgnoreCase) ||
                upperName.Contains("INSERT_", StringComparison.OrdinalIgnoreCase) ||
                upperName.Contains("DELETE_", StringComparison.OrdinalIgnoreCase))
            {
                return (true, $"Trigger '{objectName}' references missing table '{missingTable}' - appears to be a stale DML trigger, safe to drop");
            }
        }

        if (objectType == "view")
        {
            if (upperName.Contains("_VIEW", StringComparison.OrdinalIgnoreCase) ||
                upperName.Contains("_V", StringComparison.OrdinalIgnoreCase) || upperName.EndsWith("VIEW"))
            {
                return (true, $"View '{objectName}' references missing table '{missingTable}' - appears to be a computed view, safe to drop");
            }

            if (upperMissingTable.StartsWith("ANIDB_") || upperMissingTable.StartsWith("TMDB_"))
            {
                return (true, $"View '{objectName}' references missing metadata table '{missingTable}' - safe to drop as it can be recreated");
            }
        }

        return (false, $"Object '{objectName}' of type '{objectType}' references missing table '{missingTable}' - safety classification unknown, requires manual review");
    }

    private static string? GetDatabaseVersion(SqliteConnection connection)
    {
        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT VersionValue, VersionRevision
                FROM Versions
                WHERE VersionType = 'ShokoServer'
                ORDER BY CAST(VersionValue AS INTEGER) DESC, CAST(VersionRevision AS INTEGER) DESC
                LIMIT 1;";

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                var versionValue = reader.IsDBNull(0) ? "?" : reader.GetString(0);
                var versionRevision = reader.IsDBNull(1) ? "?" : reader.GetString(1);
                return $"{versionValue}.{versionRevision}";
            }
        }
        catch
        {
            return null;
        }

        return null;
    }

    private static IReadOnlyList<string> GetExistingTables(SqliteConnection connection)
    {
        var tables = new List<string>();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT name
            FROM sqlite_master
            WHERE type = 'table'
            AND name NOT LIKE 'sqlite_%'
            ORDER BY name;";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            tables.Add(reader.GetString("name"));
        }

        return tables;
    }

    private static int GetTotalSchemaObjects(SqliteConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT COUNT(*)
            FROM sqlite_master
            WHERE type IN ('table', 'index', 'trigger', 'view')
            AND name NOT LIKE 'sqlite_%';";

        var result = command.ExecuteScalar();
        return result is int count ? count : 0;
    }

    private static MalformedIndexInvestigation InvestigateMalformedIndex(SqliteConnection connection, string indexName)
    {
        string? indexSql = null;
        bool indexExists = false;
        string? tableName = null;
        bool tableExists = false;

        using (var command = connection.CreateCommand())
        {
            command.CommandText = @"
                SELECT sql, tbl_name
                FROM sqlite_master
                WHERE type = 'index'
                AND name = @indexName;";

            command.Parameters.AddWithValue("@indexName", indexName);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                indexExists = true;
                indexSql = reader.IsDBNull("sql") ? null : reader.GetString("sql");
                tableName = reader.GetString("tbl_name");
            }
        }

        if (!string.IsNullOrEmpty(tableName))
        {
            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT COUNT(*)
                FROM sqlite_master
                WHERE type = 'table'
                AND name = @tableName;";

            command.Parameters.AddWithValue("@tableName", tableName);

            var result = command.ExecuteScalar();
            tableExists = result is long count && count > 0;
        }

        var relatedTriggers = new List<string>();
        var relatedViews = new List<string>();

        if (!string.IsNullOrEmpty(tableName))
        {
            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT name, type
                FROM sqlite_master
                WHERE type IN ('trigger', 'view')
                AND (sql LIKE @tableNamePattern1 OR sql LIKE @tableNamePattern2);";

            command.Parameters.AddWithValue("@tableNamePattern1", $"%{tableName}%");
            command.Parameters.AddWithValue("@tableNamePattern2", $"%\"{tableName}\"%");

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var type = reader.GetString("type");
                var name = reader.GetString("name");
                if (type == "trigger")
                {
                    relatedTriggers.Add(name);
                }
                else if (type == "view")
                {
                    relatedViews.Add(name);
                }
            }
        }

        var classification = ClassifyCorruption(indexName, indexExists, tableExists, tableName, indexSql);
        var proposedRepair = classification.RepairIsSafe ? $"DROP INDEX IF EXISTS {indexName};" : null;

        return new MalformedIndexInvestigation
        {
            IndexName = indexName,
            IndexExists = indexExists,
            IndexSql = indexSql,
            TableExists = tableExists,
            TableName = tableName,
            RelatedTriggers = relatedTriggers,
            RelatedViews = relatedViews,
            CorruptionClassification = classification.Classification,
            ProposedRepairStatement = proposedRepair,
            RepairIsSafe = classification.RepairIsSafe
        };
    }

    private static (string Classification, bool RepairIsSafe) ClassifyCorruption(
        string indexName,
        bool indexExists,
        bool tableExists,
        string? tableName,
        string? indexSql)
    {
        if (!indexExists)
        {
            return ("Index does not exist - no corruption detected", false);
        }

        if (tableExists)
        {
            return ("Index and table both exist - may be transient query error, not corruption", false);
        }

        if (string.IsNullOrEmpty(tableName))
        {
            return ("Index exists but has no associated table name - severe corruption", false);
        }

        var upperTableName = tableName.ToUpperInvariant();

        if (upperTableName.StartsWith("ANIDB_") || upperTableName.StartsWith("TMDB_"))
        {
            if (indexName.StartsWith("IX_", StringComparison.OrdinalIgnoreCase))
            {
                return ($"Stale orphaned index '{indexName}' referencing missing metadata table '{tableName}' - safe to drop", true);
            }

            return ($"Index '{indexName}' references missing metadata table '{tableName}' - requires manual review", false);
        }

        if (indexName.Contains("_EPI_", StringComparison.OrdinalIgnoreCase) ||
            indexName.Contains("_CHAP_", StringComparison.OrdinalIgnoreCase) ||
            indexName.Contains("_REL_", StringComparison.OrdinalIgnoreCase))
        {
            return ($"Stale orphaned index '{indexName}' referencing missing table '{tableName}' - appears to be from interrupted upgrade, safe to drop", true);
        }

        return ($"Index '{indexName}' references missing table '{tableName}' - corruption classification unknown, requires manual review", false);
    }
}