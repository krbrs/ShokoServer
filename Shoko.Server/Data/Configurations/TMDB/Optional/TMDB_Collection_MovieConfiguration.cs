using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Models.TMDB;

namespace Shoko.Server.Data.Configurations.TMDB.Optional;

/// <summary>
/// EF Core configuration for <see cref="TMDB_Collection_Movie"/> — TMDB collection-to-movie join table.
/// Based on NHibernate mapping: Shoko.Server/Mappings/TMDB/Optional/TMDB_Collection_MovieMap.cs
/// </summary>
public class TMDB_Collection_MovieConfiguration : IEntityTypeConfiguration<TMDB_Collection_Movie>
{
    public void Configure(EntityTypeBuilder<TMDB_Collection_Movie> builder)
    {
        builder.ToTable("TMDB_Collection_Movie");

        builder.HasKey(x => x.TMDB_Collection_MovieID);

        builder.Property(x => x.TMDB_Collection_MovieID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.TmdbCollectionID)
            .IsRequired();

        builder.Property(x => x.TmdbMovieID)
            .IsRequired();

        builder.Property(x => x.Ordering)
            .IsRequired();
    }
}
