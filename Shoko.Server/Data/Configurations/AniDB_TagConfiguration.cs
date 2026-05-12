using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Models.AniDB;

namespace Shoko.Server.Data.Configurations;

/// <summary>
/// EF Core configuration for <see cref="AniDB_Tag"/> — AniDB tag definitions.
/// Based on NHibernate mapping: Shoko.Server/Mappings/AniDB_TagMap.cs
/// </summary>
public class AniDB_TagConfiguration : IEntityTypeConfiguration<AniDB_Tag>
{
    public void Configure(EntityTypeBuilder<AniDB_Tag> builder)
    {
        builder.ToTable("AniDB_Tag");

        builder.HasKey(x => x.AniDB_TagID);

        builder.Property(x => x.AniDB_TagID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.TagID)
            .IsRequired();

        builder.Property(x => x.ParentTagID);

        builder.Property(x => x.TagNameSource)
            .HasColumnName("TagName")
            .IsRequired();

        builder.Property(x => x.TagNameOverride);

        builder.Property(x => x.TagDescription)
            .IsRequired();

        builder.Property(x => x.GlobalSpoiler)
            .IsRequired();

        builder.Property(x => x.Verified)
            .IsRequired();

        builder.Property(x => x.LastUpdated);
    }
}
