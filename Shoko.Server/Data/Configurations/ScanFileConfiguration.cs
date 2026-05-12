using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Models.Legacy;
using Shoko.Server.Server;

namespace Shoko.Server.Data.Configurations;

/// <summary>
/// EF Core configuration for <see cref="ScanFile"/> — scan file tracking.
/// Based on NHibernate mapping: Shoko.Server/Mappings/ScanFileMap.cs
/// </summary>
public class ScanFileConfiguration : IEntityTypeConfiguration<ScanFile>
{
    public void Configure(EntityTypeBuilder<ScanFile> builder)
    {
        builder.ToTable("ScanFile");

        builder.HasKey(x => x.ScanFileID);

        builder.Property(x => x.ScanFileID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.ScanID)
            .IsRequired();

        builder.Property(x => x.ImportFolderID)
            .IsRequired();

        builder.Property(x => x.VideoLocal_Place_ID)
            .IsRequired();

        builder.Property(x => x.FullName)
            .IsRequired();

        builder.Property(x => x.FileSize)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.CheckDate);

        builder.Property(x => x.Hash)
            .IsRequired();

        builder.Property(x => x.HashResult);
    }
}
