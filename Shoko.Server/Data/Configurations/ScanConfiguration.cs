using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Models.Legacy;
using Shoko.Server.Server;

namespace Shoko.Server.Data.Configurations;

/// <summary>
/// EF Core configuration for <see cref="Scan"/> — scan tracking.
/// Based on NHibernate mapping: Shoko.Server/Mappings/ScanMap.cs
/// </summary>
public class ScanConfiguration : IEntityTypeConfiguration<Scan>
{
    public void Configure(EntityTypeBuilder<Scan> builder)
    {
        builder.ToTable("Scan");

        builder.HasKey(x => x.ScanID);

        builder.Property(x => x.ScanID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.CreationTIme)
            .HasColumnName("CreationTime")
            .IsRequired();

        builder.Property(x => x.ImportFolders)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<int>();
    }
}
