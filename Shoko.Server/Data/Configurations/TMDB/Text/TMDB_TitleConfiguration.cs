using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Models.TMDB;
using Shoko.Server.Server;

namespace Shoko.Server.Data.Configurations.TMDB.Text;

/// <summary>
/// EF Core configuration for <see cref="TMDB_Title"/> — TMDB multi-language title cache.
/// Based on NHibernate mapping: Shoko.Server/Mappings/TMDB/Text/TMDB_TitleMap.cs
/// </summary>
public class TMDB_TitleConfiguration : IEntityTypeConfiguration<TMDB_Title>
{
    public void Configure(EntityTypeBuilder<TMDB_Title> builder)
    {
        builder.ToTable("TMDB_Title");

        builder.HasKey(x => x.TMDB_TitleID);

        builder.Property(x => x.TMDB_TitleID)
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
