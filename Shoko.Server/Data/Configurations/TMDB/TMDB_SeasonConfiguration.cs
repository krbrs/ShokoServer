using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Models.TMDB;

namespace Shoko.Server.Data.Configurations.TMDB;

/// <summary>
/// EF Core configuration for <see cref="TMDB_Season"/> — TMDB season cache.
/// Based on NHibernate mapping: Shoko.Server/Mappings/TMDB/TMDB_SeasonMap.cs
/// </summary>
public class TMDB_SeasonConfiguration : IEntityTypeConfiguration<TMDB_Season>
{
    public void Configure(EntityTypeBuilder<TMDB_Season> builder)
    {
        builder.ToTable("TMDB_Season");

        builder.HasKey(x => x.TMDB_SeasonID);

        builder.Property(x => x.TMDB_SeasonID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.TmdbShowID)
            .IsRequired();

        builder.Property(x => x.TmdbSeasonID)
            .IsRequired();

        builder.Property(x => x.PosterPath);

        builder.Property(x => x.EnglishTitle)
            .IsRequired();

        builder.Property(x => x.EnglishOverview)
            .IsRequired();

        builder.Property(x => x.EpisodeCount)
            .IsRequired();

        builder.Property(x => x.HiddenEpisodeCount)
            .IsRequired();

        builder.Property(x => x.SeasonNumber)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.LastUpdatedAt)
            .IsRequired();

        builder.Ignore(x => x.TmdbEpisodes);
    }
}
