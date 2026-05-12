using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Abstractions.Metadata.Enums;
using Shoko.Server.Models.CrossReference;

namespace Shoko.Server.Data.Configurations;

/// <summary>
/// EF Core configuration for <see cref="CrossRef_AniDB_TMDB_Episode"/> — AniDB ↔ TMDB episode mapping.
/// Based on NHibernate mapping: Shoko.Server/Mappings/CrossReference/CrossRef_AniDB_TMDB_EpisodeMap.cs
/// </summary>
public class CrossRef_AniDB_TMDB_EpisodeConfiguration : IEntityTypeConfiguration<CrossRef_AniDB_TMDB_Episode>
{
    public void Configure(EntityTypeBuilder<CrossRef_AniDB_TMDB_Episode> builder)
    {
        builder.ToTable("CrossRef_AniDB_TMDB_Episode");

        builder.HasKey(x => x.CrossRef_AniDB_TMDB_EpisodeID);

        builder.Property(x => x.CrossRef_AniDB_TMDB_EpisodeID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.AnidbAnimeID)
            .IsRequired();

        builder.Property(x => x.AnidbEpisodeID)
            .IsRequired();

        builder.Property(x => x.TmdbShowID)
            .IsRequired();

        builder.Property(x => x.TmdbEpisodeID)
            .IsRequired();

        builder.Property(x => x.Ordering)
            .IsRequired();

        builder.Property(x => x.MatchRating)
            .HasConversion<byte>()
            .IsRequired();
    }
}
