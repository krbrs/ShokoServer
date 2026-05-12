using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Abstractions.Metadata.Enums;
using Shoko.Server.Models.AniDB;

namespace Shoko.Server.Data.Configurations;

/// <summary>
/// EF Core configuration for <see cref="AniDB_Episode_PreferredImage"/> — preferred episode images.
/// Based on NHibernate mapping: Shoko.Server/Mappings/AniDB_Episode_PreferredImageMap.cs
/// </summary>
public class AniDB_Episode_PreferredImageConfiguration : IEntityTypeConfiguration<AniDB_Episode_PreferredImage>
{
    public void Configure(EntityTypeBuilder<AniDB_Episode_PreferredImage> builder)
    {
        builder.ToTable("AniDB_Episode_PreferredImage");

        builder.HasKey(x => x.AniDB_Episode_PreferredImageID);

        builder.Property(x => x.AniDB_Episode_PreferredImageID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.AnidbAnimeID)
            .IsRequired();

        builder.Property(x => x.AnidbEpisodeID)
            .IsRequired();

        builder.Property(x => x.ImageID)
            .IsRequired();

        builder.Property(x => x.ImageSource)
            .IsRequired()
            .HasConversion<byte>();

        builder.Property(x => x.ImageType)
            .IsRequired()
            .HasConversion<byte>();
    }
}
