using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

#nullable enable

namespace Shoko.Server.Data.SchemaComparison;

/// <summary>
/// Result of a baseline registration operation.
/// </summary>
public class BaselineRegistrationResult
{
    /// <summary>
    /// Whether the baseline registration succeeded.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Whether the database is fresh (no existing tables) and eligible for normal migration.
    /// </summary>
    public bool IsFreshDatabase { get; set; }

    /// <summary>
    /// Whether the EF Core model schema matches the existing database.
    /// </summary>
    public bool SchemaMatches { get; set; }

    /// <summary>
    /// The migration ID that was registered (empty if not registered).
    /// </summary>
    public string RegisteredMigrationId { get; set; } = string.Empty;

    /// <summary>
    /// Provider-specific migration product version string.
    /// </summary>
    public string ProductVersion { get; set; } = string.Empty;

    /// <summary>
    /// Errors encountered during registration.
    /// </summary>
    public List<string> Errors { get; set; } = [];

    /// <summary>
    /// Warnings encountered during registration.
    /// </summary>
    public List<string> Warnings { get; set; } = [];

    /// <summary>
    /// Schema comparison result from the pre-registration validation step.
    /// </summary>
    public SchemaComparisonResult? SchemaComparison { get; set; }
}

/// <summary>
/// Handles registration of an EF Core migration baseline for existing NHibernate databases.
///
/// For existing databases: validates schema via SchemaComparer, then writes a no-op
/// baseline record to __EFMigrationsHistory if validation passes.
///
/// For fresh databases: skips registration so normal InitialCreate migration can run.
///
/// Provider-specific SQL is used to insert into __EFMigrationsHistory.
/// </summary>
public class BaselineRegistration
{
    private readonly DbContext _context;
    private readonly SchemaComparer _schemaComparer;
    private readonly string _providerName;
    private readonly string _baselineMigrationId;
    private readonly string _productVersion;

    /// <summary>
    /// Creates a new BaselineRegistration instance.
    /// </summary>
    /// <param name="context">The EF Core DbContext connected to the target database.</param>
    /// <param name="baselineMigrationId">Migration ID to register as baseline (default: "EFCoreBaseline").</param>
    /// <param name="productVersion">EF Core product version string (defaults to assembly version).</param>
    public BaselineRegistration(DbContext context, string? baselineMigrationId = null, string? productVersion = null)
    {
        _context = context;
        _schemaComparer = new SchemaComparer(context);
        _providerName = context.Database.ProviderName ?? string.Empty;
        _baselineMigrationId = baselineMigrationId ?? "EFCoreBaseline";
        _productVersion = productVersion ?? "EFCoreBaseline";
    }

    /// <summary>
    /// Registers the EF Core baseline migration for an existing NHibernate database.
    ///
    /// Workflow:
    /// 1. Compare EF Core model against actual database schema
    /// 2. If schema validation fails, return error (do not write to __EFMigrationsHistory)
    /// 3. If schema validation passes, check if __EFMigrationsHistory already exists
    /// 4. If it doesn't exist, register the baseline migration record
    /// 5. If it already exists and baseline is registered, skip (no-op)
    /// </summary>
    public async Task<BaselineRegistrationResult> RegisterBaselineAsync()
    {
        var result = new BaselineRegistrationResult
        {
            ProductVersion = _productVersion
        };

        // Step 1: Schema comparison (read-only)
        var comparison = await _schemaComparer.CompareAsync();
        result.SchemaComparison = comparison;

        // Step 2: Check if database is fresh (no tables at all)
        if (comparison.TableSummary.ActualTables == 0)
        {
            result.IsFreshDatabase = true;
            result.Success = true;
            result.SchemaMatches = true;
            result.Warnings.Add("Database is fresh — no baseline registration needed. InitialCreate migration can run normally.");
            return result;
        }

        // Step 3: If schema validation fails, do not register
        if (!comparison.IsValid)
        {
            result.Success = false;
            result.SchemaMatches = false;
            result.Errors.Add($"Schema validation failed with {comparison.Errors.Count} error(s) and {comparison.Warnings.Count} warning(s). Baseline registration aborted.");
            foreach (var error in comparison.Errors)
                result.Errors.Add($"  [{error.Category}] {error.Message}");
            foreach (var warning in comparison.Warnings)
                result.Warnings.Add($"  [{warning.Category}] {warning.Message}");
            return result;
        }

        // Step 4: Schema matches — proceed to register baseline
        result.SchemaMatches = true;

        // Step 5: Check if __EFMigrationsHistory already exists
        var migrationsHistoryExists = await CheckMigrationsHistoryTableExistsAsync();
        if (migrationsHistoryExists)
        {
            // Check if baseline is already registered
            var alreadyRegistered = await IsBaselineAlreadyRegisteredAsync();
            if (alreadyRegistered)
            {
                result.Success = true;
                result.RegisteredMigrationId = _baselineMigrationId;
                result.Warnings.Add("Baseline migration already registered — no action needed.");
                return result;
            }
        }

        // Step 6: Register the baseline migration
        try
        {
            if (!migrationsHistoryExists)
                await EnsureMigrationsHistoryTableExistsAsync();
            await RegisterBaselineInHistoryAsync();
            result.Success = true;
            result.RegisteredMigrationId = _baselineMigrationId;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Errors.Add($"Failed to register baseline in __EFMigrationsHistory: {ex.Message}");
        }

        return result;
    }

    /// <summary>
    /// Registers the EF Core baseline migration for an existing NHibernate database (synchronous).
    /// </summary>
    public BaselineRegistrationResult RegisterBaseline()
    {
        var result = new BaselineRegistrationResult
        {
            ProductVersion = _productVersion
        };

        // Step 1: Schema comparison (read-only)
        var comparison = _schemaComparer.Compare();
        result.SchemaComparison = comparison;

        // Step 2: Check if database is fresh (no tables at all)
        if (comparison.TableSummary.ActualTables == 0)
        {
            result.IsFreshDatabase = true;
            result.Success = true;
            result.SchemaMatches = true;
            result.Warnings.Add("Database is fresh — no baseline registration needed. InitialCreate migration can run normally.");
            return result;
        }

        // Step 3: If schema validation fails, do not register
        if (!comparison.IsValid)
        {
            result.Success = false;
            result.SchemaMatches = false;
            result.Errors.Add($"Schema validation failed with {comparison.Errors.Count} error(s) and {comparison.Warnings.Count} warning(s). Baseline registration aborted.");
            foreach (var error in comparison.Errors)
                result.Errors.Add($"  [{error.Category}] {error.Message}");
            foreach (var warning in comparison.Warnings)
                result.Warnings.Add($"  [{warning.Category}] {warning.Message}");
            return result;
        }

        // Step 4: Schema matches — proceed to register baseline
        result.SchemaMatches = true;

        // Step 5: Check if __EFMigrationsHistory already exists
        var migrationsHistoryExists = CheckMigrationsHistoryTableExists();
        if (migrationsHistoryExists)
        {
            // Check if baseline is already registered
            var alreadyRegistered = IsBaselineAlreadyRegistered();
            if (alreadyRegistered)
            {
                result.Success = true;
                result.RegisteredMigrationId = _baselineMigrationId;
                result.Warnings.Add("Baseline migration already registered — no action needed.");
                return result;
            }
        }

        // Step 6: Register the baseline migration
        try
        {
            if (!migrationsHistoryExists)
                EnsureMigrationsHistoryTableExists();
            RegisterBaselineInHistory();
            result.Success = true;
            result.RegisteredMigrationId = _baselineMigrationId;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Errors.Add($"Failed to register baseline in __EFMigrationsHistory: {ex.Message}");
        }

        return result;
    }

    private async Task<bool> CheckMigrationsHistoryTableExistsAsync()
    {
        return await ExecuteWithConnectionAsync(async connection =>
        {
            await using var cmd = connection.CreateCommand();
            if (_providerName.Contains("Sqlite", StringComparison.OrdinalIgnoreCase))
            {
                cmd.CommandText = @"
                    SELECT COUNT(*) FROM sqlite_master
                    WHERE type = 'table' AND name = '__EFMigrationsHistory'";
                var count = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                return count > 0;
            }
            else if (_providerName.Contains("MySql", StringComparison.OrdinalIgnoreCase) ||
                     _providerName.Contains("MariaDb", StringComparison.OrdinalIgnoreCase))
            {
                cmd.CommandText = @"
                    SELECT COUNT(*) FROM information_schema.TABLES
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = '__EFMigrationsHistory'
                      AND TABLE_TYPE = 'BASE TABLE'";
                var count = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                return count > 0;
            }
            else if (_providerName.Contains("SqlServer", StringComparison.OrdinalIgnoreCase))
            {
                cmd.CommandText = @"
                    SELECT COUNT(*) FROM sys.tables
                    WHERE name = '__EFMigrationsHistory'
                      AND schema_id = SCHEMA_ID('dbo')";
                var count = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                return count > 0;
            }
            return false;
        });
    }

    private bool CheckMigrationsHistoryTableExists()
    {
        return ExecuteWithConnection(connection =>
        {
            using var cmd = connection.CreateCommand();
            if (_providerName.Contains("Sqlite", StringComparison.OrdinalIgnoreCase))
            {
                cmd.CommandText = @"
                    SELECT COUNT(*) FROM sqlite_master
                    WHERE type = 'table' AND name = '__EFMigrationsHistory'";
                var count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
            else if (_providerName.Contains("MySql", StringComparison.OrdinalIgnoreCase) ||
                     _providerName.Contains("MariaDb", StringComparison.OrdinalIgnoreCase))
            {
                cmd.CommandText = @"
                    SELECT COUNT(*) FROM information_schema.TABLES
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = '__EFMigrationsHistory'
                      AND TABLE_TYPE = 'BASE TABLE'";
                var count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
            else if (_providerName.Contains("SqlServer", StringComparison.OrdinalIgnoreCase))
            {
                cmd.CommandText = @"
                    SELECT COUNT(*) FROM sys.tables
                    WHERE name = '__EFMigrationsHistory'
                      AND schema_id = SCHEMA_ID('dbo')";
                var count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
            return false;
        });
    }

    private async Task<bool> IsBaselineAlreadyRegisteredAsync()
    {
        return await ExecuteWithConnectionAsync(async connection =>
        {
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                SELECT COUNT(*) FROM __EFMigrationsHistory
                WHERE MigrationId = @migrationId";

            var param = cmd.CreateParameter();
            param.ParameterName = "@migrationId";
            param.Value = _baselineMigrationId;
            cmd.Parameters.Add(param);

            var count = Convert.ToInt32(await cmd.ExecuteScalarAsync());
            return count > 0;
        });
    }

    private bool IsBaselineAlreadyRegistered()
    {
        return ExecuteWithConnection(connection =>
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                SELECT COUNT(*) FROM __EFMigrationsHistory
                WHERE MigrationId = @migrationId";

            var param = cmd.CreateParameter();
            param.ParameterName = "@migrationId";
            param.Value = _baselineMigrationId;
            cmd.Parameters.Add(param);

            var count = Convert.ToInt32(cmd.ExecuteScalar());
            return count > 0;
        });
    }

    private async Task RegisterBaselineInHistoryAsync()
    {
        await ExecuteWithConnectionAsync(async connection =>
        {
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
                VALUES (@migrationId, @productVersion)";

            var migrationParam = cmd.CreateParameter();
            migrationParam.ParameterName = "@migrationId";
            migrationParam.Value = _baselineMigrationId;
            cmd.Parameters.Add(migrationParam);

            var versionParam = cmd.CreateParameter();
            versionParam.ParameterName = "@productVersion";
            versionParam.Value = _productVersion;
            cmd.Parameters.Add(versionParam);

            await cmd.ExecuteNonQueryAsync();
            return 0;
        });
    }

    private void RegisterBaselineInHistory()
    {
        ExecuteWithConnection(connection =>
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
                VALUES (@migrationId, @productVersion)";

            var migrationParam = cmd.CreateParameter();
            migrationParam.ParameterName = "@migrationId";
            migrationParam.Value = _baselineMigrationId;
            cmd.Parameters.Add(migrationParam);

            var versionParam = cmd.CreateParameter();
            versionParam.ParameterName = "@productVersion";
            versionParam.Value = _productVersion;
            cmd.Parameters.Add(versionParam);

            cmd.ExecuteNonQuery();
            return 0;
        });
    }

    private async Task EnsureMigrationsHistoryTableExistsAsync()
    {
        var historyRepository = _context.GetService<IHistoryRepository>();
        var createScript = historyRepository.GetCreateIfNotExistsScript();
        if (string.IsNullOrWhiteSpace(createScript))
            createScript = historyRepository.GetCreateScript();

        if (!string.IsNullOrWhiteSpace(createScript))
        {
            await ExecuteWithConnectionAsync(async connection =>
            {
                await using var cmd = connection.CreateCommand();
                cmd.CommandText = createScript;
                await cmd.ExecuteNonQueryAsync();
                return 0;
            });
        }
    }

    private void EnsureMigrationsHistoryTableExists()
    {
        var historyRepository = _context.GetService<IHistoryRepository>();
        var createScript = historyRepository.GetCreateIfNotExistsScript();
        if (string.IsNullOrWhiteSpace(createScript))
            createScript = historyRepository.GetCreateScript();

        if (!string.IsNullOrWhiteSpace(createScript))
        {
            ExecuteWithConnection(connection =>
            {
                using var cmd = connection.CreateCommand();
                cmd.CommandText = createScript;
                cmd.ExecuteNonQuery();
                return 0;
            });
        }
    }

    private async Task<T> ExecuteWithConnectionAsync<T>(Func<DbConnection, Task<T>> action)
    {
        await using var connection = CreateIndependentConnection();
        await connection.OpenAsync();
        return await action(connection);
    }

    private T ExecuteWithConnection<T>(Func<DbConnection, T> action)
    {
        using var connection = CreateIndependentConnection();
        connection.Open();
        return action(connection);
    }

    private DbConnection CreateIndependentConnection()
    {
        var connectionString = _context.Database.GetConnectionString();
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Database connection string is not available for baseline registration.");

        if (_providerName.Contains("Sqlite", StringComparison.OrdinalIgnoreCase))
            return new SqliteConnection(connectionString);

        if (_providerName.Contains("MySql", StringComparison.OrdinalIgnoreCase) ||
            _providerName.Contains("MariaDb", StringComparison.OrdinalIgnoreCase))
            return new MySqlConnection(connectionString);

        if (_providerName.Contains("SqlServer", StringComparison.OrdinalIgnoreCase))
            return new SqlConnection(connectionString);

        throw new NotSupportedException($"Unsupported database provider for baseline registration: '{_providerName}'.");
    }
}
