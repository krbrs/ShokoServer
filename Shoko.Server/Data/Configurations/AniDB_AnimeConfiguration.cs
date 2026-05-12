using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Models.AniDB;

namespace Shoko.Server.Data.Configurations;

/// <summary>
/// EF Core configuration for <see cref="AniDB_Anime"/> — raw AniDB metadata cache.
/// Based on NHibernate mapping: Shoko.Server/Mappings/AniDB_AnimeMap.cs
/// </summary>
public class AniDB_AnimeConfiguration : IEntityTypeConfiguration<AniDB_Anime>
{
    public void Configure(EntityTypeBuilder<AniDB_Anime> builder)
    {
        builder.ToTable("AniDB_Anime");

        builder.HasKey(x => x.AniDB_AnimeID);

        builder.Property(x => x.AniDB_AnimeID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.AnimeID)
            .IsRequired();

        builder.Property(x => x.EpisodeCount)
            .IsRequired();

        builder.Property(x => x.AirDate);

        builder.Property(x => x.EndDate);

        builder.Property(x => x.URL);

        builder.Property(x => x.Picname);

        builder.Property(x => x.BeginYear)
            .IsRequired();

        builder.Property(x => x.EndYear)
            .IsRequired();

        builder.Property(x => x.AnimeType)
            .IsRequired();

        builder.Property(x => x.MainTitle)
            .IsRequired();

        builder.Property(x => x.AllTitles)
            .IsRequired();

        builder.Property(x => x.AllTags)
            .IsRequired();

        builder.Property(x => x.Description)
            .IsRequired();

        builder.Property(x => x.EpisodeCountNormal)
            .IsRequired();

        builder.Property(x => x.EpisodeCountSpecial)
            .IsRequired();

        builder.Property(x => x.Rating)
            .IsRequired();

        builder.Property(x => x.VoteCount)
            .IsRequired();

        builder.Property(x => x.TempRating)
            .IsRequired();

        builder.Property(x => x.TempVoteCount)
            .IsRequired();

        builder.Property(x => x.AvgReviewRating)
            .IsRequired();

        builder.Property(x => x.ReviewCount)
            .IsRequired();

        builder.Property(x => x.DateTimeUpdated)
            .IsRequired();

        builder.Property(x => x.DateTimeDescUpdated)
            .IsRequired();

        builder.Property(x => x.ImageEnabled)
            .IsRequired();

        builder.Property(x => x.Restricted)
            .IsRequired();

        builder.Property(x => x.ANNID);

        builder.Property(x => x.AllCinemaID);

        builder.Property(x => x.AnisonID);

        builder.Property(x => x.SyoboiID);

        builder.Property(x => x.VNDBID);

        builder.Property(x => x.BangumiID);

        builder.Property(x => x.LainID);

        builder.Property(x => x.Site_EN);

        builder.Property(x => x.Site_JP);

        builder.Property(x => x.Wikipedia_ID);

        builder.Property(x => x.WikipediaJP_ID);

        builder.Property(x => x.CrunchyrollID);

        builder.Property(x => x.FunimationID);

        builder.Property(x => x.HiDiveID);

        builder.Property(x => x.LatestEpisodeNumber);

        builder.Ignore(x => x.IsRestricted);
        builder.Ignore(x => x.AniDBEpisodes);
        builder.Ignore(x => x.TmdbMovieBackdrops);
        builder.Ignore(x => x.TmdbShowBackdrops);
        builder.Ignore(x => x.TmdbEpisodeCrossReferences);
        builder.Ignore(x => x.TmdbSeasonCrossReferences);
        builder.Ignore(x => x.TmdbMovieCrossReferences);
        builder.Ignore(x => x.TmdbShowCrossReferences);
        builder.Ignore(x => x.TmdbMovies);
        builder.Ignore(x => x.TmdbSeasons);
        builder.Ignore(x => x.TmdbShows);
        builder.Ignore(x => x.Tags);
        builder.Ignore(x => x.CustomTags);
        builder.Ignore(x => x.MalCrossReferences);
        builder.Ignore(x => x.AnimeTags);
        builder.Ignore(x => x.Titles);
        builder.Ignore(x => x.RelatedAnime);
        builder.Ignore(x => x.SimilarAnime);
        builder.Ignore(x => x.Characters);
    }
}
