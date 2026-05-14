using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Data.Converters;
using Shoko.Server.Models.TMDB;

namespace Shoko.Server.Data.Configurations.TMDB;

/// <summary>
/// EF Core configuration for <see cref="TMDB_Episode"/> — TMDB episode cache.
/// Based on NHibernate mapping: Shoko.Server/Mappings/TMDB/TMDB_EpisodeMap.cs
/// </summary>
public class TMDB_EpisodeConfiguration : IEntityTypeConfiguration<TMDB_Episode>
{
    public void Configure(EntityTypeBuilder<TMDB_Episode> builder)
    {
        builder.ToTable("TMDB_Episode");

        builder.HasKey(x => x.TMDB_EpisodeID);

        builder.Property(x => x.TMDB_EpisodeID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.TmdbShowID)
            .IsRequired();

        builder.Property(x => x.TmdbSeasonID)
            .IsRequired();

        builder.Property(x => x.TmdbEpisodeID)
            .IsRequired();

        builder.Property(x => x.TvdbEpisodeID);

        builder.Property(x => x.ThumbnailPath)
            .IsRequired(false);

        builder.Property(x => x.EnglishTitle)
            .IsRequired();

        builder.Property(x => x.EnglishOverview)
            .IsRequired();

        builder.Property(x => x.IsHidden)
            .IsRequired();

        builder.Property(x => x.SeasonNumber)
            .IsRequired();

        builder.Property(x => x.EpisodeNumber)
            .IsRequired();

        builder.Property(x => x.RuntimeMinutes)
            .HasColumnName("Runtime");

        builder.Property(x => x.UserRating)
            .IsRequired();

        builder.Property(x => x.UserVotes)
            .IsRequired();

        builder.Property(x => x.AiredAt)
            .HasConversion<DateOnlyConverter>();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.LastUpdatedAt)
            .IsRequired();

        builder.Ignore(x => x.CrossReferences);
        builder.Ignore(x => x.FileCrossReferences);
        builder.Ignore(x => x.TmdbAlternateOrderingEpisodes);
        builder.Ignore(x => x.Cast);
        builder.Ignore(x => x.Crew);
        builder.Ignore(x => x.Runtime);
    }
}
