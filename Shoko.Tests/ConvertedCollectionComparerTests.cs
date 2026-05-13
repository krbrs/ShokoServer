using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shoko.Server.Data;
using Shoko.Server.Models.AniDB;
using Shoko.Server.Models.Shoko;
using Shoko.Server.Models.TMDB;
using Xunit;

namespace Shoko.Tests;

public class ConvertedCollectionComparerTests
{
    public static IEnumerable<object[]> TrackedCollectionProperties
    {
        get
        {
            yield return [typeof(AnimeEpisode_User), nameof(AnimeEpisode_User.UserTags)];
            yield return [typeof(AnimeSeries_User), nameof(AnimeSeries_User.UserTags)];
            yield return [typeof(TMDB_Movie), nameof(TMDB_Movie.ContentRatings)];
            yield return [typeof(TMDB_Movie), nameof(TMDB_Movie.Genres)];
            yield return [typeof(TMDB_Movie), nameof(TMDB_Movie.Keywords)];
            yield return [typeof(TMDB_Person), nameof(TMDB_Person.Aliases)];
            yield return [typeof(TMDB_Show), nameof(TMDB_Show.ContentRatings)];
            yield return [typeof(TMDB_Show), nameof(TMDB_Show.Genres)];
            yield return [typeof(TMDB_Show), nameof(TMDB_Show.Keywords)];
            yield return [typeof(TMDB_Movie), nameof(TMDB_Movie.ProductionCountries)];
            yield return [typeof(TMDB_Show), nameof(TMDB_Show.ProductionCountries)];
        }
    }

    [Theory]
    [MemberData(nameof(TrackedCollectionProperties))]
    public void ConvertedCollection_Properties_HaveComparers_And_NoStartupWarnings(Type entityType, string propertyName)
    {
        using var scope = CreateContext(out var warnings);

        var property = scope.Context.Model.FindEntityType(entityType)!.FindProperty(propertyName);
        Assert.NotNull(property);
        Assert.NotNull(property!.GetValueComparer());
        Assert.DoesNotContain(warnings, message =>
            message.Contains(entityType.Name, StringComparison.OrdinalIgnoreCase) &&
            message.Contains(propertyName, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void StringList_EquivalentAndChangedValues_TrackCorrectly()
    {
        using var scope = CreateContext(out _);
        var entity = CreateShow();
        scope.Context.TMDB_Show.Add(entity);
        scope.Context.SaveChanges();

        entity.Genres = ["Drama"];
        scope.Context.ChangeTracker.DetectChanges();
        Assert.False(scope.Context.Entry(entity).Property(x => x.Genres).IsModified);

        entity.Genres = ["Drama", "Action"];
        scope.Context.ChangeTracker.DetectChanges();
        Assert.True(scope.Context.Entry(entity).Property(x => x.Genres).IsModified);
    }

    [Fact]
    public void ContentRatings_EquivalentAndChangedValues_TrackCorrectly()
    {
        using var scope = CreateContext(out _);
        var entity = CreateShow();
        scope.Context.TMDB_Show.Add(entity);
        scope.Context.SaveChanges();

        entity.ContentRatings = [new TMDB_ContentRating("US", "TV-14")];
        scope.Context.ChangeTracker.DetectChanges();
        Assert.False(scope.Context.Entry(entity).Property(x => x.ContentRatings).IsModified);

        entity.ContentRatings = [new TMDB_ContentRating("US", "TV-MA")];
        scope.Context.ChangeTracker.DetectChanges();
        Assert.True(scope.Context.Entry(entity).Property(x => x.ContentRatings).IsModified);
    }

    [Fact]
    public void ProductionCountries_EquivalentAndChangedValues_TrackCorrectly()
    {
        using var scope = CreateContext(out _);
        var entity = CreateShow();
        scope.Context.TMDB_Show.Add(entity);
        scope.Context.SaveChanges();

        entity.ProductionCountries = [new TMDB_ProductionCountry("US", "United States")];
        scope.Context.ChangeTracker.DetectChanges();
        Assert.False(scope.Context.Entry(entity).Property(x => x.ProductionCountries).IsModified);

        entity.ProductionCountries = [new TMDB_ProductionCountry("JP", "Japan")];
        scope.Context.ChangeTracker.DetectChanges();
        Assert.True(scope.Context.Entry(entity).Property(x => x.ProductionCountries).IsModified);
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
