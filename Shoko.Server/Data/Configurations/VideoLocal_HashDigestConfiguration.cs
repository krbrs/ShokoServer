using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Models.Shoko;

namespace Shoko.Server.Data.Configurations;

/// <summary>
/// EF Core configuration for <see cref="VideoLocal_HashDigest"/> — hash digest storage (ED2K, CRC32, MD5, SHA1).
/// Based on NHibernate mapping: Shoko.Server/Mappings/VideoLocal_HashDigestMap.cs
/// </summary>
public class VideoLocal_HashDigestConfiguration : IEntityTypeConfiguration<VideoLocal_HashDigest>
{
    public void Configure(EntityTypeBuilder<VideoLocal_HashDigest> builder)
    {
        builder.ToTable("VideoLocal_HashDigest");

        builder.HasKey(x => x.VideoLocal_HashDigestID);

        builder.Property(x => x.VideoLocal_HashDigestID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.VideoLocalID)
            .IsRequired();

        builder.Property(x => x.Type)
            .IsRequired();

        builder.Property(x => x.Value)
            .IsRequired();

        builder.Property(x => x.Metadata);
    }
}
