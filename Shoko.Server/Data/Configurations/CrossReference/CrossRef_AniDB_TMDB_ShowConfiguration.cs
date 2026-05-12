using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Abstractions.Metadata.Enums;
using Shoko.Server.Models.CrossReference;

namespace Shoko.Server.Data.Configurations;

/// <summary>
/// EF Core configuration for <see cref="CrossRef_AniDB_TMDB_Show"/> — AniDB ↔ TMDB show mapping.
/// Based on NHibernate mapping: Shoko.Server/Mappings/CrossReference/CrossRef_AniDB_TMDB_ShowMap.cs
/// </summary>
public class CrossRef_AniDB_TMDB_ShowConfiguration : IEntityTypeConfiguration<CrossRef_AniDB_TMDB_Show>
{
    public void Configure(EntityTypeBuilder<CrossRef_AniDB_TMDB_Show> builder)
    {
        builder.ToTable("CrossRef_AniDB_TMDB_Show");

        builder.HasKey(x => x.CrossRef_AniDB_TMDB_ShowID);

        builder.Property(x => x.CrossRef_AniDB_TMDB_ShowID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.AnidbAnimeID)
            .IsRequired();

        builder.Property(x => x.TmdbShowID)
            .IsRequired();

        builder.Property(x => x.MatchRating)
            .HasConversion<byte>()
            .IsRequired();
    }
}
