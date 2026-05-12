using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Models.Shoko;

namespace Shoko.Server.Data.Configurations;

/// <summary>
/// EF Core configuration for <see cref="CustomTag"/> — user-defined custom tags.
/// Based on NHibernate mapping: Shoko.Server/Mappings/CustomTagMap.cs
/// </summary>
public class CustomTagConfiguration : IEntityTypeConfiguration<CustomTag>
{
    public void Configure(EntityTypeBuilder<CustomTag> builder)
    {
        builder.HasKey(x => x.CustomTagID);

        builder.Property(x => x.CustomTagID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.TagName);

        builder.Property(x => x.TagDescription);
    }
}
