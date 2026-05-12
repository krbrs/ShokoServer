using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Models.Shoko;

namespace Shoko.Server.Data.Configurations;

/// <summary>
/// EF Core configuration for <see cref="AnimeGroup_User"/> — user custom tags for groups.
/// Based on NHibernate mapping: Shoko.Server/Mappings/AnimeGroup_UserMap.cs
/// </summary>
public class AnimeGroup_UserConfiguration : IEntityTypeConfiguration<AnimeGroup_User>
{
    public void Configure(EntityTypeBuilder<AnimeGroup_User> builder)
    {
        builder.ToTable("AnimeGroup_User");

        builder.HasKey(x => x.AnimeGroup_UserID);

        builder.Property(x => x.AnimeGroup_UserID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.JMMUserID);

        builder.Property(x => x.AnimeGroupID);

        builder.Property(x => x.PlayedCount)
            .IsRequired();

        builder.Property(x => x.StoppedCount)
            .IsRequired();

        builder.Property(x => x.UnwatchedEpisodeCount)
            .IsRequired();

        builder.Property(x => x.WatchedCount)
            .IsRequired();

        builder.Property(x => x.WatchedDate);

        builder.Property(x => x.WatchedEpisodeCount);
    }
}
