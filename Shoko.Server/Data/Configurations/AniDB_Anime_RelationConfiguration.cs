using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Abstractions.Metadata;
using Shoko.Server.Models.AniDB;

namespace Shoko.Server.Data.Configurations;

/// <summary>
/// EF Core configuration for <see cref="AniDB_Anime_Relation"/> — anime relationships.
/// Based on NHibernate mapping: Shoko.Server/Mappings/AniDB_Anime_RelationMap.cs
/// </summary>
public class AniDB_Anime_RelationConfiguration : IEntityTypeConfiguration<AniDB_Anime_Relation>
{
    public void Configure(EntityTypeBuilder<AniDB_Anime_Relation> builder)
    {
        builder.ToTable("AniDB_Anime_Relation");

        builder.HasKey(x => x.AniDB_Anime_RelationID);

        builder.Property(x => x.AniDB_Anime_RelationID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.AnimeID)
            .IsRequired();

        builder.Property(x => x.RelatedAnimeID)
            .IsRequired();

        builder.Property(x => x.RelationType)
            .IsRequired();

        builder.Ignore("Base");

        builder.Ignore("Related");
    }
}
