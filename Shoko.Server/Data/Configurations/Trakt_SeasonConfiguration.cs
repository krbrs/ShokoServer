using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Models.Trakt;

namespace Shoko.Server.Data.Configurations;

/// <summary>
/// EF Core configuration for <see cref="Trakt_Season"/> — Trakt season metadata cache.
/// Based on NHibernate mapping: Shoko.Server/Mappings/Trakt_SeasonMap.cs
/// </summary>
public class Trakt_SeasonConfiguration : IEntityTypeConfiguration<Trakt_Season>
{
    public void Configure(EntityTypeBuilder<Trakt_Season> builder)
    {
        builder.HasKey(x => x.Trakt_SeasonID);

        builder.Property(x => x.Trakt_SeasonID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Season)
            .IsRequired();

        builder.Property(x => x.Trakt_ShowID)
            .IsRequired();

        builder.Property(x => x.URL);
    }
}
