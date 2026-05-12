using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Models.AniDB;

namespace Shoko.Server.Data.Configurations;

/// <summary>
/// EF Core configuration for <see cref="AniDB_Anime_Character_Creator"/> — character to creator/studio mappings.
/// Based on NHibernate mapping: Shoko.Server/Mappings/AniDB_Anime_Character_CreatorMap.cs
/// </summary>
public class AniDB_Anime_Character_CreatorConfiguration : IEntityTypeConfiguration<AniDB_Anime_Character_Creator>
{
    public void Configure(EntityTypeBuilder<AniDB_Anime_Character_Creator> builder)
    {
        builder.ToTable("AniDB_Anime_Character_Creator");

        builder.HasKey(x => x.AniDB_Anime_Character_CreatorID);

        builder.Property(x => x.AniDB_Anime_Character_CreatorID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.AnimeID)
            .IsRequired();

        builder.Property(x => x.CharacterID)
            .IsRequired();

        builder.Property(x => x.CreatorID)
            .IsRequired();

        builder.Property(x => x.Ordering)
            .IsRequired();

        builder.Ignore(x => x.CharacterCrossReference);

        builder.Ignore(x => x.Creator);
    }
}
