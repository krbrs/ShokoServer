using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using Shoko.Server.Data;
using Shoko.Server.Data.SchemaComparison;
using Shoko.Server.Models.Shoko;
using Shoko.Server.Repositories;
using Shoko.Server.Utilities;
using Xunit;

namespace Shoko.IntegrationTests.Providers;

/// <summary>
/// Validates EF Core MariaDB provider compatibility and provider-specific behaviors.
/// Runs against a MariaDB Docker container (mariadb:11.4.2) with database shoko_test.
/// </summary>
[Collection("Database")]
public class MariaDBProviderTests : IClassFixture<DatabaseMigrationFixture>
{
    private readonly DatabaseMigrationFixture _fixture;

    public MariaDBProviderTests(DatabaseMigrationFixture fixture)
    {
        _fixture = fixture;
        Assert.True(_fixture.Success, _fixture.FailureMessage ?? "MariaDB database initialization failed");
    }

    [Fact]
    public void MariaDB_CreateAndQueryAnimeSeries()
    {
        // Validate basic CRUD via EF Core against MariaDB
        var groupRepo = RepoFactory.AnimeGroup;
        var group = new AnimeGroup
        {
            GroupName = "MariaDB Test Group",
            DateTimeCreated = DateTime.UtcNow,
            DateTimeUpdated = DateTime.UtcNow
        };
        groupRepo.Save(group);
        
        var repo = RepoFactory.AnimeSeries;
        var series = new AnimeSeries
        {
            AniDB_ID = 222222,
            DateTimeCreated = DateTime.UtcNow,
            DateTimeUpdated = DateTime.UtcNow,
            AnimeGroupID = group.AnimeGroupID
        };
        repo.Save(series);
        
        var loaded = repo.GetByID(series.AnimeSeriesID);
        Assert.NotNull(loaded);
        Assert.Equal(222222, loaded.AniDB_ID);
    }

    [Fact]
    public void MariaDB_AnimeGroup_ExplicitCrudOperations()
    {
        var repo = RepoFactory.AnimeGroup;
        var createdAt = DateTime.UtcNow;
        var originalName = $"MariaDB CRUD Group {Guid.NewGuid():N}";
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
    public void MariaDB_TransactionCommit()
    {
        var groupName = $"MariaDB Commit Group {Guid.NewGuid():N}";
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
        {
            var context = readScope.ServiceProvider.GetRequiredService<ShokoDbContext>();
            var groups = context.AnimeGroup
                .AsNoTracking()
                .Where(group => group.GroupName == groupName)
                .Select(group => new
                {
                    group.AnimeGroupID,
                    group.GroupName,
                })
                .ToList();

            var group = Assert.Single(groups);
            Assert.True(group.AnimeGroupID > 0);
            Assert.Equal(groupName, group.GroupName);
        }
    }

    [Fact]
    public void MariaDB_TransactionRollback()
    {
        var groupName = $"MariaDB Rollback Group {Guid.NewGuid():N}";
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
        {
            var context = readScope.ServiceProvider.GetRequiredService<ShokoDbContext>();
            var matchingCount = context.AnimeGroup.Count(group => group.GroupName == groupName);
            var persistedById = context.AnimeGroup.AsNoTracking().SingleOrDefault(group => group.AnimeGroupID == transientId);

            Assert.Equal(0, matchingCount);
            Assert.Null(persistedById);
        }
    }

    [Fact]
    public void MariaDB_TransactionIsolationAcrossContexts()
    {
        var groupName = $"MariaDB Isolation Group {Guid.NewGuid():N}";

        using var writerScope = Utils.ServiceContainer.CreateScope();
        using var readerScope = Utils.ServiceContainer.CreateScope();
        var writerContext = writerScope.ServiceProvider.GetRequiredService<ShokoDbContext>();
        var readerContext = readerScope.ServiceProvider.GetRequiredService<ShokoDbContext>();

        using var writerTransaction = writerContext.Database.BeginTransaction();
        using var readerTransaction = readerContext.Database.BeginTransaction();

        writerContext.AnimeGroup.Add(new AnimeGroup
        {
            GroupName = groupName,
            DateTimeCreated = DateTime.UtcNow,
            DateTimeUpdated = DateTime.UtcNow
        });
        writerContext.SaveChanges();

        var visibleBeforeCommit = readerContext.AnimeGroup.Count(group => group.GroupName == groupName);
        Assert.Equal(0, visibleBeforeCommit);

        readerTransaction.Rollback();
        writerTransaction.Commit();

        using var verificationScope = Utils.ServiceContainer.CreateScope();
        var verificationContext = verificationScope.ServiceProvider.GetRequiredService<ShokoDbContext>();
        var visibleAfterCommit = verificationContext.AnimeGroup
            .AsNoTracking()
            .Where(group => group.GroupName == groupName)
            .Select(group => group.GroupName)
            .ToList();

        var persistedName = Assert.Single(visibleAfterCommit);
        Assert.Equal(groupName, persistedName);
    }

    [Fact]
    public void MariaDB_ComplexQueryWithJoins()
    {
        var groupRepo = RepoFactory.AnimeGroup;
        var seriesRepo = RepoFactory.AnimeSeries;
        var prefix = Guid.NewGuid().ToString("N");
        var matchingGroupName = $"MariaDB Query Group {prefix}";
        var otherGroupName = $"MariaDB Query Other Group {prefix}";
        var matchingAniDbId = 660002;

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
            AniDB_ID = 660001,
            DateTimeCreated = DateTime.UtcNow,
            DateTimeUpdated = DateTime.UtcNow,
            AnimeGroupID = matchingGroup.AnimeGroupID
        });
        seriesRepo.Save(new AnimeSeries
        {
            AniDB_ID = matchingAniDbId,
            DateTimeCreated = DateTime.UtcNow,
            DateTimeUpdated = DateTime.UtcNow,
            AnimeGroupID = matchingGroup.AnimeGroupID
        });
        seriesRepo.Save(new AnimeSeries
        {
            AniDB_ID = 660003,
            DateTimeCreated = DateTime.UtcNow,
            DateTimeUpdated = DateTime.UtcNow,
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
    public async Task MariaDB_ConcurrentReads()
    {
        // Validate concurrent read scenarios
        var groupRepo = RepoFactory.AnimeGroup;
        var group = new AnimeGroup
        {
            GroupName = "MariaDB Test Group 5",
            DateTimeCreated = DateTime.UtcNow,
            DateTimeUpdated = DateTime.UtcNow
        };
        groupRepo.Save(group);
        
        var repo = RepoFactory.AnimeSeries;
        var series = new AnimeSeries
        {
            AniDB_ID = 555555,
            DateTimeCreated = DateTime.UtcNow,
            DateTimeUpdated = DateTime.UtcNow,
            AnimeGroupID = group.AnimeGroupID
        };
        repo.Save(series);
        
        var tasks = new List<Task<AnimeSeries?>>();
        for (int i = 0; i < 5; i++)
        {
            tasks.Add(Task.Run(() => repo.GetByID(series.AnimeSeriesID)));
        }
        await Task.WhenAll(tasks);
        
        Assert.All(tasks, t => Assert.NotNull(t.Result));
    }

    [Fact]
    public async Task MariaDB_ProviderSpecificBehavior()
    {
        string connectionString;
        using (var metadataScope = Utils.ServiceContainer.CreateScope())
        {
            var metadataContext = metadataScope.ServiceProvider.GetRequiredService<ShokoDbContext>();
            connectionString = metadataContext.Database.GetConnectionString()!;
        }

        var groupNameMetadata = await GetStringColumnMetadataAsync(connectionString, "AnimeGroup", "GroupName");
        var createdPrecision = await GetDateTimePrecisionAsync(connectionString, "AnimeGroup", "DateTimeCreated");
        var nullablePrecision = await GetDateTimePrecisionAsync(connectionString, "AnimeGroup", "EpisodeAddedDate");

        var unicodeSuffix = Guid.NewGuid().ToString("N");
        var supportsSupplementaryUnicode = string.Equals(groupNameMetadata.CharacterSetName, "utf8mb4", StringComparison.OrdinalIgnoreCase);
        var unicodeGroupName = supportsSupplementaryUnicode
            ? $"Grüße 東京 🛰️ {unicodeSuffix}"
            : $"Grüße 東京 {unicodeSuffix}";

        var createdAt = new DateTime(2024, 05, 06, 12, 34, 56, 789, DateTimeKind.Utc).AddTicks(1234);
        var episodeAddedAt = new DateTime(2024, 05, 07, 01, 02, 03, 456, DateTimeKind.Utc).AddTicks(6543);
        var expectedCreatedAt = TruncateDateTime(createdAt, createdPrecision);
        var expectedEpisodeAddedAt = TruncateDateTime(episodeAddedAt, nullablePrecision);

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

            if (supportsSupplementaryUnicode)
                Assert.Contains("🛰️", loaded.GroupName);
            else
                Assert.DoesNotContain("🛰️", loaded.GroupName);
        }

        var collationProbeName = $"MariaDbCaseProbe-{Guid.NewGuid():N}-MiXeD";
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

            var isCaseInsensitiveCollation = groupNameMetadata.CollationName.Contains("_ci", StringComparison.OrdinalIgnoreCase);
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
    public async Task MariaDB_StartupActivation_AlreadyBaselinedSchema_RemainsIdempotent()
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

    private static async Task<HashSet<string>> GetBaseTableNamesAsync(string connectionString)
    {
        await using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();

        command.CommandText = @"
            SELECT TABLE_NAME
            FROM information_schema.TABLES
            WHERE TABLE_SCHEMA = DATABASE()
              AND TABLE_TYPE = 'BASE TABLE'
            ORDER BY TABLE_NAME";

        var tableNames = new HashSet<string>(StringComparer.Ordinal);
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            tableNames.Add(reader.GetString(0));

        return tableNames;
    }

    private static async Task<List<(string MigrationId, string ProductVersion)>> GetMigrationHistoryAsync(string connectionString)
    {
        await using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();

        command.CommandText = @"
            SELECT MigrationId, ProductVersion
            FROM __EFMigrationsHistory
            ORDER BY MigrationId";

        var rows = new List<(string MigrationId, string ProductVersion)>();
        await using var historyReader = await command.ExecuteReaderAsync();
        while (await historyReader.ReadAsync())
            rows.Add((historyReader.GetString(0), historyReader.GetString(1)));

        return rows;
    }

    private static async Task<(string CharacterSetName, string CollationName)> GetStringColumnMetadataAsync(string connectionString, string tableName, string columnName)
    {
        await using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT CHARACTER_SET_NAME, COLLATION_NAME
            FROM information_schema.COLUMNS
            WHERE TABLE_SCHEMA = DATABASE()
              AND TABLE_NAME = @tableName
              AND COLUMN_NAME = @columnName";
        command.Parameters.AddWithValue("@tableName", tableName);
        command.Parameters.AddWithValue("@columnName", columnName);

        await using var reader = await command.ExecuteReaderAsync();
        Assert.True(await reader.ReadAsync(), $"Column metadata for {tableName}.{columnName} was not found.");
        return (reader.GetString(0), reader.GetString(1));
    }

    private static async Task<int> GetDateTimePrecisionAsync(string connectionString, string tableName, string columnName)
    {
        await using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT COALESCE(DATETIME_PRECISION, 0)
            FROM information_schema.COLUMNS
            WHERE TABLE_SCHEMA = DATABASE()
              AND TABLE_NAME = @tableName
              AND COLUMN_NAME = @columnName";
        command.Parameters.AddWithValue("@tableName", tableName);
        command.Parameters.AddWithValue("@columnName", columnName);

        var precision = await command.ExecuteScalarAsync();
        Assert.NotNull(precision);
        return Convert.ToInt32(precision);
    }

    private static DateTime TruncateDateTime(DateTime value, int fractionalSecondsPrecision)
    {
        var ticksPerUnit = fractionalSecondsPrecision switch
        {
            <= 0 => TimeSpan.TicksPerSecond,
            1 => TimeSpan.TicksPerSecond / 10,
            2 => TimeSpan.TicksPerSecond / 100,
            3 => TimeSpan.TicksPerSecond / 1000,
            4 => TimeSpan.TicksPerSecond / 10000,
            5 => TimeSpan.TicksPerSecond / 100000,
            6 => 10,
            _ => throw new ArgumentOutOfRangeException(nameof(fractionalSecondsPrecision), fractionalSecondsPrecision, "MariaDB DATETIME precision must be between 0 and 6.")
        };

        return new DateTime(value.Ticks - (value.Ticks % ticksPerUnit), value.Kind);
    }
}
