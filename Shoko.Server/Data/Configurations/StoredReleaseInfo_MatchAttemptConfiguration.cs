using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Models.Release;

namespace Shoko.Server.Data.Configurations;

/// <summary>
/// EF Core configuration for <see cref="StoredReleaseInfo_MatchAttempt"/> — release match attempt log.
/// Based on NHibernate mapping: Shoko.Server/Mappings/StoredReleaseInfo_MatchAttemptMap.cs
/// </summary>
public class StoredReleaseInfo_MatchAttemptConfiguration : IEntityTypeConfiguration<StoredReleaseInfo_MatchAttempt>
{
    public void Configure(EntityTypeBuilder<StoredReleaseInfo_MatchAttempt> builder)
    {
        builder.ToTable("StoredReleaseInfo_MatchAttempt");

        builder.HasKey(x => x.StoredReleaseInfo_MatchAttemptID);

        builder.Property(x => x.StoredReleaseInfo_MatchAttemptID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.ProviderName);

        builder.Property(x => x.ProviderID);

        builder.Property(x => x.ED2K)
            .IsRequired();

        builder.Property(x => x.FileSize)
            .IsRequired();

        builder.Property(x => x.EmbeddedAttemptProviderNames)
            .HasColumnName("AttemptProviderNames")
            .IsRequired();

        builder.Property(x => x.AttemptStartedAt)
            .IsRequired();

        builder.Property(x => x.AttemptEndedAt)
            .IsRequired();

        builder.Ignore(x => x.AttemptedProviderNames);
    }
}
