using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Data.Converters;
using Shoko.Server.Models.TMDB;

namespace Shoko.Server.Data.Configurations.TMDB;

/// <summary>
/// EF Core configuration for <see cref="TMDB_Image_Entity"/> — TMDB image-to-entity cross-reference.
/// Based on NHibernate mapping: Shoko.Server/Mappings/TMDB/TMDB_Image_EntityMap.cs
/// </summary>
public class TMDB_Image_EntityConfiguration : IEntityTypeConfiguration<TMDB_Image_Entity>
{
    public void Configure(EntityTypeBuilder<TMDB_Image_Entity> builder)
    {
        builder.ToTable("TMDB_Image_Entity");

        builder.HasKey(x => x.TMDB_Image_EntityID);

        builder.Property(x => x.TMDB_Image_EntityID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.RemoteFileName)
            .IsRequired();

        builder.Property(x => x.ImageType)
            .IsRequired()
            .HasConversion<byte>();

        builder.Property(x => x.TmdbEntityType)
            .IsRequired();

        builder.Property(x => x.TmdbEntityID)
            .IsRequired();

        builder.Property(x => x.Ordering)
            .IsRequired();

        builder.Property(x => x.ReleasedAt)
            .HasConversion<DateOnlyConverter>();
    }
}
