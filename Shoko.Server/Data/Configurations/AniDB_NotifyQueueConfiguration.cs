using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Models.AniDB;

namespace Shoko.Server.Data.Configurations;

/// <summary>
/// EF Core configuration for <see cref="AniDB_NotifyQueue"/> — AniDB notification staging.
/// Based on NHibernate mapping: Shoko.Server/Mappings/AniDB_NotifyQueueMap.cs
/// </summary>
public class AniDB_NotifyQueueConfiguration : IEntityTypeConfiguration<AniDB_NotifyQueue>
{
    public void Configure(EntityTypeBuilder<AniDB_NotifyQueue> builder)
    {
        builder.ToTable("AniDB_NotifyQueue");

        builder.HasKey(x => x.AniDB_NotifyQueueID);

        builder.Property(x => x.AniDB_NotifyQueueID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Type)
            .IsRequired();

        builder.Property(x => x.ID)
            .IsRequired();

        builder.Property(x => x.AddedAt)
            .IsRequired();
    }
}
