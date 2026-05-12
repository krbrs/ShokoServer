using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Models.AniDB;

namespace Shoko.Server.Data.Configurations;

/// <summary>
/// EF Core configuration for <see cref="AniDB_Character"/> — character definitions.
/// Based on NHibernate mapping: Shoko.Server/Mappings/AniDB_CharacterMap.cs
/// </summary>
public class AniDB_CharacterConfiguration : IEntityTypeConfiguration<AniDB_Character>
{
    public void Configure(EntityTypeBuilder<AniDB_Character> builder)
    {
        builder.ToTable("AniDB_Character");

        builder.HasKey(x => x.AniDB_CharacterID);

        builder.Property(x => x.AniDB_CharacterID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.CharacterID)
            .IsRequired();

        builder.Property(x => x.Name)
            .IsRequired();

        builder.Property(x => x.OriginalName)
            .IsRequired();

        builder.Property(x => x.Description)
            .IsRequired();

        builder.Property(x => x.ImagePath)
            .IsRequired();

        builder.Property(x => x.Gender)
            .IsRequired();

        builder.Property(x => x.Type)
            .IsRequired();

        builder.Property(x => x.LastUpdated)
            .IsRequired();
    }
}
