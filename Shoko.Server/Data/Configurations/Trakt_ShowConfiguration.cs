using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Models.Trakt;

namespace Shoko.Server.Data.Configurations;

/// <summary>
/// EF Core configuration for <see cref="Trakt_Show"/> — Trakt show metadata cache.
/// Based on NHibernate mapping: Shoko.Server/Mappings/Trakt_ShowMap.cs
/// </summary>
public class Trakt_ShowConfiguration : IEntityTypeConfiguration<Trakt_Show>
{
    public void Configure(EntityTypeBuilder<Trakt_Show> builder)
    {
        builder.HasKey(x => x.Trakt_ShowID);

        builder.Property(x => x.Trakt_ShowID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.TraktID);

        builder.Property(x => x.TmdbShowID);

        builder.Property(x => x.Title);

        builder.Property(x => x.Year);

        builder.Property(x => x.URL);

        builder.Property(x => x.Overview);
    }
}
