using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Models.Shoko;

namespace Shoko.Server.Data.Configurations;

/// <summary>
/// EF Core configuration for <see cref="VideoLocal_User"/> — per-user watch data for video files.
/// Based on NHibernate mapping: Shoko.Server/Mappings/VideoLocal_UserMap.cs
/// </summary>
public class VideoLocal_UserConfiguration : IEntityTypeConfiguration<VideoLocal_User>
{
    public void Configure(EntityTypeBuilder<VideoLocal_User> builder)
    {
        builder.ToTable("VideoLocal_User");

        builder.HasKey(x => x.VideoLocal_UserID);

        builder.Property(x => x.VideoLocal_UserID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.JMMUserID)
            .IsRequired();

        builder.Property(x => x.VideoLocalID)
            .IsRequired();

        builder.Property(x => x.WatchedDate);

        builder.Property(x => x.WatchedCount)
            .IsRequired();

        builder.Property(x => x.ResumePosition)
            .IsRequired();

        builder.Property(x => x.LastUpdated)
            .IsRequired();

        builder.Ignore(x => x.ProgressPosition);
    }
}
