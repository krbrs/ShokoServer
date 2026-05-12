using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Data.Converters;
using Shoko.Server.Models.Internal;

namespace Shoko.Server.Data.Configurations;

/// <summary>
/// EF Core configuration for <see cref="Versions"/> — database version tracking.
/// Based on NHibernate mapping: Shoko.Server/Mappings/VersionsMap.cs
/// </summary>
public class VersionsConfiguration : IEntityTypeConfiguration<Versions>
{
    public void Configure(EntityTypeBuilder<Versions> builder)
    {
        builder.ToTable("Versions");

        builder.HasKey(x => x.VersionsID);

        builder.Property(x => x.VersionsID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.VersionType)
            .IsRequired();

        builder.Property(x => x.VersionValue)
            .IsRequired();

        builder.Property(x => x.VersionRevision);

        builder.Property(x => x.VersionCommand);

        builder.Property(x => x.VersionProgram);
    }
}
