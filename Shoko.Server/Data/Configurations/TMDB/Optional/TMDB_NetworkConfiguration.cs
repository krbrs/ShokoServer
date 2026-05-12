using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Models.TMDB;

namespace Shoko.Server.Data.Configurations.TMDB.Optional;

/// <summary>
/// EF Core configuration for <see cref="TMDB_Network"/> — TMDB production network cache.
/// Based on NHibernate mapping: Shoko.Server/Mappings/TMDB/Optional/TMDB_NetworkMap.cs
/// </summary>
public class TMDB_NetworkConfiguration : IEntityTypeConfiguration<TMDB_Network>
{
    public void Configure(EntityTypeBuilder<TMDB_Network> builder)
    {
        builder.ToTable("TMDB_Network");

        builder.HasKey(x => x.TMDB_NetworkID);

        builder.Property(x => x.TMDB_NetworkID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.TmdbNetworkID)
            .IsRequired();

        builder.Property(x => x.Name)
            .IsRequired();

        builder.Property(x => x.CountryOfOrigin)
            .IsRequired();

        builder.Property(x => x.LastOrphanedAt);
    }
}
