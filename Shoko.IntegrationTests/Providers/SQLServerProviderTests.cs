#nullable enable
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shoko.Server.Data;
using Shoko.Server.Data.SchemaComparison;
using Shoko.Server.Models.Shoko;
using Shoko.Server.Repositories;
using Shoko.Server.Utilities;
using Xunit;

namespace Shoko.IntegrationTests.Providers;

/// <summary>
/// Validates EF Core SQL Server provider compatibility and provider-specific behaviors.
/// Runs against the reproducible SQL Server 2022 Express Docker environment from T163.
/// </summary>
[Collection("Database")]
public class SQLServerProviderTests : IClassFixture<DatabaseMigrationFixture>
{
    private readonly DatabaseMigrationFixture _fixture;

    public SQLServerProviderTests(DatabaseMigrationFixture fixture)
    {
        _fixture = fixture;
        Assert.True(_fixture.Success, _fixture.FailureMessage ?? "SQL Server database initialization failed");
    }

    [Fact]
    public async Task SQLServer_DbContext_CanConnect_AndQueryVersions()
    {
        using var scope = Utils.ServiceContainer.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ShokoDbContext>();

        Assert.Contains("SqlServer", context.Database.ProviderName);
        Assert.True(await context.Database.CanConnectAsync());

        var versionRows = await context.Versions.AsNoTracking().CountAsync();
        Assert.True(versionRows > 0);
    }

    [Fact]
    public void SQLServer_CreateAndQueryAnimeSeries()
    {
        var groupRepo = RepoFactory.AnimeGroup;
        var group = new AnimeGroup
        {
            GroupName = "SQLServer Test Group",
            DateTimeCreated = DateTime.UtcNow,
            DateTimeUpdated = DateTime.UtcNow
        };
        groupRepo.Save(group);

        var repo = RepoFactory.AnimeSeries;
        var series = new AnimeSeries
        {
            AniDB_ID = 888888,
            DateTimeCreated = DateTime.UtcNow,
            DateTimeUpdated = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            AnimeGroupID = group.AnimeGroupID
        };
        repo.Save(series);

        var loaded = repo.GetByID(series.AnimeSeriesID);
        Assert.NotNull(loaded);
        Assert.Equal(888888, loaded.AniDB_ID);
    }

    [Fact]
    public void SQLServer_AnimeGroup_ExplicitCrudOperations()
    {
        var repo = RepoFactory.AnimeGroup;
        var createdAt = DateTime.UtcNow;
        var originalName = $"SQLServer CRUD Group {Guid.NewGuid():N}";
        var updatedName = $"{originalName} Updated";

        var group = new AnimeGroup
        {
            GroupName = originalName,
            DateTimeCreated = createdAt,
            DateTimeUpdated = createdAt
        };

        repo.Save(group);

        var created = repo.GetByID(group.AnimeGroupID);
        Assert.NotNull(created);
        Assert.Equal(originalName, created.GroupName);

        created.GroupName = updatedName;
        created.DateTimeUpdated = DateTime.UtcNow;
        repo.Save(created);

        var updated = repo.GetByID(group.AnimeGroupID);
        Assert.NotNull(updated);
        Assert.Equal(updatedName, updated.GroupName);

        repo.Delete(updated);

        var deleted = repo.GetByID(group.AnimeGroupID);
        Assert.Null(deleted);
    }

    [Fact]
    public void SQLServer_TransactionCommit()
    {
        var groupName = $"SQLServer Commit Group {Guid.NewGuid():N}";
        var createdAt = DateTime.UtcNow;

        using (var writeScope = Utils.ServiceContainer.CreateScope())
        {
            var context = writeScope.ServiceProvider.GetRequiredService<ShokoDbContext>();
            using var transaction = context.Database.BeginTransaction();

            context.AnimeGroup.Add(new AnimeGroup
            {
                GroupName = groupName,
                DateTimeCreated = createdAt,
                DateTimeUpdated = createdAt
            });
            context.SaveChanges();

            transaction.Commit();
        }

        using var readScope = Utils.ServiceContainer.CreateScope();
        var readContext = readScope.ServiceProvider.GetRequiredService<ShokoDbContext>();
        var groups = readContext.AnimeGroup
            .AsNoTracking()
            .Where(group => group.GroupName == groupName)
            .Select(group => new
            {
                group.AnimeGroupID,
                group.GroupName
            })
            .ToList();

        var group = Assert.Single(groups);
        Assert.True(group.AnimeGroupID > 0);
        Assert.Equal(groupName, group.GroupName);
    }

    [Fact]
    public void SQLServer_TransactionRollback()
    {
        var groupName = $"SQLServer Rollback Group {Guid.NewGuid():N}";
        int transientId;

        using (var writeScope = Utils.ServiceContainer.CreateScope())
        {
            var context = writeScope.ServiceProvider.GetRequiredService<ShokoDbContext>();
            using var transaction = context.Database.BeginTransaction();

            var group = new AnimeGroup
            {
                GroupName = groupName,
                DateTimeCreated = DateTime.UtcNow,
                DateTimeUpdated = DateTime.UtcNow
            };

            context.AnimeGroup.Add(group);
            context.SaveChanges();
            transientId = group.AnimeGroupID;

            transaction.Rollback();
        }

        using var readScope = Utils.ServiceContainer.CreateScope();
        var readContext = readScope.ServiceProvider.GetRequiredService<ShokoDbContext>();
        var matchingCount = readContext.AnimeGroup.Count(group => group.GroupName == groupName);
        var persistedById = readContext.AnimeGroup.AsNoTracking().SingleOrDefault(group => group.AnimeGroupID == transientId);

        Assert.Equal(0, matchingCount);
        Assert.Null(persistedById);
    }

    [Fact]
    public async Task SQLServer_TransactionIsolationAcrossContexts()
    {
        string connectionString;
        using (var metadataScope = Utils.ServiceContainer.CreateScope())
        {
            var metadataContext = metadataScope.ServiceProvider.GetRequiredService<ShokoDbContext>();
            connectionString = metadataContext.Database.GetConnectionString()!;
        }

        var isolationSettings = await GetDatabaseIsolationSettingsAsync(connectionString);
        var groupName = $"SQLServer Isolation Group {Guid.NewGuid():N}";
        int transientId;

        using var writerScope = Utils.ServiceContainer.CreateScope();
        var writerContext = writerScope.ServiceProvider.GetRequiredService<ShokoDbContext>();
        using var writerTransaction = await writerContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

        var group = new AnimeGroup
        {
            GroupName = groupName,
            DateTimeCreated = DateTime.UtcNow,
            DateTimeUpdated = DateTime.UtcNow
        };

        writerContext.AnimeGroup.Add(group);
        await writerContext.SaveChangesAsync();
        transientId = group.AnimeGroupID;

        using (var readerScope = Utils.ServiceContainer.CreateScope())
        {
            var readerContext = readerScope.ServiceProvider.GetRequiredService<ShokoDbContext>();
            readerContext.Database.SetCommandTimeout(2);

            if (isolationSettings.ReadCommittedSnapshotOn || isolationSettings.SnapshotIsolationOn)
            {
                var visibleBeforeCommit = await readerContext.AnimeGroup
                    .AsNoTracking()
                    .SingleOrDefaultAsync(item => item.AnimeGroupID == transientId);

                Assert.Null(visibleBeforeCommit);
            }
            else
            {
                var exception = await Assert.ThrowsAsync<SqlException>(async () =>
                    await readerContext.AnimeGroup
                        .AsNoTracking()
                        .SingleOrDefaultAsync(item => item.AnimeGroupID == transientId));

                Assert.True(exception.Number == -2 || exception.Message.Contains("Timeout", StringComparison.OrdinalIgnoreCase),
                    $"Expected a timeout while reading an uncommitted row under READ COMMITTED. Actual SQL error: {exception.Number} {exception.Message}");
            }
        }

        await writerTransaction.CommitAsync();

        using var verificationScope = Utils.ServiceContainer.CreateScope();
        var verificationContext = verificationScope.ServiceProvider.GetRequiredService<ShokoDbContext>();
        var visibleAfterCommit = await verificationContext.AnimeGroup
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.AnimeGroupID == transientId);

        Assert.NotNull(visibleAfterCommit);
        Assert.Equal(groupName, visibleAfterCommit.GroupName);
    }

    [Fact]
    public void SQLServer_ComplexQueryWithJoins()
    {
        var groupRepo = RepoFactory.AnimeGroup;
        var seriesRepo = RepoFactory.AnimeSeries;
        var prefix = Guid.NewGuid().ToString("N");
        var matchingGroupName = $"SQLServer Query Group {prefix}";
        var otherGroupName = $"SQLServer Query Other Group {prefix}";
        var matchingAniDbId = 860002;

        var matchingGroup = new AnimeGroup
        {
            GroupName = matchingGroupName,
            DateTimeCreated = DateTime.UtcNow,
            DateTimeUpdated = DateTime.UtcNow
        };
        var otherGroup = new AnimeGroup
        {
            GroupName = otherGroupName,
            DateTimeCreated = DateTime.UtcNow,
            DateTimeUpdated = DateTime.UtcNow,
        };
        groupRepo.Save(matchingGroup);
        groupRepo.Save(otherGroup);

        seriesRepo.Save(new AnimeSeries
        {
            AniDB_ID = 860001,
            DateTimeCreated = DateTime.UtcNow,
            DateTimeUpdated = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            AnimeGroupID = matchingGroup.AnimeGroupID
        });
        seriesRepo.Save(new AnimeSeries
        {
            AniDB_ID = matchingAniDbId,
            DateTimeCreated = DateTime.UtcNow,
            DateTimeUpdated = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            AnimeGroupID = matchingGroup.AnimeGroupID
        });
        seriesRepo.Save(new AnimeSeries
        {
            AniDB_ID = 860003,
            DateTimeCreated = DateTime.UtcNow,
            DateTimeUpdated = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            AnimeGroupID = otherGroup.AnimeGroupID
        });

        using var scope = Utils.ServiceContainer.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ShokoDbContext>();

        var results = context.AnimeSeries
            .AsNoTracking()
            .Join(
                context.AnimeGroup.AsNoTracking(),
                series => series.AnimeGroupID,
                group => group.AnimeGroupID,
                (series, group) => new { series, group })
            .Where(row => row.group.GroupName == matchingGroupName && row.series.AniDB_ID >= matchingAniDbId)
            .OrderBy(row => row.series.AniDB_ID)
            .Select(row => new
            {
                row.series.AniDB_ID,
                row.group.GroupName,
                row.series.AnimeGroupID
            })
            .ToList();

        var result = Assert.Single(results);
        Assert.Equal(matchingAniDbId, result.AniDB_ID);
        Assert.Equal(matchingGroupName, result.GroupName);
        Assert.Equal(matchingGroup.AnimeGroupID, result.AnimeGroupID);
    }

    [Fact]
    public async Task SQLServer_ConcurrentReads()
    {
        var groupRepo = RepoFactory.AnimeGroup;
        var group = new AnimeGroup
        {
            GroupName = "SQLServer Test Group 5",
            DateTimeCreated = DateTime.UtcNow,
            DateTimeUpdated = DateTime.UtcNow
        };
        groupRepo.Save(group);

        var repo = RepoFactory.AnimeSeries;
        var series = new AnimeSeries
        {
            AniDB_ID = 777777,
            DateTimeCreated = DateTime.UtcNow,
            DateTimeUpdated = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            AnimeGroupID = group.AnimeGroupID
        };
        repo.Save(series);

        var tasks = new List<Task<AnimeSeries?>>();
        for (var i = 0; i < 5; i++)
        {
            tasks.Add(Task.Run<AnimeSeries?>(() => repo.GetByID(series.AnimeSeriesID)));
        }

        await Task.WhenAll(tasks);

        Assert.All(tasks, task => Assert.NotNull(task.Result));
    }

    [Fact]
    public async Task SQLServer_ProviderSpecificBehavior()
    {
        string connectionString;
        using (var metadataScope = Utils.ServiceContainer.CreateScope())
        {
            var metadataContext = metadataScope.ServiceProvider.GetRequiredService<ShokoDbContext>();
            connectionString = metadataContext.Database.GetConnectionString()!;
        }

        var columnMetadata = await GetColumnMetadataAsync(connectionString, "AnimeGroup", "GroupName");
        var createdPrecision = await GetDateTimeScaleAsync(connectionString, "AnimeGroup", "DateTimeCreated");
        var nullablePrecision = await GetDateTimeScaleAsync(connectionString, "AnimeGroup", "EpisodeAddedDate");

        var unicodeSuffix = Guid.NewGuid().ToString("N");
        var unicodeGroupName = $"Grüße 東京 🛰️ {unicodeSuffix}";
        var createdAt = new DateTime(2024, 05, 06, 12, 34, 56, 789, DateTimeKind.Utc).AddTicks(1234);
        var episodeAddedAt = new DateTime(2024, 05, 07, 01, 02, 03, 456, DateTimeKind.Utc).AddTicks(6543);
        var expectedCreatedAt = TruncateDateTimeForDateTime2(createdAt, createdPrecision);
        var expectedEpisodeAddedAt = TruncateDateTimeForDateTime2(episodeAddedAt, nullablePrecision);

        int unicodeGroupId;
        using (var writeScope = Utils.ServiceContainer.CreateScope())
        {
            var context = writeScope.ServiceProvider.GetRequiredService<ShokoDbContext>();
            var unicodeGroup = new AnimeGroup
            {
                GroupName = unicodeGroupName,
                DateTimeCreated = createdAt,
                DateTimeUpdated = createdAt,
                EpisodeAddedDate = episodeAddedAt
            };

            context.AnimeGroup.Add(unicodeGroup);
            await context.SaveChangesAsync();
            unicodeGroupId = unicodeGroup.AnimeGroupID;
        }

        using (var readScope = Utils.ServiceContainer.CreateScope())
        {
            var context = readScope.ServiceProvider.GetRequiredService<ShokoDbContext>();
            var loaded = await context.AnimeGroup
                .AsNoTracking()
                .SingleAsync(group => group.AnimeGroupID == unicodeGroupId);

            Assert.Equal(unicodeGroupName, loaded.GroupName);
            Assert.Equal(expectedCreatedAt.Ticks, loaded.DateTimeCreated.Ticks);
            Assert.Equal(expectedEpisodeAddedAt.Ticks, loaded.EpisodeAddedDate?.Ticks);
            Assert.Contains("🛰️", loaded.GroupName);
        }

        var collationProbeName = $"SqlServerCaseProbe-{Guid.NewGuid():N}-MiXeD";
        int collationProbeId;
        using (var writeScope = Utils.ServiceContainer.CreateScope())
        {
            var context = writeScope.ServiceProvider.GetRequiredService<ShokoDbContext>();
            var collationGroup = new AnimeGroup
            {
                GroupName = collationProbeName,
                DateTimeCreated = DateTime.UtcNow,
                DateTimeUpdated = DateTime.UtcNow
            };

            context.AnimeGroup.Add(collationGroup);
            await context.SaveChangesAsync();
            collationProbeId = collationGroup.AnimeGroupID;
        }

        var alternativeCaseName = collationProbeName.Replace("MiXeD", "mixed", StringComparison.Ordinal);
        using (var readScope = Utils.ServiceContainer.CreateScope())
        {
            var context = readScope.ServiceProvider.GetRequiredService<ShokoDbContext>();
            var matches = await context.AnimeGroup
                .AsNoTracking()
                .Where(group => group.GroupName == alternativeCaseName)
                .Select(group => group.AnimeGroupID)
                .ToListAsync();

            var isCaseInsensitiveCollation = columnMetadata.CollationName?.Contains("_CI_", StringComparison.OrdinalIgnoreCase) == true ||
                                             columnMetadata.CollationName?.EndsWith("_CI_AS", StringComparison.OrdinalIgnoreCase) == true ||
                                             columnMetadata.CollationName?.EndsWith("_CI_AI", StringComparison.OrdinalIgnoreCase) == true;
            if (isCaseInsensitiveCollation)
            {
                Assert.Contains(collationProbeId, matches);
            }
            else
            {
                Assert.DoesNotContain(collationProbeId, matches);
            }
        }
    }

    [Fact]
    public async Task SQLServer_StartupActivation_AlreadyBaselinedSchema_RemainsIdempotent()
    {
        string connectionString;
        string initialCreateMigrationId;
        int animeGroupCountBefore;
        using (var compareScope = Utils.ServiceContainer.CreateScope())
        {
            var compareContext = compareScope.ServiceProvider.GetRequiredService<ShokoDbContext>();
            connectionString = compareContext.Database.GetConnectionString()!;
            var preComparison = await new SchemaComparer(compareContext).CompareAsync();
            Assert.True(preComparison.IsValid,
                $"Pre-activation schema comparison failed:{Environment.NewLine}{string.Join(Environment.NewLine, preComparison.Errors.Select(error => error.Message))}");
            animeGroupCountBefore = await compareContext.AnimeGroup.AsNoTracking().CountAsync();
            initialCreateMigrationId = compareContext.Database.GetMigrations().Single(migrationId => migrationId.EndsWith("_InitialCreate", StringComparison.Ordinal));
        }

        var tableNamesBefore = await GetBaseTableNamesAsync(connectionString);
        Assert.Contains("__EFMigrationsHistory", tableNamesBefore);

        using (var baselineScope = Utils.ServiceContainer.CreateScope())
        {
            var baselineContext = baselineScope.ServiceProvider.GetRequiredService<ShokoDbContext>();
            var activationService = new EfStartupActivationService(baselineContext);
            var registrationResult = await activationService.ActivateAsync();

            Assert.True(registrationResult.Success, $"Startup activation failed: {string.Join(Environment.NewLine, registrationResult.Errors)}");
            Assert.Equal(initialCreateMigrationId, registrationResult.BaselineMigrationId);
        }

        var tableNamesAfter = await GetBaseTableNamesAsync(connectionString);
        Assert.Contains("__EFMigrationsHistory", tableNamesAfter);
        Assert.True(tableNamesBefore.SetEquals(tableNamesAfter), "Startup activation changed the existing table set.");

        var registeredHistory = await GetMigrationHistoryAsync(connectionString);
        var initialCreateEntries = registeredHistory.Where(entry => entry.MigrationId == initialCreateMigrationId).ToList();
        var historyEntry = Assert.Single(initialCreateEntries);
        Assert.NotEmpty(historyEntry.ProductVersion);

        using (var postStateScope = Utils.ServiceContainer.CreateScope())
        {
            var postStateContext = postStateScope.ServiceProvider.GetRequiredService<ShokoDbContext>();
            var animeGroupCountAfter = await postStateContext.AnimeGroup.AsNoTracking().CountAsync();
            Assert.Equal(animeGroupCountBefore, animeGroupCountAfter);
        }

        using (var postCompareScope = Utils.ServiceContainer.CreateScope())
        {
            var postCompareContext = postCompareScope.ServiceProvider.GetRequiredService<ShokoDbContext>();
            var postComparison = await new SchemaComparer(postCompareContext).CompareAsync();
            Assert.True(postComparison.IsValid,
                $"Post-activation schema comparison failed:{Environment.NewLine}{string.Join(Environment.NewLine, postComparison.Errors.Select(error => error.Message))}");
        }
    }

    private static async Task<SqlServerColumnMetadata> GetColumnMetadataAsync(string connectionString, string tableName, string columnName)
    {
        const string sql = """
SELECT c.collation_name, t.name AS data_type
FROM sys.columns c
INNER JOIN sys.tables tbl ON c.object_id = tbl.object_id
INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
WHERE tbl.name = @tableName AND c.name = @columnName;
""";

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@tableName", tableName);
        command.Parameters.AddWithValue("@columnName", columnName);

        await using var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow);
        Assert.True(await reader.ReadAsync(), $"Expected metadata for {tableName}.{columnName}");

        return new SqlServerColumnMetadata(
            reader.IsDBNull(0) ? null : reader.GetString(0),
            reader.GetString(1));
    }

    private static async Task<int> GetDateTimeScaleAsync(string connectionString, string tableName, string columnName)
    {
        const string sql = """
SELECT DATETIME_PRECISION
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = @tableName AND COLUMN_NAME = @columnName;
""";

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@tableName", tableName);
        command.Parameters.AddWithValue("@columnName", columnName);

        var result = await command.ExecuteScalarAsync();
        Assert.NotNull(result);
        return Convert.ToInt32(result);
    }

    private static async Task<SqlServerIsolationSettings> GetDatabaseIsolationSettingsAsync(string connectionString)
    {
        const string sql = """
SELECT is_read_committed_snapshot_on, snapshot_isolation_state
FROM sys.databases
WHERE name = DB_NAME();
""";

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow);
        Assert.True(await reader.ReadAsync(), "Expected SQL Server database isolation metadata.");

        var readCommittedSnapshotOn = reader.GetBoolean(0);
        var snapshotIsolationState = Convert.ToInt32(reader.GetValue(1));
        return new SqlServerIsolationSettings(readCommittedSnapshotOn, snapshotIsolationState == 1 || snapshotIsolationState == 3);
    }

    private static async Task<HashSet<string>> GetBaseTableNamesAsync(string connectionString)
    {
        const string sql = """
SELECT TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;
""";

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);

        var tableNames = new HashSet<string>(StringComparer.Ordinal);
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            tableNames.Add(reader.GetString(0));

        return tableNames;
    }

    private static async Task<List<(string MigrationId, string ProductVersion)>> GetMigrationHistoryAsync(string connectionString)
    {
        const string sql = """
SELECT MigrationId, ProductVersion
FROM __EFMigrationsHistory
ORDER BY MigrationId;
""";

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        await using var command = new SqlCommand(sql, connection);

        var rows = new List<(string MigrationId, string ProductVersion)>();
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            rows.Add((reader.GetString(0), reader.GetString(1)));

        return rows;
    }

    private static DateTime TruncateDateTimeForDateTime2(DateTime value, int scale)
    {
        if (scale >= 7)
            return value;

        var factor = (long)Math.Pow(10, 7 - Math.Max(scale, 0));
        var truncatedTicks = value.Ticks - (value.Ticks % factor);
        return new DateTime(truncatedTicks, value.Kind);
    }

    private sealed record SqlServerColumnMetadata(string? CollationName, string DataType);
    private sealed record SqlServerIsolationSettings(bool ReadCommittedSnapshotOn, bool SnapshotIsolationOn);
}
