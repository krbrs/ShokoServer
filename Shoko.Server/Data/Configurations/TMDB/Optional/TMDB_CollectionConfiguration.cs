using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Models.TMDB;

namespace Shoko.Server.Data.Configurations.TMDB.Optional;

/// <summary>
/// EF Core configuration for <see cref="TMDB_Collection"/> — TMDB movie collection cache.
/// Based on NHibernate mapping: Shoko.Server/Mappings/TMDB/Optional/TMDB_CollectionMap.cs
/// </summary>
public class TMDB_CollectionConfiguration : IEntityTypeConfiguration<TMDB_Collection>
{
    public void Configure(EntityTypeBuilder<TMDB_Collection> builder)
    {
        builder.ToTable("TMDB_Collection");

        builder.HasKey(x => x.TMDB_CollectionID);

        builder.Property(x => x.TMDB_CollectionID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.TmdbCollectionID)
            .IsRequired();

        builder.Property(x => x.EnglishTitle)
            .IsRequired();

        builder.Property(x => x.EnglishOverview)
            .IsRequired();

        builder.Property(x => x.MovieCount)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.LastUpdatedAt)
            .IsRequired();
    }
}
