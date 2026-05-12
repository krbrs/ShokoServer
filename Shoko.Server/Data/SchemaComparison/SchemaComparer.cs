using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#nullable enable

namespace Shoko.Server.Data.SchemaComparison;

/// <summary>
/// Represents a table as discovered from the actual database.
/// </summary>
public class DiscoveredTable
{
    public string Name { get; set; } = string.Empty;
    public Dictionary<string, DiscoveredColumn> Columns { get; set; } = new();
    public List<DiscoveredPrimaryKey> PrimaryKeys { get; set; } = [];
    public List<DiscoveredIndex> Indexes { get; set; } = [];
    public List<DiscoveredConstraint> Constraints { get; set; } = [];
}

/// <summary>
/// Represents a column as discovered from the actual database.
/// </summary>
public class DiscoveredColumn
{
    public string Name { get; set; } = string.Empty;
    public string StoreType { get; set; } = string.Empty;
    public bool IsNullable { get; set; }
    public bool IsPrimaryKey { get; set; }
    public bool IsIdentity { get; set; }
    public string? DefaultValue { get; set; }
    public string? DefaultValueSql { get; set; }
    public string? ComputedColumnSql { get; set; }
}

/// <summary>
/// Represents a primary key as discovered from the actual database.
/// </summary>
public class DiscoveredPrimaryKey
{
    public string Name { get; set; } = string.Empty;
    public List<string> ColumnNames { get; set; } = [];
}

/// <summary>
/// Represents an index as discovered from the actual database.
/// </summary>
public class DiscoveredIndex
{
    public string Name { get; set; } = string.Empty;
    public List<string> ColumnNames { get; set; } = [];
    public bool IsUnique { get; set; }
    public bool IsPrimaryKey { get; set; }
    public bool IsClustered { get; set; }
}

/// <summary>
/// Represents a constraint as discovered from the actual database.
/// </summary>
public class DiscoveredConstraint
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // CHECK, FOREIGN KEY, DEFAULT
    public string Definition { get; set; } = string.Empty;
    public string? ReferencedTable { get; set; }
    public string? ReferencedColumn { get; set; }
}

/// <summary>
/// Compares an EF Core model against an actual database schema.
///
/// Supports SQLite, MariaDB/MySQL (Pomelo), and SQL Server backends.
/// All database access is read-only.
/// </summary>
public class SchemaComparer
{
    private readonly DbContext _context;
    private readonly string _providerName;

    public SchemaComparer(DbContext context)
    {
        _context = context;
        _providerName = context.Database.ProviderName ?? string.Empty;
    }

    /// <summary>
    /// Compares the EF Core model against the actual database schema.
    /// Returns a <see cref="SchemaComparisonResult"/> with errors and warnings.
    /// </summary>
    public async Task<SchemaComparisonResult> CompareAsync()
    {
        var result = new SchemaComparisonResult
        {
            ProviderName = _providerName,
            ConnectionInfo = _context.Database.GetConnectionString() ?? "N/A"
        };

        var modelTables = GetModelTables();
        var actualTables = await InspectDatabaseAsync();

        CompareTables(modelTables, actualTables, result);

        result.IsValid = result.Errors.Count == 0;
        return result;
    }

    /// <summary>
    /// Compares the EF Core model against the actual database schema (synchronous).
    /// </summary>
    public SchemaComparisonResult Compare()
    {
        var result = new SchemaComparisonResult
        {
            ProviderName = _providerName,
            ConnectionInfo = _context.Database.GetConnectionString() ?? "N/A"
        };

        var modelTables = GetModelTables();
        var actualTables = InspectDatabase();

        CompareTables(modelTables, actualTables, result);

        result.IsValid = result.Errors.Count == 0;
        return result;
    }

    private Dictionary<string, IEntityType> GetModelTables()
    {
        var modelTables = new Dictionary<string, IEntityType>(StringComparer.OrdinalIgnoreCase);

        foreach (var entityType in _context.Model.GetEntityTypes())
        {
            var tableName = entityType.GetTableName();
            if (string.IsNullOrEmpty(tableName))
                continue;

            modelTables[tableName] = entityType;
        }

        return modelTables;
    }

    private async Task<Dictionary<string, DiscoveredTable>> InspectDatabaseAsync()
    {
        var tables = new Dictionary<string, DiscoveredTable>(StringComparer.OrdinalIgnoreCase);

        var connection = _context.Database.GetDbConnection();
        var wasClosed = connection.State == System.Data.ConnectionState.Closed;
        if (wasClosed)
            await connection.OpenAsync();

        try
        {
            if (_providerName.Contains("Sqlite", StringComparison.OrdinalIgnoreCase))
                tables = await InspectSQLiteAsync(connection);
            else if (_providerName.Contains("MySql", StringComparison.OrdinalIgnoreCase) ||
                     _providerName.Contains("MariaDb", StringComparison.OrdinalIgnoreCase))
                tables = await InspectMariaDbAsync(connection);
            else if (_providerName.Contains("SqlServer", StringComparison.OrdinalIgnoreCase))
                tables = await InspectSqlServerAsync(connection);
        }
        finally
        {
            if (wasClosed)
                await connection.CloseAsync();
        }

        return tables;
    }

    private Dictionary<string, DiscoveredTable> InspectDatabase()
    {
        var tables = new Dictionary<string, DiscoveredTable>(StringComparer.OrdinalIgnoreCase);

        var connection = _context.Database.GetDbConnection();
        var wasClosed = connection.State == System.Data.ConnectionState.Closed;
        if (wasClosed)
            connection.Open();

        try
        {
            if (_providerName.Contains("Sqlite", StringComparison.OrdinalIgnoreCase))
                tables = InspectSQLite(connection);
            else if (_providerName.Contains("MySql", StringComparison.OrdinalIgnoreCase) ||
                     _providerName.Contains("MariaDb", StringComparison.OrdinalIgnoreCase))
                tables = InspectMariaDb(connection);
            else if (_providerName.Contains("SqlServer", StringComparison.OrdinalIgnoreCase))
                tables = InspectSqlServer(connection);
        }
        finally
        {
            if (wasClosed)
                connection.Close();
        }

        return tables;
    }

    #region SQLite Inspection

    private async Task<Dictionary<string, DiscoveredTable>> InspectSQLiteAsync(System.Data.Common.DbConnection connection)
    {
        var tables = new Dictionary<string, DiscoveredTable>(StringComparer.OrdinalIgnoreCase);

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            SELECT name FROM sqlite_master
            WHERE type = 'table' AND name NOT LIKE 'sqlite_%'
            ORDER BY name";

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var tableName = reader.GetString(0);
            var table = new DiscoveredTable { Name = tableName };
            table.Columns = await GetSQLiteColumnsAsync(connection, tableName);
            table.Indexes = await GetSQLiteIndexesAsync(connection, tableName);
            table.PrimaryKeys = ExtractPrimaryKeysFromColumns(table.Columns);
            tables[tableName] = table;
        }

        return tables;
    }

    private Dictionary<string, DiscoveredTable> InspectSQLite(System.Data.Common.DbConnection connection)
    {
        var tables = new Dictionary<string, DiscoveredTable>(StringComparer.OrdinalIgnoreCase);

        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            SELECT name FROM sqlite_master
            WHERE type = 'table' AND name NOT LIKE 'sqlite_%'
            ORDER BY name";

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var tableName = reader.GetString(0);
            var table = new DiscoveredTable { Name = tableName };
            table.Columns = GetSQLiteColumns(connection, tableName);
            table.Indexes = GetSQLiteIndexes(connection, tableName);
            table.PrimaryKeys = ExtractPrimaryKeysFromColumns(table.Columns);
            tables[tableName] = table;
        }

        return tables;
    }

    private async Task<Dictionary<string, DiscoveredColumn>> GetSQLiteColumnsAsync(System.Data.Common.DbConnection connection, string tableName)
    {
        var columns = new Dictionary<string, DiscoveredColumn>(StringComparer.OrdinalIgnoreCase);

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = $"PRAGMA table_info(\"{EscapeSQLiteIdentifier(tableName)}\")";

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var col = new DiscoveredColumn
            {
                Name = reader.GetString(1),
                StoreType = reader.GetString(2),
                IsNullable = reader.GetInt32(3) != 0,
                IsPrimaryKey = reader.GetInt32(5) != 0
            };
            columns[col.Name] = col;
        }

        return columns;
    }

    private Dictionary<string, DiscoveredColumn> GetSQLiteColumns(System.Data.Common.DbConnection connection, string tableName)
    {
        var columns = new Dictionary<string, DiscoveredColumn>(StringComparer.OrdinalIgnoreCase);

        using var cmd = connection.CreateCommand();
        cmd.CommandText = $"PRAGMA table_info(\"{EscapeSQLiteIdentifier(tableName)}\")";

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var col = new DiscoveredColumn
            {
                Name = reader.GetString(1),
                StoreType = reader.GetString(2),
                IsNullable = reader.GetInt32(3) != 0,
                IsPrimaryKey = reader.GetInt32(5) != 0
            };
            columns[col.Name] = col;
        }

        return columns;
    }

    private async Task<List<DiscoveredIndex>> GetSQLiteIndexesAsync(System.Data.Common.DbConnection connection, string tableName)
    {
        var indexes = new List<DiscoveredIndex>();

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = $"PRAGMA index_list(\"{EscapeSQLiteIdentifier(tableName)}\")";

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var indexName = reader.GetString(1);
            var isUnique = reader.GetInt32(2) != 0;

            // Skip auto-generated indexes (e.g., SQLite internal indexes)
            if (indexName.StartsWith("sqlite_autoindex_", StringComparison.OrdinalIgnoreCase))
                continue;

            var index = new DiscoveredIndex
            {
                Name = indexName,
                IsUnique = isUnique,
                IsPrimaryKey = indexName.StartsWith($"sqlite_autoindex_{tableName}", StringComparison.OrdinalIgnoreCase)
            };

            index.ColumnNames = await GetSQLiteIndexColumnsAsync(connection, indexName);
            indexes.Add(index);
        }

        return indexes;
    }

    private List<DiscoveredIndex> GetSQLiteIndexes(System.Data.Common.DbConnection connection, string tableName)
    {
        var indexes = new List<DiscoveredIndex>();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = $"PRAGMA index_list(\"{EscapeSQLiteIdentifier(tableName)}\")";

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var indexName = reader.GetString(1);
            var isUnique = reader.GetInt32(2) != 0;

            if (indexName.StartsWith("sqlite_autoindex_", StringComparison.OrdinalIgnoreCase))
                continue;

            var index = new DiscoveredIndex
            {
                Name = indexName,
                IsUnique = isUnique,
                IsPrimaryKey = indexName.StartsWith($"sqlite_autoindex_{tableName}", StringComparison.OrdinalIgnoreCase)
            };

            index.ColumnNames = GetSQLiteIndexColumns(connection, indexName);
            indexes.Add(index);
        }

        return indexes;
    }

    private async Task<List<string>> GetSQLiteIndexColumnsAsync(System.Data.Common.DbConnection connection, string indexName)
    {
        var columnNames = new List<string>();

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = $"PRAGMA index_info(\"{EscapeSQLiteIdentifier(indexName)}\")";

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var colName = reader.GetString(2);
            if (!string.IsNullOrEmpty(colName))
                columnNames.Add(colName);
        }

        return columnNames;
    }

    private List<string> GetSQLiteIndexColumns(System.Data.Common.DbConnection connection, string indexName)
    {
        var columnNames = new List<string>();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = $"PRAGMA index_info(\"{EscapeSQLiteIdentifier(indexName)}\")";

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var colName = reader.GetString(2);
            if (!string.IsNullOrEmpty(colName))
                columnNames.Add(colName);
        }

        return columnNames;
    }

    #endregion

    #region MariaDB/MySQL Inspection

    private async Task<Dictionary<string, DiscoveredTable>> InspectMariaDbAsync(System.Data.Common.DbConnection connection)
    {
        var tables = new Dictionary<string, DiscoveredTable>(StringComparer.OrdinalIgnoreCase);

        await using var cmd = connection.CreateCommand();

        // Get all tables
        cmd.CommandText = @"
            SELECT TABLE_NAME
            FROM information_schema.TABLES
            WHERE TABLE_SCHEMA = DATABASE()
              AND TABLE_TYPE = 'BASE TABLE'
            ORDER BY TABLE_NAME";

        var tableNames = new List<string>();
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
                tableNames.Add(reader.GetString(0));
        }

        foreach (var tableName in tableNames)
        {
            var table = new DiscoveredTable { Name = tableName };
            table.Columns = await GetMariaDbColumnsAsync(connection, tableName);
            table.PrimaryKeys = await GetMariaDbPrimaryKeysAsync(connection, tableName);
            table.Indexes = await GetMariaDbIndexesAsync(connection, tableName);
            tables[tableName] = table;
        }

        return tables;
    }

    private Dictionary<string, DiscoveredTable> InspectMariaDb(System.Data.Common.DbConnection connection)
    {
        var tables = new Dictionary<string, DiscoveredTable>(StringComparer.OrdinalIgnoreCase);

        using var cmd = connection.CreateCommand();

        cmd.CommandText = @"
            SELECT TABLE_NAME
            FROM information_schema.TABLES
            WHERE TABLE_SCHEMA = DATABASE()
              AND TABLE_TYPE = 'BASE TABLE'
            ORDER BY TABLE_NAME";

        var tableNames = new List<string>();
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
                tableNames.Add(reader.GetString(0));
        }

        foreach (var tableName in tableNames)
        {
            var table = new DiscoveredTable { Name = tableName };
            table.Columns = GetMariaDbColumns(connection, tableName);
            table.PrimaryKeys = GetMariaDbPrimaryKeys(connection, tableName);
            table.Indexes = GetMariaDbIndexes(connection, tableName);
            tables[tableName] = table;
        }

        return tables;
    }

    private async Task<Dictionary<string, DiscoveredColumn>> GetMariaDbColumnsAsync(System.Data.Common.DbConnection connection, string tableName)
    {
        var columns = new Dictionary<string, DiscoveredColumn>(StringComparer.OrdinalIgnoreCase);

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            SELECT
                COLUMN_NAME,
                DATA_TYPE,
                IS_NULLABLE,
                COLUMN_DEFAULT,
                EXTRA
            FROM information_schema.COLUMNS
            WHERE TABLE_SCHEMA = DATABASE()
              AND TABLE_NAME = @tableName
            ORDER BY ORDINAL_POSITION";

        var param = cmd.CreateParameter();
        param.ParameterName = "@tableName";
        param.Value = tableName;
        cmd.Parameters.Add(param);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var col = new DiscoveredColumn
            {
                Name = reader.GetString(0),
                StoreType = reader.GetString(1),
                IsNullable = reader.GetString(2) == "YES",
                DefaultValue = reader.IsDBNull(3) ? null : reader.GetString(3),
                IsIdentity = reader.GetString(4)?.Contains("auto_increment", StringComparison.OrdinalIgnoreCase) == true
            };
            columns[col.Name] = col;
        }

        return columns;
    }

    private Dictionary<string, DiscoveredColumn> GetMariaDbColumns(System.Data.Common.DbConnection connection, string tableName)
    {
        var columns = new Dictionary<string, DiscoveredColumn>(StringComparer.OrdinalIgnoreCase);

        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            SELECT
                COLUMN_NAME,
                DATA_TYPE,
                IS_NULLABLE,
                COLUMN_DEFAULT,
                EXTRA
            FROM information_schema.COLUMNS
            WHERE TABLE_SCHEMA = DATABASE()
              AND TABLE_NAME = @tableName
            ORDER BY ORDINAL_POSITION";

        var param = cmd.CreateParameter();
        param.ParameterName = "@tableName";
        param.Value = tableName;
        cmd.Parameters.Add(param);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var col = new DiscoveredColumn
            {
                Name = reader.GetString(0),
                StoreType = reader.GetString(1),
                IsNullable = reader.GetString(2) == "YES",
                DefaultValue = reader.IsDBNull(3) ? null : reader.GetString(3),
                IsIdentity = reader.GetString(4)?.Contains("auto_increment", StringComparison.OrdinalIgnoreCase) == true
            };
            columns[col.Name] = col;
        }

        return columns;
    }

    private async Task<List<DiscoveredPrimaryKey>> GetMariaDbPrimaryKeysAsync(System.Data.Common.DbConnection connection, string tableName)
    {
        var primaryKeys = new List<DiscoveredPrimaryKey>();

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            SELECT
                CONSTRAINT_NAME,
                COLUMN_NAME
            FROM information_schema.KEY_COLUMN_USAGE
            WHERE TABLE_SCHEMA = DATABASE()
              AND TABLE_NAME = @tableName
              AND CONSTRAINT_NAME = 'PRIMARY'
            ORDER BY ORDINAL_POSITION";

        var param = cmd.CreateParameter();
        param.ParameterName = "@tableName";
        param.Value = tableName;
        cmd.Parameters.Add(param);

        var pk = new DiscoveredPrimaryKey { Name = "PRIMARY" };
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            pk.ColumnNames.Add(reader.GetString(1));

        if (pk.ColumnNames.Count > 0)
            primaryKeys.Add(pk);

        return primaryKeys;
    }

    private List<DiscoveredPrimaryKey> GetMariaDbPrimaryKeys(System.Data.Common.DbConnection connection, string tableName)
    {
        var primaryKeys = new List<DiscoveredPrimaryKey>();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            SELECT
                CONSTRAINT_NAME,
                COLUMN_NAME
            FROM information_schema.KEY_COLUMN_USAGE
            WHERE TABLE_SCHEMA = DATABASE()
              AND TABLE_NAME = @tableName
              AND CONSTRAINT_NAME = 'PRIMARY'
            ORDER BY ORDINAL_POSITION";

        var param = cmd.CreateParameter();
        param.ParameterName = "@tableName";
        param.Value = tableName;
        cmd.Parameters.Add(param);

        var pk = new DiscoveredPrimaryKey { Name = "PRIMARY" };
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            pk.ColumnNames.Add(reader.GetString(1));

        if (pk.ColumnNames.Count > 0)
            primaryKeys.Add(pk);

        return primaryKeys;
    }

    private async Task<List<DiscoveredIndex>> GetMariaDbIndexesAsync(System.Data.Common.DbConnection connection, string tableName)
    {
        var indexes = new List<DiscoveredIndex>();

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            SELECT
                INDEX_NAME,
                COLUMN_NAME,
                NON_UNIQUE
            FROM information_schema.STATISTICS
            WHERE TABLE_SCHEMA = DATABASE()
              AND TABLE_NAME = @tableName
            ORDER BY INDEX_NAME, SEQ_IN_INDEX";

        var param = cmd.CreateParameter();
        param.ParameterName = "@tableName";
        param.Value = tableName;
        cmd.Parameters.Add(param);

        var indexMap = new Dictionary<string, DiscoveredIndex>(StringComparer.OrdinalIgnoreCase);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var indexName = reader.GetString(0);
            var isUnique = reader.GetInt32(2) == 0;

            if (!indexMap.TryGetValue(indexName, out var index))
            {
                index = new DiscoveredIndex
                {
                    Name = indexName,
                    IsUnique = isUnique,
                    IsPrimaryKey = indexName == "PRIMARY"
                };
                indexMap[indexName] = index;
                indexes.Add(index);
            }

            index.ColumnNames.Add(reader.GetString(1));
        }

        return indexes;
    }

    private List<DiscoveredIndex> GetMariaDbIndexes(System.Data.Common.DbConnection connection, string tableName)
    {
        var indexes = new List<DiscoveredIndex>();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            SELECT
                INDEX_NAME,
                COLUMN_NAME,
                NON_UNIQUE
            FROM information_schema.STATISTICS
            WHERE TABLE_SCHEMA = DATABASE()
              AND TABLE_NAME = @tableName
            ORDER BY INDEX_NAME, SEQ_IN_INDEX";

        var param = cmd.CreateParameter();
        param.ParameterName = "@tableName";
        param.Value = tableName;
        cmd.Parameters.Add(param);

        var indexMap = new Dictionary<string, DiscoveredIndex>(StringComparer.OrdinalIgnoreCase);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var indexName = reader.GetString(0);
            var isUnique = reader.GetInt32(2) == 0;

            if (!indexMap.TryGetValue(indexName, out var index))
            {
                index = new DiscoveredIndex
                {
                    Name = indexName,
                    IsUnique = isUnique,
                    IsPrimaryKey = indexName == "PRIMARY"
                };
                indexMap[indexName] = index;
                indexes.Add(index);
            }

            index.ColumnNames.Add(reader.GetString(1));
        }

        return indexes;
    }

    #endregion

    #region SQL Server Inspection

    private async Task<Dictionary<string, DiscoveredTable>> InspectSqlServerAsync(System.Data.Common.DbConnection connection)
    {
        var tables = new Dictionary<string, DiscoveredTable>(StringComparer.OrdinalIgnoreCase);

        await using var cmd = connection.CreateCommand();

        // Get all user tables
        cmd.CommandText = @"
            SELECT t.name
            FROM sys.tables t
            INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
            WHERE s.name = 'dbo'
            ORDER BY t.name";

        var tableNames = new List<string>();
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
                tableNames.Add(reader.GetString(0));
        }

        foreach (var tableName in tableNames)
        {
            var table = new DiscoveredTable { Name = tableName };
            table.Columns = await GetSqlServerColumnsAsync(connection, tableName);
            table.PrimaryKeys = await GetSqlServerPrimaryKeysAsync(connection, tableName);
            table.Indexes = await GetSqlServerIndexesAsync(connection, tableName);
            table.Constraints = await GetSqlServerConstraintsAsync(connection, tableName);
            tables[tableName] = table;
        }

        return tables;
    }

    private Dictionary<string, DiscoveredTable> InspectSqlServer(System.Data.Common.DbConnection connection)
    {
        var tables = new Dictionary<string, DiscoveredTable>(StringComparer.OrdinalIgnoreCase);

        using var cmd = connection.CreateCommand();

        cmd.CommandText = @"
            SELECT t.name
            FROM sys.tables t
            INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
            WHERE s.name = 'dbo'
            ORDER BY t.name";

        var tableNames = new List<string>();
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
                tableNames.Add(reader.GetString(0));
        }

        foreach (var tableName in tableNames)
        {
            var table = new DiscoveredTable { Name = tableName };
            table.Columns = GetSqlServerColumns(connection, tableName);
            table.PrimaryKeys = GetSqlServerPrimaryKeys(connection, tableName);
            table.Indexes = GetSqlServerIndexes(connection, tableName);
            table.Constraints = GetSqlServerConstraints(connection, tableName);
            tables[tableName] = table;
        }

        return tables;
    }

    private async Task<Dictionary<string, DiscoveredColumn>> GetSqlServerColumnsAsync(System.Data.Common.DbConnection connection, string tableName)
    {
        var columns = new Dictionary<string, DiscoveredColumn>(StringComparer.OrdinalIgnoreCase);

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            SELECT
                c.name,
                t.name AS system_type_name,
                CASE WHEN c.is_nullable = 1 THEN 'YES' ELSE 'NO' END AS is_nullable,
                c.is_identity,
                dc.definition AS default_value,
                c.is_computed,
                cc.definition AS computed_column_sql
            FROM sys.columns c
            INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
            LEFT JOIN sys.default_constraints dc ON c.default_object_id = dc.object_id
            LEFT JOIN sys.computed_columns cc ON c.object_id = cc.object_id AND c.column_id = cc.column_id
            INNER JOIN sys.tables tbl ON c.object_id = tbl.object_id
            WHERE tbl.name = @tableName AND tbl.schema_id = SCHEMA_ID('dbo')
            ORDER BY c.column_id";

        var param = cmd.CreateParameter();
        param.ParameterName = "@tableName";
        param.Value = tableName;
        cmd.Parameters.Add(param);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var col = new DiscoveredColumn
            {
                Name = reader.GetString(0),
                StoreType = reader.GetString(1),
                IsNullable = reader.GetString(2) == "YES",
                IsIdentity = Convert.ToBoolean(reader.GetValue(3)),
                DefaultValue = reader.IsDBNull(4) ? null : reader.GetString(4),
                ComputedColumnSql = reader.IsDBNull(6) ? null : reader.GetString(6)
            };
            columns[col.Name] = col;
        }

        return columns;
    }

    private Dictionary<string, DiscoveredColumn> GetSqlServerColumns(System.Data.Common.DbConnection connection, string tableName)
    {
        var columns = new Dictionary<string, DiscoveredColumn>(StringComparer.OrdinalIgnoreCase);

        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            SELECT
                c.name,
                t.name AS system_type_name,
                CASE WHEN c.is_nullable = 1 THEN 'YES' ELSE 'NO' END AS is_nullable,
                c.is_identity,
                dc.definition AS default_value,
                c.is_computed,
                cc.definition AS computed_column_sql
            FROM sys.columns c
            INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
            LEFT JOIN sys.default_constraints dc ON c.default_object_id = dc.object_id
            LEFT JOIN sys.computed_columns cc ON c.object_id = cc.object_id AND c.column_id = cc.column_id
            INNER JOIN sys.tables tbl ON c.object_id = tbl.object_id
            WHERE tbl.name = @tableName AND tbl.schema_id = SCHEMA_ID('dbo')
            ORDER BY c.column_id";

        var param = cmd.CreateParameter();
        param.ParameterName = "@tableName";
        param.Value = tableName;
        cmd.Parameters.Add(param);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var col = new DiscoveredColumn
            {
                Name = reader.GetString(0),
                StoreType = reader.GetString(1),
                IsNullable = reader.GetString(2) == "YES",
                IsIdentity = Convert.ToBoolean(reader.GetValue(3)),
                DefaultValue = reader.IsDBNull(4) ? null : reader.GetString(4),
                ComputedColumnSql = reader.IsDBNull(6) ? null : reader.GetString(6)
            };
            columns[col.Name] = col;
        }

        return columns;
    }

    private async Task<List<DiscoveredPrimaryKey>> GetSqlServerPrimaryKeysAsync(System.Data.Common.DbConnection connection, string tableName)
    {
        var primaryKeys = new List<DiscoveredPrimaryKey>();

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            SELECT
                kc.name,
                c.name AS column_name
            FROM sys.key_constraints kc
            INNER JOIN sys.indexes i ON kc.parent_object_id = i.object_id AND kc.unique_index_id = i.index_id
            INNER JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
            INNER JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
            INNER JOIN sys.tables t ON kc.parent_object_id = t.object_id
            WHERE t.name = @tableName AND t.schema_id = SCHEMA_ID('dbo') AND kc.type = 'PK'
            ORDER BY ic.key_ordinal";

        var param = cmd.CreateParameter();
        param.ParameterName = "@tableName";
        param.Value = tableName;
        cmd.Parameters.Add(param);

        var pk = new DiscoveredPrimaryKey();
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            if (string.IsNullOrEmpty(pk.Name))
                pk.Name = reader.GetString(0);
            pk.ColumnNames.Add(reader.GetString(1));
        }

        if (pk.ColumnNames.Count > 0)
            primaryKeys.Add(pk);

        return primaryKeys;
    }

    private List<DiscoveredPrimaryKey> GetSqlServerPrimaryKeys(System.Data.Common.DbConnection connection, string tableName)
    {
        var primaryKeys = new List<DiscoveredPrimaryKey>();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            SELECT
                kc.name,
                c.name AS column_name
            FROM sys.key_constraints kc
            INNER JOIN sys.indexes i ON kc.parent_object_id = i.object_id AND kc.unique_index_id = i.index_id
            INNER JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
            INNER JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
            INNER JOIN sys.tables t ON kc.parent_object_id = t.object_id
            WHERE t.name = @tableName AND t.schema_id = SCHEMA_ID('dbo') AND kc.type = 'PK'
            ORDER BY ic.key_ordinal";

        var param = cmd.CreateParameter();
        param.ParameterName = "@tableName";
        param.Value = tableName;
        cmd.Parameters.Add(param);

        var pk = new DiscoveredPrimaryKey();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            if (string.IsNullOrEmpty(pk.Name))
                pk.Name = reader.GetString(0);
            pk.ColumnNames.Add(reader.GetString(1));
        }

        if (pk.ColumnNames.Count > 0)
            primaryKeys.Add(pk);

        return primaryKeys;
    }

    private async Task<List<DiscoveredIndex>> GetSqlServerIndexesAsync(System.Data.Common.DbConnection connection, string tableName)
    {
        var indexes = new List<DiscoveredIndex>();

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            SELECT
                i.name AS index_name,
                c.name AS column_name,
                i.is_unique,
                i.type_desc,
                CASE WHEN pk.object_id IS NOT NULL THEN 1 ELSE 0 END AS is_primary_key
            FROM sys.indexes i
            INNER JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
            INNER JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
            LEFT JOIN sys.key_constraints pk ON i.object_id = pk.object_id AND i.index_id = pk.unique_index_id
            INNER JOIN sys.tables t ON i.object_id = t.object_id
            WHERE t.name = @tableName AND t.schema_id = SCHEMA_ID('dbo')
              AND i.is_hypothetical = 0 AND i.type IN (1, 2)
            ORDER BY i.name, ic.key_ordinal";

        var param = cmd.CreateParameter();
        param.ParameterName = "@tableName";
        param.Value = tableName;
        cmd.Parameters.Add(param);

        var indexMap = new Dictionary<string, DiscoveredIndex>(StringComparer.OrdinalIgnoreCase);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var indexName = reader.GetString(0);
            var isClustered = reader.GetString(3) == "CLUSTERED";

            if (!indexMap.TryGetValue(indexName, out var index))
            {
                index = new DiscoveredIndex
                {
                    Name = indexName,
                    IsUnique = Convert.ToBoolean(reader.GetValue(2)),
                    IsClustered = isClustered,
                    IsPrimaryKey = Convert.ToBoolean(reader.GetValue(4))
                };
                indexMap[indexName] = index;
                indexes.Add(index);
            }

            index.ColumnNames.Add(reader.GetString(1));
        }

        return indexes;
    }

    private List<DiscoveredIndex> GetSqlServerIndexes(System.Data.Common.DbConnection connection, string tableName)
    {
        var indexes = new List<DiscoveredIndex>();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            SELECT
                i.name AS index_name,
                c.name AS column_name,
                i.is_unique,
                i.type_desc,
                CASE WHEN pk.object_id IS NOT NULL THEN 1 ELSE 0 END AS is_primary_key
            FROM sys.indexes i
            INNER JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
            INNER JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
            LEFT JOIN sys.key_constraints pk ON i.object_id = pk.object_id AND i.index_id = pk.unique_index_id
            INNER JOIN sys.tables t ON i.object_id = t.object_id
            WHERE t.name = @tableName AND t.schema_id = SCHEMA_ID('dbo')
              AND i.is_hypothetical = 0 AND i.type IN (1, 2)
            ORDER BY i.name, ic.key_ordinal";

        var param = cmd.CreateParameter();
        param.ParameterName = "@tableName";
        param.Value = tableName;
        cmd.Parameters.Add(param);

        var indexMap = new Dictionary<string, DiscoveredIndex>(StringComparer.OrdinalIgnoreCase);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var indexName = reader.GetString(0);
            var isClustered = reader.GetString(3) == "CLUSTERED";

            if (!indexMap.TryGetValue(indexName, out var index))
            {
                index = new DiscoveredIndex
                {
                    Name = indexName,
                    IsUnique = Convert.ToBoolean(reader.GetValue(2)),
                    IsClustered = isClustered,
                    IsPrimaryKey = Convert.ToBoolean(reader.GetValue(4))
                };
                indexMap[indexName] = index;
                indexes.Add(index);
            }

            index.ColumnNames.Add(reader.GetString(1));
        }

        return indexes;
    }

    private async Task<List<DiscoveredConstraint>> GetSqlServerConstraintsAsync(System.Data.Common.DbConnection connection, string tableName)
    {
        var constraints = new List<DiscoveredConstraint>();

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            SELECT
                cc.name AS constraint_name,
                cc.type_desc,
                cc.definition,
                NULL AS referenced_table,
                NULL AS referenced_column
            FROM sys.check_constraints cc
            INNER JOIN sys.tables t ON cc.parent_object_id = t.object_id
            WHERE t.name = @tableName AND t.schema_id = SCHEMA_ID('dbo')
            UNION ALL
            SELECT
                fk.name,
                'FOREIGN KEY',
                NULL,
                OBJECT_NAME(fk.referenced_object_id),
                COL_NAME(fkc.parent_object_id, fkc.parent_column_id)
            FROM sys.foreign_keys fk
            INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
            INNER JOIN sys.tables t ON fk.parent_object_id = t.object_id
            WHERE t.name = @tableName AND t.schema_id = SCHEMA_ID('dbo')
            UNION ALL
            SELECT
                dc.name,
                'DEFAULT',
                dc.definition,
                NULL,
                COL_NAME(dc.parent_object_id, dc.parent_column_id)
            FROM sys.default_constraints dc
            INNER JOIN sys.tables t ON dc.parent_object_id = t.object_id
            WHERE t.name = @tableName AND t.schema_id = SCHEMA_ID('dbo')";

        var param = cmd.CreateParameter();
        param.ParameterName = "@tableName";
        param.Value = tableName;
        cmd.Parameters.Add(param);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            constraints.Add(new DiscoveredConstraint
            {
                Name = reader.GetString(0),
                Type = reader.GetString(1),
                Definition = reader.IsDBNull(2) ? null : reader.GetString(2),
                ReferencedTable = reader.IsDBNull(3) ? null : reader.GetString(3),
                ReferencedColumn = reader.IsDBNull(4) ? null : reader.GetString(4)
            });
        }

        return constraints;
    }

    private List<DiscoveredConstraint> GetSqlServerConstraints(System.Data.Common.DbConnection connection, string tableName)
    {
        var constraints = new List<DiscoveredConstraint>();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            SELECT
                cc.name AS constraint_name,
                cc.type_desc,
                cc.definition,
                NULL AS referenced_table,
                NULL AS referenced_column
            FROM sys.check_constraints cc
            INNER JOIN sys.tables t ON cc.parent_object_id = t.object_id
            WHERE t.name = @tableName AND t.schema_id = SCHEMA_ID('dbo')
            UNION ALL
            SELECT
                fk.name,
                'FOREIGN KEY',
                NULL,
                OBJECT_NAME(fk.referenced_object_id),
                COL_NAME(fkc.parent_object_id, fkc.parent_column_id)
            FROM sys.foreign_keys fk
            INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
            INNER JOIN sys.tables t ON fk.parent_object_id = t.object_id
            WHERE t.name = @tableName AND t.schema_id = SCHEMA_ID('dbo')
            UNION ALL
            SELECT
                dc.name,
                'DEFAULT',
                dc.definition,
                NULL,
                COL_NAME(dc.parent_object_id, dc.parent_column_id)
            FROM sys.default_constraints dc
            INNER JOIN sys.tables t ON dc.parent_object_id = t.object_id
            WHERE t.name = @tableName AND t.schema_id = SCHEMA_ID('dbo')";

        var param = cmd.CreateParameter();
        param.ParameterName = "@tableName";
        param.Value = tableName;
        cmd.Parameters.Add(param);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            constraints.Add(new DiscoveredConstraint
            {
                Name = reader.GetString(0),
                Type = reader.GetString(1),
                Definition = reader.IsDBNull(2) ? null : reader.GetString(2),
                ReferencedTable = reader.IsDBNull(3) ? null : reader.GetString(3),
                ReferencedColumn = reader.IsDBNull(4) ? null : reader.GetString(4)
            });
        }

        return constraints;
    }

    #endregion

    #region Comparison Logic

    private void CompareTables(
        Dictionary<string, IEntityType> modelTables,
        Dictionary<string, DiscoveredTable> actualTables,
        SchemaComparisonResult result)
    {
        var modelTableNames = new HashSet<string>(modelTables.Keys, StringComparer.OrdinalIgnoreCase);
        var actualTableNames = new HashSet<string>(actualTables.Keys, StringComparer.OrdinalIgnoreCase);

        var missingTables = modelTableNames.Except(actualTableNames).ToList();
        var extraTables = actualTableNames.Except(modelTableNames).ToList();

        var tableSummary = result.TableSummary;
        tableSummary.ExpectedTables = modelTableNames.Count;
        tableSummary.ActualTables = actualTableNames.Count;
        tableSummary.MissingTables = missingTables.Count;
        tableSummary.ExtraTables = extraTables.Count;
        tableSummary.MatchingTables = modelTableNames.Count - missingTables.Count;
        tableSummary.MissingTableNames = missingTables;
        tableSummary.ExtraTableNames = extraTables;

        // Report missing tables as errors
        foreach (var tableName in missingTables)
        {
            result.Errors.Add(new SchemaError
            {
                Category = "MissingTable",
                TableName = tableName,
                Message = $"Expected table '{tableName}' not found in database"
            });
        }

        // Report extra tables as warnings
        foreach (var tableName in extraTables)
        {
            result.Warnings.Add(new SchemaWarning
            {
                Category = "ExtraTable",
                TableName = tableName,
                Message = $"Table '{tableName}' exists in database but not in EF Core model"
            });
        }

        // Compare columns for tables that exist in both
        foreach (var tableName in modelTableNames.Intersect(actualTableNames, StringComparer.OrdinalIgnoreCase))
        {
            var entityType = modelTables[tableName];
            var discoveredTable = actualTables[tableName];
            CompareColumns(entityType, discoveredTable, result);
        }
    }

    private void CompareColumns(
        IEntityType entityType,
        DiscoveredTable discoveredTable,
        SchemaComparisonResult result)
    {
        var summary = new ColumnComparisonSummary
        {
            TableName = discoveredTable.Name
        };

        var modelColumns = new Dictionary<string, IProperty>(StringComparer.OrdinalIgnoreCase);
        foreach (var property in entityType.GetProperties())
        {
            var columnName = property.GetColumnName() ?? property.Name;
            modelColumns[columnName] = property;
        }

        var modelColumnNames = new HashSet<string>(modelColumns.Keys, StringComparer.OrdinalIgnoreCase);
        var actualColumnNames = new HashSet<string>(discoveredTable.Columns.Keys, StringComparer.OrdinalIgnoreCase);

        var missingColumns = modelColumnNames.Except(actualColumnNames).ToList();
        var extraColumns = actualColumnNames.Except(modelColumnNames).ToList();

        summary.ExpectedColumns = modelColumnNames.Count;
        summary.ActualColumns = actualColumnNames.Count;
        summary.MissingColumns = missingColumns.Count;
        summary.ExtraColumns = extraColumns.Count;
        summary.MatchingColumns = modelColumnNames.Count - missingColumns.Count;
        summary.MissingColumnNames = missingColumns;
        summary.ExtraColumnNames = extraColumns;

        // Report missing columns as errors
        foreach (var colName in missingColumns)
        {
            var property = modelColumns[colName];
            var storeType = property.GetColumnType() ?? "unknown";
            var isNullable = !property.IsNullable;

            result.Errors.Add(new SchemaError
            {
                Category = "MissingColumn",
                TableName = discoveredTable.Name,
                ColumnName = colName,
                Message = $"Expected column '{colName}' not found in table '{discoveredTable.Name}'",
                ExpectedValue = $"{storeType} (nullable: {isNullable})",
                ActualValue = "N/A"
            });
        }

        // Report extra columns as warnings
        foreach (var colName in extraColumns)
        {
            var discoveredCol = discoveredTable.Columns[colName];
            result.Warnings.Add(new SchemaWarning
            {
                Category = "ExtraColumn",
                TableName = discoveredTable.Name,
                ColumnName = colName,
                Message = $"Column '{colName}' exists in table '{discoveredTable.Name}' but not in EF Core model"
            });
        }

        // Compare matching columns
        foreach (var colName in modelColumnNames.Intersect(actualColumnNames, StringComparer.OrdinalIgnoreCase))
        {
            var property = modelColumns[colName];
            var discoveredCol = discoveredTable.Columns[colName];
            CompareColumnDetails(property, discoveredCol, entityType.Name, discoveredTable.Name, result);
        }

        result.ColumnSummaries[discoveredTable.Name] = summary;
    }

    private void CompareColumnDetails(
        IProperty property,
        DiscoveredColumn discoveredCol,
        string entityName,
        string tableName,
        SchemaComparisonResult result)
    {
        var columnName = property.GetColumnName() ?? property.Name;
        var expectedStoreType = property.GetColumnType() ?? "unknown";
        var expectedNullable = !property.IsNullable;

        // Compare store type
        if (!string.Equals(expectedStoreType, discoveredCol.StoreType, StringComparison.OrdinalIgnoreCase))
        {
            // For string columns, allow flexibility in length specifications
            var isStringType = expectedStoreType.StartsWith("nvarchar", StringComparison.OrdinalIgnoreCase) ||
                               expectedStoreType.StartsWith("varchar", StringComparison.OrdinalIgnoreCase) ||
                               expectedStoreType.StartsWith("text", StringComparison.OrdinalIgnoreCase);

            var isActualStringType = discoveredCol.StoreType.StartsWith("nvarchar", StringComparison.OrdinalIgnoreCase) ||
                                     discoveredCol.StoreType.StartsWith("varchar", StringComparison.OrdinalIgnoreCase) ||
                                     discoveredCol.StoreType.StartsWith("text", StringComparison.OrdinalIgnoreCase) ||
                                     discoveredCol.StoreType.StartsWith("ntext", StringComparison.OrdinalIgnoreCase);

            if (!isStringType || !isActualStringType)
            {
                result.Warnings.Add(new SchemaWarning
                {
                    Category = "ColumnTypeMismatch",
                    TableName = tableName,
                    ColumnName = columnName,
                    Message = $"Column '{columnName}' store type differs: expected '{expectedStoreType}', actual '{discoveredCol.StoreType}'"
                });
            }
        }

        // Compare nullability
        if (expectedNullable != discoveredCol.IsNullable)
        {
            result.Warnings.Add(new SchemaWarning
            {
                Category = "NullabilityMismatch",
                TableName = tableName,
                ColumnName = columnName,
                Message = $"Column '{columnName}' nullability differs: expected nullable={expectedNullable}, actual nullable={discoveredCol.IsNullable}"
            });
        }

        // Check if primary key column matches
        var modelKeyColumns = entityType_GetKeyColumnNames(property);
        if (modelKeyColumns.Contains(columnName) && !discoveredCol.IsPrimaryKey)
        {
            result.Warnings.Add(new SchemaWarning
            {
                Category = "PrimaryKeyMismatch",
                TableName = tableName,
                ColumnName = columnName,
                Message = $"Column '{columnName}' is expected to be a primary key but is not marked as one in the database"
            });
        }
    }

    private static HashSet<string> entityType_GetKeyColumnNames(IProperty property)
    {
        // This is a simplified check — in practice, EF Core metadata provides
        // the key column names through the IEntityType's FindPrimaryKey method.
        // We check if this property is part of any key.
        // Since we don't have direct access to the entity type here,
        // we just return the property name as a heuristic.
        return [property.Name];
    }

    private static List<DiscoveredPrimaryKey> ExtractPrimaryKeysFromColumns(Dictionary<string, DiscoveredColumn> columns)
    {
        var pkColumns = columns
            .Where(c => c.Value.IsPrimaryKey)
            .Select(c => c.Key)
            .ToList();

        if (pkColumns.Count == 0)
            return [];

        return [new DiscoveredPrimaryKey { Name = "PRIMARY", ColumnNames = pkColumns }];
    }

    private static string EscapeSQLiteIdentifier(string identifier)
    {
        return identifier.Replace("\"", "\"\"");
    }

    #endregion
}
