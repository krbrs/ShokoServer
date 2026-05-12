using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Models.CrossReference;

namespace Shoko.Server.Data.Configurations;

/// <summary>
/// EF Core configuration for <see cref="CrossRef_AniDB_MAL"/> — AniDB ↔ MyAnimeList mapping.
/// Based on NHibernate mapping: Shoko.Server/Mappings/CrossRef_AniDB_MALMap.cs
/// </summary>
public class CrossRef_AniDB_MALConfiguration : IEntityTypeConfiguration<CrossRef_AniDB_MAL>
{
    public void Configure(EntityTypeBuilder<CrossRef_AniDB_MAL> builder)
    {
        builder.HasKey(x => x.CrossRef_AniDB_MALID);

        builder.Property(x => x.CrossRef_AniDB_MALID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.AnimeID)
            .IsRequired();

        builder.Property(x => x.MALID)
            .IsRequired();
    }
}
