using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Models.Shoko;

namespace Shoko.Server.Data.Configurations;

/// <summary>
/// EF Core configuration for <see cref="StoredRelocationPipe"/> — relocation script configurations.
/// Based on NHibernate mapping: Shoko.Server/Mappings/StoredRelocationPipeMap.cs
/// </summary>
public class StoredRelocationPipeConfiguration : IEntityTypeConfiguration<StoredRelocationPipe>
{
    public void Configure(EntityTypeBuilder<StoredRelocationPipe> builder)
    {
        builder.ToTable("StoredRelocationPipe");

        builder.HasKey(x => x.StoredRelocationPipeID);

        builder.Property(x => x.StoredRelocationPipeID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.ProviderID)
            .IsRequired();

        builder.Property(x => x.Name)
            .IsRequired();

        builder.Property(x => x.Configuration);
    }
}
