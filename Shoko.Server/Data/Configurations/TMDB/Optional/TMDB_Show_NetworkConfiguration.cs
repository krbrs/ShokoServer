using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Models.TMDB;

namespace Shoko.Server.Data.Configurations.TMDB.Optional;

/// <summary>
/// EF Core configuration for <see cref="TMDB_Show_Network"/> — TMDB show-to-network join table.
/// Based on NHibernate mapping: Shoko.Server/Mappings/TMDB/Optional/TMDB_Show_NetworkMap.cs
/// </summary>
public class TMDB_Show_NetworkConfiguration : IEntityTypeConfiguration<TMDB_Show_Network>
{
    public void Configure(EntityTypeBuilder<TMDB_Show_Network> builder)
    {
        builder.ToTable("TMDB_Show_Network");

        builder.HasKey(x => x.TMDB_Show_NetworkID);

        builder.Property(x => x.TMDB_Show_NetworkID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.TmdbShowID)
            .IsRequired();

        builder.Property(x => x.TmdbNetworkID)
            .IsRequired();

        builder.Property(x => x.Ordering)
            .IsRequired();
    }
}
