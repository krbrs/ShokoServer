using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Models.AniDB;

namespace Shoko.Server.Data.Configurations;

/// <summary>
/// EF Core configuration for <see cref="AniDB_Anime_Tag"/> — anime tag associations.
/// Based on NHibernate mapping: Shoko.Server/Mappings/AniDB_Anime_TagMap.cs
/// </summary>
public class AniDB_Anime_TagConfiguration : IEntityTypeConfiguration<AniDB_Anime_Tag>
{
    public void Configure(EntityTypeBuilder<AniDB_Anime_Tag> builder)
    {
        builder.ToTable("AniDB_Anime_Tag");

        builder.HasKey(x => x.AniDB_Anime_TagID);

        builder.Property(x => x.AniDB_Anime_TagID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.AnimeID)
            .IsRequired();

        builder.Property(x => x.TagID)
            .IsRequired();

        builder.Property(x => x.LocalSpoiler)
            .IsRequired();

        builder.Property(x => x.Weight)
            .IsRequired();
    }
}
