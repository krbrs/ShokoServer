using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Data.Converters;
using Shoko.Server.MediaInfo;
using Shoko.Server.Models.Shoko;

namespace Shoko.Server.Data.Configurations;

/// <summary>
/// EF Core configuration for <see cref="VideoLocal"/> — canonical video file record.
/// Based on NHibernate mapping: Shoko.Server/Mappings/VideoLocalMap.cs
/// </summary>
public class VideoLocalConfiguration : IEntityTypeConfiguration<VideoLocal>
{
    public void Configure(EntityTypeBuilder<VideoLocal> builder)
    {
        builder.ToTable("VideoLocal");

        builder.HasKey(x => x.VideoLocalID);

        builder.Property(x => x.VideoLocalID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.DateTimeUpdated)
            .IsRequired();

        builder.Property(x => x.DateTimeCreated)
            .IsRequired();

        builder.Property(x => x.DateTimeImported);

#pragma warning disable CS0618
        builder.Property(x => x.FileName)
            .IsRequired();
#pragma warning restore CS0618

        builder.Property(x => x.FileSize)
            .IsRequired();

        builder.Property(x => x.Hash)
            .IsRequired();

        builder.Property(x => x.HashSource)
            .IsRequired();

        builder.Property(x => x.IsIgnored)
            .IsRequired();

        builder.Property(x => x.IsVariation)
            .IsRequired();

        builder.Property(x => x.MediaVersion)
            .IsRequired();

        builder.Property(x => x.MediaInfo)
            .HasColumnName("MediaBlob")
            .HasConversion(new MessagePackConverter<MediaContainer>());

        builder.Property(x => x.MyListID)
            .IsRequired();

        builder.Property(x => x.LastAVDumped);

        builder.Property(x => x.LastAVDumpVersion);

        builder.Ignore(x => x.AnimeEpisodes);
        builder.Ignore(x => x.EpisodeCrossReferences);
        builder.Ignore(x => x.Hashes);
        builder.Ignore(x => x.Places);
    }
}
