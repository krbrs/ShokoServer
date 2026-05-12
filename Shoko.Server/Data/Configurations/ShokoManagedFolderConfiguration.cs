using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Models.Shoko;

namespace Shoko.Server.Data.Configurations;

/// <summary>
/// EF Core configuration for <see cref="ShokoManagedFolder"/> — import folders.
/// Based on NHibernate mapping: Shoko.Server/Mappings/ShokoManagedFolderMap.cs
/// </summary>
public class ShokoManagedFolderConfiguration : IEntityTypeConfiguration<ShokoManagedFolder>
{
    public void Configure(EntityTypeBuilder<ShokoManagedFolder> builder)
    {
        builder.ToTable("ImportFolder");

        builder.HasKey(x => x.ID);

        builder.Property(x => x.ID)
            .ValueGeneratedOnAdd()
            .HasColumnName("ImportFolderID");

        builder.Property(x => x.Path)
            .HasColumnName("ImportFolderLocation")
            .IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("ImportFolderName")
            .IsRequired();

        builder.Property(x => x.IsDropDestination)
            .IsRequired();

        builder.Property(x => x.IsDropSource)
            .IsRequired();

        builder.Property(x => x.IsWatched)
            .IsRequired();

        builder.Ignore(x => x.DropFolderType);
        builder.Ignore(x => x.Places);
    }
}
