using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Models.AniDB;

namespace Shoko.Server.Data.Configurations;

/// <summary>
/// EF Core configuration for <see cref="AniDB_Anime_Similar"/> — similar anime relationships.
/// Based on NHibernate mapping: Shoko.Server/Mappings/AniDB_Anime_SimilarMap.cs
/// </summary>
public class AniDB_Anime_SimilarConfiguration : IEntityTypeConfiguration<AniDB_Anime_Similar>
{
    public void Configure(EntityTypeBuilder<AniDB_Anime_Similar> builder)
    {
        builder.ToTable("AniDB_Anime_Similar");

        builder.HasKey(x => x.AniDB_Anime_SimilarID);

        builder.Property(x => x.AniDB_Anime_SimilarID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.AnimeID)
            .IsRequired();

        builder.Property(x => x.SimilarAnimeID)
            .IsRequired();

        builder.Property(x => x.Approval)
            .IsRequired();

        builder.Property(x => x.Total)
            .IsRequired();
    }
}
