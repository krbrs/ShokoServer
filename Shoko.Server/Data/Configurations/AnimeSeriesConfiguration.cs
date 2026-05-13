using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Models.Shoko;

namespace Shoko.Server.Data.Configurations;

/// <summary>
/// EF Core configuration for <see cref="AnimeSeries"/> — Shoko series wrapper around AniDB anime.
/// Based on NHibernate mapping: Shoko.Server/Mappings/AnimeSeriesMap.cs
/// </summary>
public class AnimeSeriesConfiguration : IEntityTypeConfiguration<AnimeSeries>
{
    public void Configure(EntityTypeBuilder<AnimeSeries> builder)
    {
        builder.ToTable("AnimeSeries");

        builder.HasKey(x => x.AnimeSeriesID);

        builder.Property(x => x.AnimeSeriesID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.AniDB_ID)
            .IsRequired();

        builder.Property(x => x.AnimeGroupID)
            .IsRequired();

        builder.Property(x => x.DateTimeCreated)
            .IsRequired();

        builder.Property(x => x.DateTimeUpdated)
            .IsRequired();

        builder.Property(x => x.DefaultAudioLanguage);

        builder.Property(x => x.DefaultSubtitleLanguage);

        builder.Property(x => x.LatestLocalEpisodeNumber)
            .IsRequired();

        builder.Property(x => x.EpisodeAddedDate);

        builder.Property(x => x.LatestEpisodeAirDate);

        builder.Property(x => x.MissingEpisodeCount)
            .IsRequired();

        builder.Property(x => x.MissingEpisodeCountGroups)
            .IsRequired();

        builder.Property(x => x.HiddenMissingEpisodeCount)
            .IsRequired();

        builder.Property(x => x.HiddenMissingEpisodeCountGroups)
            .IsRequired();

        builder.Property(x => x.SeriesNameOverride);

        builder.Property(x => x.AirsOn);

        builder.Property(x => x.UpdatedAt)
            .IsRequired();

        builder.Property(x => x.DisableAutoMatchFlags)
            .IsRequired();

        builder.Ignore(x => x.IsTMDBAutoMatchingDisabled);
        builder.Ignore(x => x.IsMALAutoMatchingDisabled);
        builder.Ignore(x => x.IsAniListAutoMatchingDisabled);
        builder.Ignore(x => x.IsAnimeshonAutoMatchingDisabled);
        builder.Ignore(x => x.IsKitsuAutoMatchingDisabled);
        builder.Ignore(x => x.AnimeGroup);
        builder.Ignore(x => x.TopLevelAnimeGroup);
        builder.Ignore(x => x.AllAnimeEpisodes);
        builder.Ignore(x => x.AnimeEpisodes);
        builder.Ignore(x => x.AllGroupsAbove);
        builder.Ignore(x => x.TmdbEpisodeCrossReferences);
        builder.Ignore(x => x.TmdbSeasonCrossReferences);
        builder.Ignore(x => x.TmdbMovieCrossReferences);
        builder.Ignore(x => x.TmdbShowCrossReferences);
        builder.Ignore(x => x.FileCrossReferences);
        builder.Ignore(x => x.VideoLocals);
        builder.Ignore(x => x.TmdbMovies);
        builder.Ignore(x => x.TmdbSeasons);
        builder.Ignore(x => x.TmdbShows);
        builder.Ignore(x => x.MalCrossReferences);
    }
}
