using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Models.TMDB;
using Shoko.Server.Providers.TMDB;

namespace Shoko.Server.Data.Configurations.TMDB.Optional;

/// <summary>
/// EF Core configuration for <see cref="TMDB_AlternateOrdering"/> — TMDB alternate episode ordering (Episode Groups).
/// Based on NHibernate mapping: Shoko.Server/Mappings/TMDB/Optional/TMDB_AlternateOrderingMap.cs
/// </summary>
public class TMDB_AlternateOrderingConfiguration : IEntityTypeConfiguration<TMDB_AlternateOrdering>
{
    public void Configure(EntityTypeBuilder<TMDB_AlternateOrdering> builder)
    {
        builder.ToTable("TMDB_AlternateOrdering");

        builder.HasKey(x => x.TMDB_AlternateOrderingID);

        builder.Property(x => x.TMDB_AlternateOrderingID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.TmdbShowID)
            .IsRequired();

        builder.Property(x => x.TmdbNetworkID);

        builder.Property(x => x.TmdbEpisodeGroupCollectionID)
            .IsRequired();

        builder.Property(x => x.EnglishTitle)
            .IsRequired();

        builder.Property(x => x.EnglishOverview)
            .IsRequired();

        builder.Property(x => x.EpisodeCount)
            .IsRequired();

        builder.Property(x => x.HiddenEpisodeCount)
            .IsRequired();

        builder.Property(x => x.SeasonCount)
            .IsRequired();

        builder.Property(x => x.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.LastUpdatedAt)
            .IsRequired();

        builder.Ignore(x => x.TmdbAlternateOrderingSeasons);
        builder.Ignore(x => x.TmdbAlternateOrderingEpisodes);
    }
}
