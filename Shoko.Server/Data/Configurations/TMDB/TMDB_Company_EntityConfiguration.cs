using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Data.Converters;
using Shoko.Server.Models.TMDB;

namespace Shoko.Server.Data.Configurations.TMDB;

/// <summary>
/// EF Core configuration for <see cref="TMDB_Company_Entity"/> — TMDB company-to-entity cross-reference.
/// Based on NHibernate mapping: Shoko.Server/Mappings/TMDB/TMDB_Company_EntityMap.cs
/// </summary>
public class TMDB_Company_EntityConfiguration : IEntityTypeConfiguration<TMDB_Company_Entity>
{
    public void Configure(EntityTypeBuilder<TMDB_Company_Entity> builder)
    {
        builder.ToTable("TMDB_Company_Entity");

        builder.HasKey(x => x.TMDB_Company_EntityID);

        builder.Property(x => x.TMDB_Company_EntityID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.TmdbCompanyID)
            .IsRequired();

        builder.Property(x => x.TmdbEntityType)
            .IsRequired();

        builder.Property(x => x.TmdbEntityID)
            .IsRequired();

        builder.Property(x => x.Ordering)
            .IsRequired();

        builder.Property(x => x.ReleasedAt)
            .HasConversion<DateOnlyConverter>();
    }
}
