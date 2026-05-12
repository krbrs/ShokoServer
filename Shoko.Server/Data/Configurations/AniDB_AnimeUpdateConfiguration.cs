using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Models.AniDB;

namespace Shoko.Server.Data.Configurations;

/// <summary>
/// EF Core configuration for <see cref="AniDB_AnimeUpdate"/> — last update timestamp per anime.
/// Based on NHibernate mapping: Shoko.Server/Mappings/AniDB_AnimeUpdateMap.cs
/// </summary>
public class AniDB_AnimeUpdateConfiguration : IEntityTypeConfiguration<AniDB_AnimeUpdate>
{
    public void Configure(EntityTypeBuilder<AniDB_AnimeUpdate> builder)
    {
        builder.ToTable("AniDB_AnimeUpdate");

        builder.HasKey(x => x.AniDB_AnimeUpdateID);

        builder.Property(x => x.AniDB_AnimeUpdateID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.AnimeID)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired();
    }
}
