using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Models.TMDB;

namespace Shoko.Server.Data.Configurations.TMDB.Optional;

/// <summary>
/// EF Core configuration for <see cref="TMDB_AlternateOrdering_Episode"/> — TMDB alternate ordering episode.
/// Based on NHibernate mapping: Shoko.Server/Mappings/TMDB/Optional/TMDB_AlternateOrdering_EpisodeMap.cs
/// </summary>
public class TMDB_AlternateOrdering_EpisodeConfiguration : IEntityTypeConfiguration<TMDB_AlternateOrdering_Episode>
{
    public void Configure(EntityTypeBuilder<TMDB_AlternateOrdering_Episode> builder)
    {
        builder.ToTable("TMDB_AlternateOrdering_Episode");

        builder.HasKey(x => x.TMDB_AlternateOrdering_EpisodeID);

        builder.Property(x => x.TMDB_AlternateOrdering_EpisodeID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.TmdbShowID)
            .IsRequired();

        builder.Property(x => x.TmdbEpisodeGroupCollectionID)
            .IsRequired();

        builder.Property(x => x.TmdbEpisodeGroupID)
            .IsRequired();

        builder.Property(x => x.TmdbEpisodeID)
            .IsRequired();

        builder.Property(x => x.SeasonNumber)
            .IsRequired();

        builder.Property(x => x.EpisodeNumber)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.LastUpdatedAt)
            .IsRequired();
    }
}
