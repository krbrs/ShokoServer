using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Models.Trakt;

namespace Shoko.Server.Data.Configurations;

/// <summary>
/// EF Core configuration for <see cref="Trakt_Episode"/> — Trakt episode metadata cache.
/// Based on NHibernate mapping: Shoko.Server/Mappings/Trakt_EpisodeMap.cs
/// </summary>
public class Trakt_EpisodeConfiguration : IEntityTypeConfiguration<Trakt_Episode>
{
    public void Configure(EntityTypeBuilder<Trakt_Episode> builder)
    {
        builder.HasKey(x => x.Trakt_EpisodeID);

        builder.Property(x => x.Trakt_EpisodeID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Trakt_ShowID)
            .IsRequired();

        builder.Property(x => x.EpisodeNumber);

        builder.Property(x => x.Overview)
            ;

        builder.Property(x => x.Season)
            .IsRequired();

        builder.Property(x => x.Title);

        builder.Property(x => x.URL);

        builder.Property(x => x.TraktID);
    }
}
