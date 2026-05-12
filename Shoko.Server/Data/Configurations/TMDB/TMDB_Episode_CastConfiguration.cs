using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Models.TMDB;

namespace Shoko.Server.Data.Configurations.TMDB;

/// <summary>
/// EF Core configuration for <see cref="TMDB_Episode_Cast"/> — TMDB episode cast join table.
/// Based on NHibernate mapping: Shoko.Server/Mappings/TMDB/TMDB_Episode_CastMap.cs
/// </summary>
public class TMDB_Episode_CastConfiguration : IEntityTypeConfiguration<TMDB_Episode_Cast>
{
    public void Configure(EntityTypeBuilder<TMDB_Episode_Cast> builder)
    {
        builder.ToTable("TMDB_Episode_Cast");

        builder.HasKey(x => x.TMDB_Episode_CastID);

        builder.Property(x => x.TMDB_Episode_CastID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.TmdbShowID)
            .IsRequired();

        builder.Property(x => x.TmdbSeasonID)
            .IsRequired();

        builder.Property(x => x.TmdbEpisodeID)
            .IsRequired();

        builder.Property(x => x.TmdbPersonID)
            .IsRequired();

        builder.Property(x => x.TmdbCreditID)
            .IsRequired();

        builder.Property(x => x.CharacterName)
            .IsRequired();

        builder.Property(x => x.IsGuestRole)
            .IsRequired();

        builder.Property(x => x.Ordering)
            .IsRequired();
    }
}
