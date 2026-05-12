using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Shoko.Server.Data;
using Xunit;

#nullable enable
namespace Shoko.Tests;

public class MigrationVerificationTests : IDisposable
{
    private readonly string _dbPath;

    public MigrationVerificationTests()
    {
        _dbPath = Path.Combine(Path.GetTempPath(), $"shoko-migration-test-{Guid.NewGuid():N}.db");
    }

    public void Dispose()
    {
        if (File.Exists(_dbPath))
            File.Delete(_dbPath);
    }

    [Fact]
    public void Migration_CreatesAllTables()
    {
        var optionsBuilder = new DbContextOptionsBuilder<ShokoDbContext>();
        optionsBuilder.UseSqlite($"Data Source={_dbPath}");

        using (var context = new ShokoDbContext(optionsBuilder.Options))
        {
            context.Database.Migrate();
        }

        // Verify tables exist
        using var conn = new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={_dbPath}");
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT name FROM sqlite_master WHERE type = 'table' AND name NOT LIKE 'sqlite_%' ORDER BY name";
        
        var tables = new List<string>();
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
                tables.Add(reader.GetString(0));
        }

        // Check specific tables
        Assert.Contains("AniDB_Anime", tables);
        Assert.Contains("Trakt_Show", tables);
        Assert.Contains("Trakt_Season", tables);
        Assert.Contains("Trakt_Episode", tables);
        
        // Check AniDB_Anime columns
        cmd.CommandText = "PRAGMA table_info([AniDB_Anime])";
        var aniDbColumns = new List<string>();
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
                aniDbColumns.Add(reader.GetString(1));
        }
        
        Assert.Contains("AniDB_AnimeID", aniDbColumns);
        Assert.Contains("AnimeID", aniDbColumns);
        Assert.Contains("ANNID", aniDbColumns);
        Assert.Contains("AirDate", aniDbColumns);
        
        // Check Trakt_Show columns
        cmd.CommandText = "PRAGMA table_info([Trakt_Show])";
        var traktColumns = new List<string>();
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
                traktColumns.Add(reader.GetString(1));
        }
        
        Assert.Contains("Trakt_ShowID", traktColumns);
        Assert.Contains("TraktID", traktColumns);
        Assert.Contains("Title", traktColumns);
        Assert.Contains("Overview", traktColumns);
    }
}
