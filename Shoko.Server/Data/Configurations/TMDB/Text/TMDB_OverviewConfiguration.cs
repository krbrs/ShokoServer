using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Models.TMDB;
using Shoko.Server.Server;

namespace Shoko.Server.Data.Configurations.TMDB.Text;

/// <summary>
/// EF Core configuration for <see cref="TMDB_Overview"/> — TMDB multi-language overview cache.
/// Based on NHibernate mapping: Shoko.Server/Mappings/TMDB/Text/TMDB_OverviewMap.cs
/// </summary>
public class TMDB_OverviewConfiguration : IEntityTypeConfiguration<TMDB_Overview>
{
    public void Configure(EntityTypeBuilder<TMDB_Overview> builder)
    {
        builder.ToTable("TMDB_Overview");

        builder.HasKey(x => x.TMDB_OverviewID);

        builder.Property(x => x.TMDB_OverviewID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.ParentID)
            .IsRequired();

        builder.Property(x => x.ParentType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.LanguageCode)
            .IsRequired();

        builder.Property(x => x.CountryCode)
            .IsRequired();

        builder.Property(x => x.Value)
            .IsRequired();
    }
}
