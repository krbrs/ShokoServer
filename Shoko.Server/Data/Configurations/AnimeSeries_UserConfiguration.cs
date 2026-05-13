using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Data.Comparers;
using Shoko.Server.Data.Converters;
using Shoko.Server.Models.Shoko;

namespace Shoko.Server.Data.Configurations;

/// <summary>
/// EF Core configuration for <see cref="AnimeSeries_User"/> — user ratings for series.
/// Based on NHibernate mapping: Shoko.Server/Mappings/AnimeSeries_UserMap.cs
/// </summary>
public class AnimeSeries_UserConfiguration : IEntityTypeConfiguration<AnimeSeries_User>
{
    public void Configure(EntityTypeBuilder<AnimeSeries_User> builder)
    {
        builder.ToTable("AnimeSeries_User");

        builder.HasKey(x => x.AnimeSeries_UserID);

        builder.Property(x => x.AnimeSeries_UserID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.JMMUserID)
            .IsRequired();

        builder.Property(x => x.AnimeSeriesID)
            .IsRequired();

        builder.Property(x => x.PlayedCount)
            .IsRequired();

        builder.Property(x => x.StoppedCount)
            .IsRequired();

        builder.Property(x => x.UnwatchedEpisodeCount)
            .IsRequired();

        builder.Property(x => x.WatchedCount)
            .IsRequired();

        builder.Property(x => x.WatchedDate);

        builder.Property(x => x.WatchedEpisodeCount)
            .IsRequired();

        builder.Property(x => x.LastEpisodeUpdate);

        builder.Property(x => x.LastVideoUpdate);

        builder.Property(x => x.HiddenUnwatchedEpisodeCount)
            .IsRequired();

        builder.Property(x => x.IsFavorite)
            .IsRequired();

        builder.Property(x => x.AbsoluteUserRating);

        builder.Property(x => x.UserRatingVoteType);

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
