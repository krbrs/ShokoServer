using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Models.TMDB;

namespace Shoko.Server.Data.Configurations.TMDB.Optional;

/// <summary>
/// EF Core configuration for <see cref="TMDB_AlternateOrdering_Season"/> — TMDB alternate ordering season.
/// Based on NHibernate mapping: Shoko.Server/Mappings/TMDB/Optional/TMDB_AlternateOrdering_SeasonMap.cs
/// </summary>
public class TMDB_AlternateOrdering_SeasonConfiguration : IEntityTypeConfiguration<TMDB_AlternateOrdering_Season>
{
    public void Configure(EntityTypeBuilder<TMDB_AlternateOrdering_Season> builder)
    {
        builder.ToTable("TMDB_AlternateOrdering_Season");

        builder.HasKey(x => x.TMDB_AlternateOrdering_SeasonID);

        builder.Property(x => x.TMDB_AlternateOrdering_SeasonID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.TmdbShowID)
            .IsRequired();

        builder.Property(x => x.TmdbEpisodeGroupCollectionID)
            .IsRequired();

        builder.Property(x => x.TmdbEpisodeGroupID)
            .IsRequired();

        builder.Property(x => x.EnglishTitle)
            .IsRequired();

        builder.Property(x => x.EpisodeCount)
            .IsRequired();

        builder.Property(x => x.HiddenEpisodeCount)
            .IsRequired();

        builder.Property(x => x.SeasonNumber)
            .IsRequired();

        builder.Property(x => x.IsLocked)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.LastUpdatedAt)
            .IsRequired();

        builder.Ignore(x => x.TmdbAlternateOrderingEpisodes);
    }
}
