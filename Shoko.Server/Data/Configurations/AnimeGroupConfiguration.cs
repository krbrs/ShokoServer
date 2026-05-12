using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Models.Shoko;

namespace Shoko.Server.Data.Configurations;

/// <summary>
/// EF Core configuration for <see cref="AnimeGroup"/> — self-referential group container.
/// Based on NHibernate mapping: Shoko.Server/Mappings/AnimeGroupMap.cs
/// </summary>
public class AnimeGroupConfiguration : IEntityTypeConfiguration<AnimeGroup>
{
    public void Configure(EntityTypeBuilder<AnimeGroup> builder)
    {
        builder.ToTable("AnimeGroup");

        builder.HasKey(x => x.AnimeGroupID);

        builder.Property(x => x.AnimeGroupID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.AnimeGroupParentID);

        builder.Property(x => x.DefaultAnimeSeriesID);

        builder.Property(x => x.MainAniDBAnimeID);

        builder.Property(x => x.DateTimeCreated)
            .IsRequired();

        builder.Property(x => x.DateTimeUpdated)
            .IsRequired();

        builder.Property(x => x.Description);

        builder.Property(x => x.GroupName);

        builder.Property(x => x.IsManuallyNamed)
            .IsRequired();

        builder.Property(x => x.OverrideDescription)
            .IsRequired();

        builder.Property(x => x.EpisodeAddedDate);

        builder.Property(x => x.LatestEpisodeAirDate);

        builder.Property(x => x.MissingEpisodeCount)
            .IsRequired();

        builder.Property(x => x.MissingEpisodeCountGroups)
            .IsRequired();

        builder.Ignore(x => x.AllSeries);
        builder.Ignore(x => x.MainSeries);
        builder.Ignore(x => x.AllGroupsAbove);
        builder.Ignore(x => x.Series);
        builder.Ignore(x => x.Children);
        builder.Ignore(x => x.AllChildren);
        builder.Ignore(x => x.Anime);
        builder.Ignore(x => x.Tags);
        builder.Ignore(x => x.CustomTags);
        builder.Ignore(x => x.Titles);
    }
}
