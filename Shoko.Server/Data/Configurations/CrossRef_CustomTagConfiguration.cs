using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Models.CrossReference;

namespace Shoko.Server.Data.Configurations;

/// <summary>
/// EF Core configuration for <see cref="CrossRef_CustomTag"/> — custom tag cross-reference.
/// Based on NHibernate mapping: Shoko.Server/Mappings/CrossRef_CustomTagMap.cs
/// </summary>
public class CrossRef_CustomTagConfiguration : IEntityTypeConfiguration<CrossRef_CustomTag>
{
    public void Configure(EntityTypeBuilder<CrossRef_CustomTag> builder)
    {
        builder.HasKey(x => x.CrossRef_CustomTagID);

        builder.Property(x => x.CrossRef_CustomTagID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.CustomTagID)
            .IsRequired();

        builder.Property(x => x.CrossRefID)
            .IsRequired();

        builder.Property(x => x.CrossRefType)
            .IsRequired();
    }
}
