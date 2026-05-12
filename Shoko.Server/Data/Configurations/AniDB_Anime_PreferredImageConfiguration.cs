using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Abstractions.Metadata.Enums;
using Shoko.Server.Models.AniDB;

namespace Shoko.Server.Data.Configurations;

/// <summary>
/// EF Core configuration for <see cref="AniDB_Anime_PreferredImage"/> — preferred poster/backdrop images.
/// Based on NHibernate mapping: Shoko.Server/Mappings/AniDB_Anime_PreferredImageMap.cs
/// </summary>
public class AniDB_Anime_PreferredImageConfiguration : IEntityTypeConfiguration<AniDB_Anime_PreferredImage>
{
    public void Configure(EntityTypeBuilder<AniDB_Anime_PreferredImage> builder)
    {
        builder.ToTable("AniDB_Anime_PreferredImage");

        builder.HasKey(x => x.AniDB_Anime_PreferredImageID);

        builder.Property(x => x.AniDB_Anime_PreferredImageID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.AnidbAnimeID)
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
