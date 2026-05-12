using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Models.Internal;

namespace Shoko.Server.Data.Configurations;

/// <summary>
/// EF Core configuration for <see cref="ScheduledUpdate"/> — periodic task timestamps.
/// Based on NHibernate mapping: Shoko.Server/Mappings/ScheduledUpdateMap.cs
/// </summary>
public class ScheduledUpdateConfiguration : IEntityTypeConfiguration<ScheduledUpdate>
{
    public void Configure(EntityTypeBuilder<ScheduledUpdate> builder)
    {
        builder.HasKey(x => x.ScheduledUpdateID);

        builder.Property(x => x.ScheduledUpdateID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.LastUpdate)
            .IsRequired();

        builder.Property(x => x.UpdateDetails);

        builder.Property(x => x.UpdateType)
            .IsRequired();
    }
}
