using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Models.TMDB;

namespace Shoko.Server.Data.Configurations.TMDB;

/// <summary>
/// EF Core configuration for <see cref="TMDB_Movie_Cast"/> — TMDB movie cast join table.
/// Based on NHibernate mapping: Shoko.Server/Mappings/TMDB/TMDB_Movie_CastMap.cs
/// </summary>
public class TMDB_Movie_CastConfiguration : IEntityTypeConfiguration<TMDB_Movie_Cast>
{
    public void Configure(EntityTypeBuilder<TMDB_Movie_Cast> builder)
    {
        builder.ToTable("TMDB_Movie_Cast");

        builder.HasKey(x => x.TMDB_Movie_CastID);

        builder.Property(x => x.TMDB_Movie_CastID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.TmdbMovieID)
            .IsRequired();

        builder.Property(x => x.TmdbPersonID)
            .IsRequired();

        builder.Property(x => x.TmdbCreditID)
            .IsRequired();

        builder.Property(x => x.CharacterName)
            .IsRequired();

        builder.Property(x => x.Ordering)
            .IsRequired();
    }
}
