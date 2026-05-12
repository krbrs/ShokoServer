using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Models.Shoko;

namespace Shoko.Server.Data.Configurations;

/// <summary>
/// EF Core configuration for <see cref="AnimeEpisode"/> — Shoko episode wrapper around AniDB episode.
/// Based on NHibernate mapping: Shoko.Server/Mappings/AnimeEpisodeMap.cs
/// </summary>
public class AnimeEpisodeConfiguration : IEntityTypeConfiguration<AnimeEpisode>
{
    public void Configure(EntityTypeBuilder<AnimeEpisode> builder)
    {
        builder.ToTable("AnimeEpisode");

        builder.HasKey(x => x.AnimeEpisodeID);

        builder.Property(x => x.AnimeEpisodeID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.AniDB_EpisodeID)
            .IsRequired();

        builder.Property(x => x.AnimeSeriesID)
            .IsRequired();

        builder.Property(x => x.DateTimeCreated)
            .IsRequired();

        builder.Property(x => x.DateTimeUpdated)
            .IsRequired();

        builder.Property(x => x.IsHidden)
            .IsRequired();

        builder.Property(x => x.EpisodeNameOverride);

        builder.Ignore(x => x.TmdbEpisodeCrossReferences);
        builder.Ignore(x => x.TmdbMovieCrossReferences);
        builder.Ignore(x => x.FileCrossReferences);
        builder.Ignore(x => x.TmdbEpisodes);
        builder.Ignore(x => x.TmdbMovies);
        builder.Ignore(x => x.VideoLocals);
    }
}
