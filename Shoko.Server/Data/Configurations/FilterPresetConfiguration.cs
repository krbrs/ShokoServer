using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shoko.Server.Data.Converters;
using Shoko.Server.Models.Shoko;
using Shoko.Server.Server;

namespace Shoko.Server.Data.Configurations;

/// <summary>
/// EF Core configuration for <see cref="FilterPreset"/> — filter expressions with self-referential parent.
/// Based on NHibernate mapping: Shoko.Server/Mappings/FilterPresetMap.cs
/// </summary>
public class FilterPresetConfiguration : IEntityTypeConfiguration<FilterPreset>
{
    public void Configure(EntityTypeBuilder<FilterPreset> builder)
    {
        builder.ToTable("FilterPreset");

        builder.HasKey(x => x.FilterPresetID);

        builder.Property(x => x.FilterPresetID)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.ParentFilterPresetID);

        builder.Property(x => x.Name)
            .IsRequired();

        builder.Property(x => x.FilterType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.Locked)
            .IsRequired();

        builder.Property(x => x.Hidden)
            .IsRequired();

        builder.Property(x => x.ApplyAtSeriesLevel)
            .IsRequired();

        builder.Property(x => x.Expression)
            .HasConversion(new FilterExpressionConverter());

        builder.Property(x => x.SortingExpression)
            .HasConversion(new SortingExpressionConverter());

        builder.Ignore(x => x.Parent);
        builder.Ignore(x => x.Children);
    }
}
