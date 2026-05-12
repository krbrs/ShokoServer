using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Models.CrossReference;

namespace Shoko.Server.Data.Configurations;

/// <summary>
/// EF Core configuration for <see cref="CrossRef_AniDB_TraktV2"/> — AniDB ↔ Trakt mapping.
/// Based on NHibernate mapping: Shoko.Server/Mappings/CrossRef_AniDB_TraktV2Map.cs
/// </summary>
public class CrossRef_AniDB_TraktV2Configuration : IEntityTypeConfiguration<CrossRef_AniDB_TraktV2>
{
    public void Configure(EntityTypeBuilder<CrossRef_AniDB_TraktV2> builder)
    {
        builder.ToTable("CrossRef_AniDB_TraktV2");

        builder.HasKey(x => x.CrossRef_AniDB_TraktV2ID);

        builder.Property(x => x.CrossRef_AniDB_TraktV2ID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.AnimeID)
            .IsRequired();

        builder.Property(x => x.CrossRefSource)
            .IsRequired();

        builder.Property(x => x.TraktID);

        builder.Property(x => x.TraktSeasonNumber)
            .IsRequired();

        builder.Property(x => x.AniDBStartEpisodeType)
            .IsRequired();

        builder.Property(x => x.AniDBStartEpisodeNumber)
            .IsRequired();

        builder.Property(x => x.TraktStartEpisodeNumber)
            .IsRequired();

        builder.Property(x => x.TraktTitle);
    }
}
