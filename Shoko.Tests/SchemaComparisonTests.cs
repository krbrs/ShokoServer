using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shoko.Server.Data;
using Shoko.Server.Data.SchemaComparison;
using Xunit;

#nullable enable
namespace Shoko.Tests;

public class SchemaComparisonTests : IDisposable
{
    private readonly string _dbPath;

    public SchemaComparisonTests()
    {
        _dbPath = Path.Combine(Path.GetTempPath(), $"shoko-schema-test-{Guid.NewGuid():N}.db");
    }

    public void Dispose()
    {
        if (File.Exists(_dbPath))
            File.Delete(_dbPath);
        var bak = _dbPath + ".bak";
        if (File.Exists(bak))
            File.Delete(bak);
    }

    [Fact]
    public void Compare_EFModel_MatchesAppliedMigration()
    {
        var optionsBuilder = new DbContextOptionsBuilder<ShokoDbContext>();
        optionsBuilder.UseSqlite($"Data Source={_dbPath}");

        // Migrate on a dedicated context to ensure tables are created
        using (var initContext = new ShokoDbContext(optionsBuilder.Options))
        {
            initContext.Database.Migrate();
        }

        // Verify Trakt_Show table exists and has correct columns
        using var verifyConn = new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={_dbPath}");
        verifyConn.Open();
        using var cmd = verifyConn.CreateCommand();
        
        // Check if Trakt_Show table exists
        cmd.CommandText = "SELECT name FROM sqlite_master WHERE type = 'table' AND name = 'Trakt_Show'";
        var hasTable = cmd.ExecuteScalar() != null;
        
        // Get columns for Trakt_Show
        cmd.CommandText = "PRAGMA table_info([Trakt_Show])";
        var traktColumns = new List<string>();
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
                traktColumns.Add(reader.GetString(1));
        }

        // Now create a fresh context with the same connection string for comparison
        using var context = new ShokoDbContext(optionsBuilder.Options);
        var comparer = new SchemaComparer(context);
        var result = comparer.Compare();

        Assert.True(result.IsValid, $"Trakt_Show exists: {hasTable}. Trakt_Show columns: [{string.Join(", ", traktColumns)}]. Errors: {string.Join(Environment.NewLine, result.Errors.Take(10).Select(e => $"{e.TableName}.{e.ColumnName}: {e.Message}"))}");
    }

    [Fact]
    public void Compare_PopulatedDatabase_MatchesEFModel()
    {
        var optionsBuilder = new DbContextOptionsBuilder<ShokoDbContext>();
        optionsBuilder.UseSqlite($"Data Source={_dbPath}");

        // Migrate on a dedicated context to ensure tables are created
        using (var initContext = new ShokoDbContext(optionsBuilder.Options))
        {
            initContext.Database.Migrate();
        }

        // Insert sample data to simulate a populated NHibernate database
        using (var populateContext = new ShokoDbContext(optionsBuilder.Options))
        {
            populateContext.AniDB_Anime.Add(new Shoko.Server.Models.AniDB.AniDB_Anime
            {
                AnimeID = 1,
                MainTitle = "Test Anime",
                EpisodeCount = 12,
                BeginYear = 2024,
                EndYear = 2024,
                AnimeType = (Shoko.Abstractions.Metadata.Enums.AnimeType)1,
                AllTitles = "[]",
                AllTags = "[]",
                Description = "Test description",
                EpisodeCountNormal = 12,
                EpisodeCountSpecial = 0,
                Rating = 75,
                VoteCount = 100,
                TempRating = 75,
                TempVoteCount = 100,
                AvgReviewRating = 0,
                ReviewCount = 0,
                DateTimeUpdated = DateTime.UtcNow,
                DateTimeDescUpdated = DateTime.UtcNow,
                ImageEnabled = 1,
                Restricted = 0
            });

            populateContext.AnimeSeries.Add(new Shoko.Server.Models.Shoko.AnimeSeries
            {
                AniDB_ID = 1,
                AnimeGroupID = 1,
                DateTimeCreated = DateTime.UtcNow,
                DateTimeUpdated = DateTime.UtcNow,
                LatestLocalEpisodeNumber = 12,
                MissingEpisodeCount = 0,
                MissingEpisodeCountGroups = 0,
                HiddenMissingEpisodeCount = 0,
                HiddenMissingEpisodeCountGroups = 0,
                UpdatedAt = DateTime.UtcNow
            });

            populateContext.AnimeGroup.Add(new Shoko.Server.Models.Shoko.AnimeGroup
            {
                DateTimeCreated = DateTime.UtcNow,
                DateTimeUpdated = DateTime.UtcNow,
                IsManuallyNamed = 0,
                MissingEpisodeCount = 0,
                MissingEpisodeCountGroups = 0
            });

            populateContext.JMMUser.Add(new Shoko.Server.Models.Shoko.JMMUser
            {
                IsAniDBUser = 1,
                IsTraktUser = 0,
                IsAdmin = 1,
                Username = "testuser"
            });

            populateContext.VideoLocal.Add(new Shoko.Server.Models.Shoko.VideoLocal
            {
                FileName = "test_episode_01.mp4",
                FileSize = 1024 * 1024 * 500,
                Hash = "E99A18C428CB38D5F260853678922E03",
                HashSource = 0,
                IsIgnored = false,
                IsVariation = false,
                DateTimeUpdated = DateTime.UtcNow,
                DateTimeCreated = DateTime.UtcNow,
                MediaVersion = 1,
                MyListID = 0
            });

            populateContext.SaveChanges();
        }

        // Now run schema comparison against the populated database
        using var context = new ShokoDbContext(optionsBuilder.Options);
        var comparer = new SchemaComparer(context);
        var result = comparer.Compare();

        Assert.True(result.IsValid, $"Errors: {string.Join(Environment.NewLine, result.Errors.Take(20).Select(e => $"{e.TableName}.{e.ColumnName}: {e.Message}"))}");
    }

    [Fact]
    public async Task BaselineRegistration_ExistingNHibernateDatabase_ValidatesAndRegisters()
    {
        var testProjectDir = Path.GetDirectoryName(typeof(SchemaComparisonTests).Assembly.Location)!;
        var solutionDir = Directory.GetParent(testProjectDir)!.Parent!.Parent!.Parent!.FullName;
        var shokoDbPath = Path.Combine(solutionDir, "Shoko.Server", "shoko.db");

        var nhibernateDbPath = Path.Combine(Path.GetTempPath(), "shoko-nhibernate-test.db");
        var testDbPath = Path.Combine(Path.GetTempPath(), $"shoko-baseline-test-{Guid.NewGuid():N}.db");

        if (!File.Exists(nhibernateDbPath))
        {
            File.Copy(shokoDbPath, nhibernateDbPath, true);
        }

        File.Copy(nhibernateDbPath, testDbPath, true);

        try
        {
            var optionsBuilder = new DbContextOptionsBuilder<ShokoDbContext>();
            optionsBuilder.UseSqlite($"Data Source={testDbPath}");

            using var context = new ShokoDbContext(optionsBuilder.Options);
            var baselineRegistration = new BaselineRegistration(context, "EFCoreBaseline", "1.0.0");
            var result = await baselineRegistration.RegisterBaselineAsync();

            Assert.True(result.Success, $"Baseline registration failed: {string.Join(Environment.NewLine, result.Errors)}");
            Assert.True(result.SchemaMatches, "Schema does not match EF Core model");
            Assert.Equal("EFCoreBaseline", result.RegisteredMigrationId);

            using var verifyConn = new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={testDbPath}");
            verifyConn.Open();
            using var cmd = verifyConn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM __EFMigrationsHistory WHERE MigrationId = 'EFCoreBaseline'";
            var count = Convert.ToInt32(cmd.ExecuteScalar());
            Assert.Equal(1, count);
        }
        finally
        {
            if (File.Exists(testDbPath))
                File.Delete(testDbPath);
        }
    }

    [Fact]
    public async Task BaselineRegistration_ExistingDatabase_NoDuplicateTables()
    {
        var optionsBuilder = new DbContextOptionsBuilder<ShokoDbContext>();
        optionsBuilder.UseSqlite($"Data Source={_dbPath}");

        using (var initContext = new ShokoDbContext(optionsBuilder.Options))
        {
            initContext.Database.Migrate();
        }

        using var verifyBefore = new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={_dbPath}");
        verifyBefore.Open();
        using var cmdBefore = verifyBefore.CreateCommand();
        cmdBefore.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name NOT LIKE 'sqlite_%'";
        var tableCountBefore = Convert.ToInt32(cmdBefore.ExecuteScalar());

        using var context = new ShokoDbContext(optionsBuilder.Options);
        var baselineRegistration = new BaselineRegistration(context, "EFCoreBaseline", "1.0.0");
        var result = await baselineRegistration.RegisterBaselineAsync();

        Assert.True(result.Success, $"Baseline registration failed: {string.Join(Environment.NewLine, result.Errors)}");
        Assert.True(result.SchemaMatches, "Schema does not match EF Core model");
        Assert.Equal("EFCoreBaseline", result.RegisteredMigrationId);

        using var verifyAfter = new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={_dbPath}");
        verifyAfter.Open();
        using var cmdAfter = verifyAfter.CreateCommand();
        cmdAfter.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name NOT LIKE 'sqlite_%'";
        var tableCountAfter = Convert.ToInt32(cmdAfter.ExecuteScalar());

        Assert.True(tableCountBefore == tableCountAfter, $"Duplicate tables created: before={tableCountBefore}, after={tableCountAfter}");

        using var verifyHistory = new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={_dbPath}");
        verifyHistory.Open();
        using var cmdHistory = verifyHistory.CreateCommand();
        cmdHistory.CommandText = "SELECT COUNT(*) FROM __EFMigrationsHistory WHERE MigrationId = 'EFCoreBaseline'";
        var historyCount = Convert.ToInt32(cmdHistory.ExecuteScalar());
        Assert.Equal(1, historyCount);

        var tableNamesBefore = new HashSet<string>();
        using (var cmdNames = verifyBefore.CreateCommand())
        {
            cmdNames.CommandText = "SELECT name FROM sqlite_master WHERE type = 'table' AND name NOT LIKE 'sqlite_%' ORDER BY name";
            using var reader = cmdNames.ExecuteReader();
            while (reader.Read())
                tableNamesBefore.Add(reader.GetString(0));
        }

        var tableNamesAfter = new HashSet<string>();
        using (var cmdNames = verifyAfter.CreateCommand())
        {
            cmdNames.CommandText = "SELECT name FROM sqlite_master WHERE type = 'table' AND name NOT LIKE 'sqlite_%' ORDER BY name";
            using var reader = cmdNames.ExecuteReader();
            while (reader.Read())
                tableNamesAfter.Add(reader.GetString(0));
        }

        var missingTables = tableNamesBefore.Except(tableNamesAfter).ToList();
        var extraTables = tableNamesAfter.Except(tableNamesBefore).ToList();
        if (missingTables.Count > 0 || extraTables.Count > 0)
        {
            var msg = "Table names differ after baseline registration: missing=[" + string.Join(", ", missingTables) + "] extra=[" + string.Join(", ", extraTables) + "]";
            Assert.Fail(msg);
        }
    }

    [Fact]
    public async Task BaselineRegistration_FreshDatabase_SkipsRegistration()
    {
        var optionsBuilder = new DbContextOptionsBuilder<ShokoDbContext>();
        optionsBuilder.UseSqlite($"Data Source={_dbPath}");

        using var context = new ShokoDbContext(optionsBuilder.Options);
        var baselineRegistration = new BaselineRegistration(context, "EFCoreBaseline", "1.0.0");
        var result = await baselineRegistration.RegisterBaselineAsync();

        Assert.True(result.Success, "Fresh database baseline registration failed");
        Assert.True(result.IsFreshDatabase, "Expected fresh database detection");
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task EfStartupActivation_ExistingSchemaWithoutHistory_RegistersInitialCreateAndIsIdempotent()
    {
        var optionsBuilder = new DbContextOptionsBuilder<ShokoDbContext>();
        optionsBuilder.UseSqlite($"Data Source={_dbPath}");

        string initialCreateMigrationId;
        using (var initContext = new ShokoDbContext(optionsBuilder.Options))
        {
            initialCreateMigrationId = initContext.Database.GetMigrations().Single(migrationId => migrationId.EndsWith("_InitialCreate", StringComparison.Ordinal));
            initContext.Database.EnsureCreated();
        }

        await using (var activationContext = new ShokoDbContext(optionsBuilder.Options))
        {
            var activationService = new EfStartupActivationService(activationContext);
            var activationResult = await activationService.ActivateAsync();

            Assert.True(activationResult.Success, string.Join(Environment.NewLine, activationResult.Errors));
            Assert.Equal(initialCreateMigrationId, activationResult.BaselineMigrationId);
            Assert.NotNull(activationResult.BaselineRegistration);
            Assert.True(activationResult.BaselineRegistration!.Success);
        }

        await using (var secondActivationContext = new ShokoDbContext(optionsBuilder.Options))
        {
            var activationService = new EfStartupActivationService(secondActivationContext);
            var activationResult = await activationService.ActivateAsync();

            Assert.True(activationResult.Success, string.Join(Environment.NewLine, activationResult.Errors));
            Assert.Empty(activationResult.AppliedMigrations);
        }

        await using var verifyConnection = new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={_dbPath}");
        await verifyConnection.OpenAsync();
        await using var verifyCommand = verifyConnection.CreateCommand();
        verifyCommand.CommandText = $"SELECT COUNT(*) FROM __EFMigrationsHistory WHERE MigrationId = '{initialCreateMigrationId}'";
        var registrationCount = Convert.ToInt32(await verifyCommand.ExecuteScalarAsync());
        Assert.Equal(1, registrationCount);
    }
}
