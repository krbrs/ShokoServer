using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Models.Shoko;

namespace Shoko.Server.Data.Configurations;

/// <summary>
/// EF Core configuration for <see cref="FileNameHash"/> — filename to ED2K hash cache.
/// Based on NHibernate mapping: Shoko.Server/Mappings/FileNameHashMap.cs
/// </summary>
public class FileNameHashConfiguration : IEntityTypeConfiguration<FileNameHash>
{
    public void Configure(EntityTypeBuilder<FileNameHash> builder)
    {
        builder.ToTable("FileNameHash");

        builder.HasKey(x => x.FileNameHashID);

        builder.Property(x => x.FileNameHashID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Hash);

        builder.Property(x => x.FileName);

        builder.Property(x => x.FileSize)
            .IsRequired();

        builder.Property(x => x.DateTimeUpdated)
            .IsRequired();
    }
}
