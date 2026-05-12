using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Models.TMDB;

namespace Shoko.Server.Data.Configurations.TMDB;

/// <summary>
/// EF Core configuration for <see cref="TMDB_Company"/> — TMDB production company cache.
/// Based on NHibernate mapping: Shoko.Server/Mappings/TMDB/TMDB_CompanyMap.cs
/// </summary>
public class TMDB_CompanyConfiguration : IEntityTypeConfiguration<TMDB_Company>
{
    public void Configure(EntityTypeBuilder<TMDB_Company> builder)
    {
        builder.ToTable("TMDB_Company");

        builder.HasKey(x => x.TMDB_CompanyID);

        builder.Property(x => x.TMDB_CompanyID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.TmdbCompanyID)
            .IsRequired();

        builder.Property(x => x.Name)
            .IsRequired();

        builder.Property(x => x.CountryOfOrigin)
            .IsRequired();
    }
}
