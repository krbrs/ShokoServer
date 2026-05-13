using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Data.Comparers;
using Shoko.Server.Data.Converters;
using Shoko.Server.Models.Shoko;

namespace Shoko.Server.Data.Configurations;

/// <summary>
/// EF Core configuration for <see cref="AnimeEpisode_User"/> — user watch data for episodes.
/// Based on NHibernate mapping: Shoko.Server/Mappings/AnimeEpisode_UserMap.cs
/// </summary>
public class AnimeEpisode_UserConfiguration : IEntityTypeConfiguration<AnimeEpisode_User>
{
    public void Configure(EntityTypeBuilder<AnimeEpisode_User> builder)
    {
        builder.ToTable("AnimeEpisode_User");

        builder.HasKey(x => x.AnimeEpisode_UserID);

        builder.Property(x => x.AnimeEpisode_UserID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.AnimeEpisodeID)
            .IsRequired();

        builder.Property(x => x.AnimeSeriesID)
            .IsRequired();

        builder.Property(x => x.JMMUserID)
            .IsRequired();

        builder.Property(x => x.PlayedCount)
            .IsRequired();

        builder.Property(x => x.StoppedCount)
            .IsRequired();

        builder.Property(x => x.WatchedCount)
            .IsRequired();

        builder.Property(x => x.WatchedDate);

        builder.Property(x => x.IsFavorite)
            .IsRequired();

        builder.Property(x => x.AbsoluteUserRating);

        builder.Property(x => x.UserTags)
            .IsRequired()
            .HasConversion(new StringListConverter());
        builder.Property(x => x.UserTags)
            .Metadata.SetValueComparer(StringListComparer.Instance);

        builder.Property(x => x.LastUpdated)
            .IsRequired();

        builder.Ignore(x => x.UserRating);
    }
}
