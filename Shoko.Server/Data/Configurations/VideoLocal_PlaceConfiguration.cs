using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Models.Shoko;

namespace Shoko.Server.Data.Configurations;

/// <summary>
/// EF Core configuration for <see cref="VideoLocal_Place"/> — physical file location.
/// Based on NHibernate mapping: Shoko.Server/Mappings/VideoLocal_PlaceMap.cs
/// </summary>
public class VideoLocal_PlaceConfiguration : IEntityTypeConfiguration<VideoLocal_Place>
{
    public void Configure(EntityTypeBuilder<VideoLocal_Place> builder)
    {
        builder.ToTable("VideoLocal_Place");

        builder.HasKey(x => x.ID);

        builder.Property(x => x.ID)
            .ValueGeneratedOnAdd()
            .HasColumnName("VideoLocal_Place_ID");

        builder.Property(x => x.VideoID)
            .HasColumnName("VideoLocalID")
            .IsRequired();

        builder.Property(x => x.ManagedFolderID)
            .HasColumnName("ImportFolderID")
            .IsRequired();

        builder.Property(x => x.RelativePath)
            .HasColumnName("FilePath")
            .IsRequired();
    }
}
