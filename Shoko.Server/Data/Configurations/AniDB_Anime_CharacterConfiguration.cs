using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Models.AniDB;

namespace Shoko.Server.Data.Configurations;

/// <summary>
/// EF Core configuration for <see cref="AniDB_Anime_Character"/> — character casting for anime.
/// Based on NHibernate mapping: Shoko.Server/Mappings/AniDB_Anime_CharacterMap.cs
/// </summary>
public class AniDB_Anime_CharacterConfiguration : IEntityTypeConfiguration<AniDB_Anime_Character>
{
    public void Configure(EntityTypeBuilder<AniDB_Anime_Character> builder)
    {
        builder.ToTable("AniDB_Anime_Character");

        builder.HasKey(x => x.AniDB_Anime_CharacterID);

        builder.Property(x => x.AniDB_Anime_CharacterID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.AnimeID)
            .IsRequired();

        builder.Property(x => x.CharacterID)
            .IsRequired();

        builder.Property(x => x.Appearance)
            .IsRequired();

        builder.Property(x => x.AppearanceType)
            .IsRequired();

        builder.Property(x => x.Ordering)
            .IsRequired();

        builder.Ignore(x => x.Anime);

        builder.Ignore(x => x.Creators);
        builder.Ignore(x => x.CreatorCrossReferences);
    }
}
