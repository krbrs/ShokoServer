using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Models.Shoko;

namespace Shoko.Server.Data.Configurations;

/// <summary>
/// EF Core configuration for <see cref="JMMUser"/> — users with password hash and admin flags.
/// Based on NHibernate mapping: Shoko.Server/Mappings/JMMUserMap.cs
/// </summary>
public class JMMUserConfiguration : IEntityTypeConfiguration<JMMUser>
{
    public void Configure(EntityTypeBuilder<JMMUser> builder)
    {
        builder.ToTable("JMMUser");

        builder.HasKey(x => x.JMMUserID);

        builder.Property(x => x.JMMUserID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.HideCategories);

        builder.Property(x => x.IsAniDBUser)
            .IsRequired();

        builder.Property(x => x.IsTraktUser)
            .IsRequired();

        builder.Property(x => x.IsAdmin)
            .IsRequired();

        builder.Property(x => x.Password);

        builder.Property(x => x.Username);

        builder.Property(x => x.CanEditServerSettings);

        builder.Property(x => x.PlexUsers);

        builder.Property(x => x.PlexToken);

        builder.Property(x => x.AvatarImageBlob);

        builder.Property(x => x.RawAvatarImageMetadata)
            .HasColumnName("AvatarImageMetadata");
    }
}
