using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shoko.Server.Data;
using Shoko.Server.Models.TMDB;
using Xunit;

#nullable enable
namespace Shoko.Tests;

public class TmdbProductionCountryComparerTests
{
    [Fact]
    public void TMDB_Show_ProductionCountries_UsesComparerAndTracksStructuralChanges()
    {
        using var scope = CreateContext(out var warnings);

        var property = scope.Context.Model.FindEntityType(typeof(TMDB_Show))!.FindProperty(nameof(TMDB_Show.ProductionCountries));
        Assert.NotNull(property);
        Assert.NotNull(property!.GetValueComparer());
        Assert.DoesNotContain(warnings, message => message.Contains("ProductionCountries", StringComparison.OrdinalIgnoreCase));

        var show = CreateShow();
        scope.Context.TMDB_Show.Add(show);
        scope.Context.SaveChanges();

        show.ProductionCountries = new List<TMDB_ProductionCountry>
        {
            new("US", "United States"),
        };
        scope.Context.ChangeTracker.DetectChanges();
        Assert.False(scope.Context.Entry(show).Property(x => x.ProductionCountries).IsModified);

        show.ProductionCountries = new List<TMDB_ProductionCountry>
        {
            new("JP", "Japan"),
        };
        scope.Context.ChangeTracker.DetectChanges();
        Assert.True(scope.Context.Entry(show).Property(x => x.ProductionCountries).IsModified);
    }

    [Fact]
    public void TMDB_Movie_ProductionCountries_UsesComparerAndTracksStructuralChanges()
    {
        using var scope = CreateContext(out var warnings);

        var property = scope.Context.Model.FindEntityType(typeof(TMDB_Movie))!.FindProperty(nameof(TMDB_Movie.ProductionCountries));
        Assert.NotNull(property);
        Assert.NotNull(property!.GetValueComparer());
        Assert.DoesNotContain(warnings, message => message.Contains("ProductionCountries", StringComparison.OrdinalIgnoreCase));

        var movie = CreateMovie();
        scope.Context.TMDB_Movie.Add(movie);
        scope.Context.SaveChanges();

        movie.ProductionCountries = new List<TMDB_ProductionCountry>
        {
            new("US", "United States"),
        };
        scope.Context.ChangeTracker.DetectChanges();
        Assert.False(scope.Context.Entry(movie).Property(x => x.ProductionCountries).IsModified);

        movie.ProductionCountries = new List<TMDB_ProductionCountry>
        {
            new("JP", "Japan"),
        };
        scope.Context.ChangeTracker.DetectChanges();
        Assert.True(scope.Context.Entry(movie).Property(x => x.ProductionCountries).IsModified);
    }

    private static TestScope CreateContext(out List<string> warnings)
    {
        warnings = [];
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        var optionsBuilder = new DbContextOptionsBuilder<ShokoDbContext>();
        optionsBuilder.UseSqlite(connection);
        optionsBuilder.LogTo(warnings.Add, LogLevel.Warning);

        var context = new ShokoDbContext(optionsBuilder.Options);
        _ = context.Model;
        return new TestScope(context, connection);
    }

    private static TMDB_Show CreateShow()
        => new(1)
        {
            EnglishTitle = "Test Show",
            EnglishOverview = "Test Overview",
            OriginalTitle = "Test Show",
            OriginalLanguageCode = "en",
            IsRestricted = false,
            Genres = ["Drama"],
            Keywords = ["test"],
            ContentRatings = [new TMDB_ContentRating("US", "TV-14")],
            ProductionCountries = [new TMDB_ProductionCountry("US", "United States")],
            EpisodeCount = 1,
            HiddenEpisodeCount = 0,
            SeasonCount = 1,
            AlternateOrderingCount = 0,
            UserRating = 0,
            UserVotes = 0,
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            LastUpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        };

    private static TMDB_Movie CreateMovie()
        => new(1)
        {
            EnglishTitle = "Test Movie",
            EnglishOverview = "Test Overview",
            OriginalTitle = "Test Movie",
            OriginalLanguageCode = "en",
            IsRestricted = false,
            IsVideo = false,
            Genres = ["Drama"],
            Keywords = ["test"],
            ContentRatings = [new TMDB_ContentRating("US", "TV-14")],
            ProductionCountries = [new TMDB_ProductionCountry("US", "United States")],
            UserRating = 0,
            UserVotes = 0,
            ReleasedAt = new DateOnly(2026, 1, 1),
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            LastUpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        };

    private sealed class TestScope : IDisposable
    {
        public TestScope(ShokoDbContext context, SqliteConnection connection)
        {
            Context = context;
            Connection = connection;
            Context.Database.EnsureCreated();
        }

        public ShokoDbContext Context { get; }
        private SqliteConnection Connection { get; }

        public void Dispose()
        {
            Context.Dispose();
            Connection.Dispose();
        }
    }
}
