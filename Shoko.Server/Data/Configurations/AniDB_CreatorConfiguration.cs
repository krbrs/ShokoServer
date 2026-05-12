using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Models.AniDB;

namespace Shoko.Server.Data.Configurations;

/// <summary>
/// EF Core configuration for <see cref="AniDB_Creator"/> — creator/studio definitions.
/// Based on NHibernate mapping: Shoko.Server/Mappings/AniDB_CreatorMap.cs
/// </summary>
public class AniDB_CreatorConfiguration : IEntityTypeConfiguration<AniDB_Creator>
{
    public void Configure(EntityTypeBuilder<AniDB_Creator> builder)
    {
        builder.ToTable("AniDB_Creator");

        builder.HasKey(x => x.AniDB_CreatorID);

        builder.Property(x => x.AniDB_CreatorID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.CreatorID)
            .IsRequired();

        builder.Property(x => x.Name)
            .IsRequired();

        builder.Property(x => x.OriginalName);

        builder.Property(x => x.Type)
            .IsRequired();

        builder.Property(x => x.ImagePath);

        builder.Property(x => x.EnglishHomepageUrl);

        builder.Property(x => x.JapaneseHomepageUrl);

        builder.Property(x => x.EnglishWikiUrl);

        builder.Property(x => x.JapaneseWikiUrl);

        builder.Property(x => x.LastUpdatedAt)
            .IsRequired();

        builder.Ignore(x => x.Characters);

        builder.Ignore(x => x.Staff);
    }
}
