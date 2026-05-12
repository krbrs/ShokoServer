using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Models.AniDB;

namespace Shoko.Server.Data.Configurations;

/// <summary>
/// EF Core configuration for <see cref="AniDB_Anime_Staff"/> — staff credits for anime.
/// Based on NHibernate mapping: Shoko.Server/Mappings/AniDB_Anime_StaffMap.cs
/// </summary>
public class AniDB_Anime_StaffConfiguration : IEntityTypeConfiguration<AniDB_Anime_Staff>
{
    public void Configure(EntityTypeBuilder<AniDB_Anime_Staff> builder)
    {
        builder.ToTable("AniDB_Anime_Staff");

        builder.HasKey(x => x.AniDB_Anime_StaffID);

        builder.Property(x => x.AniDB_Anime_StaffID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.AnimeID)
            .IsRequired();

        builder.Property(x => x.CreatorID)
            .IsRequired();

        builder.Property(x => x.RoleType)
            .IsRequired();

        builder.Property(x => x.Role)
            .IsRequired();

        builder.Property(x => x.Ordering)
            .IsRequired();
    }
}
