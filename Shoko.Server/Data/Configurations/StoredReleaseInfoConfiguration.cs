using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Data.Converters;
using Shoko.Server.Models.Release;
using Shoko.Abstractions.Video.Enums;

namespace Shoko.Server.Data.Configurations;

/// <summary>
/// EF Core configuration for <see cref="StoredReleaseInfo"/> — release provider cache.
/// Based on NHibernate mapping: Shoko.Server/Mappings/StoredReleaseInfoMap.cs
/// </summary>
public class StoredReleaseInfoConfiguration : IEntityTypeConfiguration<StoredReleaseInfo>
{
    public void Configure(EntityTypeBuilder<StoredReleaseInfo> builder)
    {
        builder.ToTable("StoredReleaseInfo");

        builder.HasKey(x => x.StoredReleaseInfoID);

        builder.Property(x => x.StoredReleaseInfoID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.ED2K)
            .IsRequired();

        builder.Property(x => x.FileSize)
            .IsRequired();

        builder.Property(x => x.ID);

        builder.Property(x => x.ProviderName)
            .IsRequired();

        builder.Property(x => x.ReleaseURI);

        builder.Property(x => x.Version)
            .IsRequired();

        builder.Property(x => x.ProvidedFileSize);

        builder.Property(x => x.Comment);

        builder.Property(x => x.OriginalFilename);

        builder.Property(x => x.IsCensored);

        builder.Property(x => x.IsChaptered);

        builder.Property(x => x.IsCreditless);

        builder.Property(x => x.IsCorrupted)
            .IsRequired();

        builder.Property(x => x.Source)
            .IsRequired();

        builder.Property(x => x.GroupID);

        builder.Property(x => x.GroupSource);

        builder.Property(x => x.GroupName);

        builder.Property(x => x.GroupShortName);

        builder.Property(x => x.EmbeddedHashes)
            .HasColumnName("Hashes");

        builder.Property(x => x.EmbeddedAudioLanguages)
            .HasColumnName("AudioLanguages");

        builder.Property(x => x.EmbeddedSubtitleLanguages)
            .HasColumnName("SubtitleLanguages");

        builder.Property(x => x.EmbeddedCrossReferences)
            .HasColumnName("CrossReferences")
            .IsRequired();

        builder.Property(x => x.ReleasedAt)
            .HasConversion<DateOnlyConverter>();

        builder.Property(x => x.LastUpdatedAt)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Ignore(x => x.AudioLanguages);
        builder.Ignore(x => x.SubtitleLanguages);
        builder.Ignore(x => x.CrossReferences);
    }
}
