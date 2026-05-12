using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Shoko.Server.Data.Converters;

/// <summary>
/// Maps nullable <see cref="DateOnly"/> values to nullable <see cref="DateTime"/> values.
/// Use this for legacy schemas that physically store date-only values in DATE/DATETIME columns.
/// </summary>
public class NullableDateOnlyDateTimeConverter : ValueConverter<DateOnly?, DateTime?>
{
    public NullableDateOnlyDateTimeConverter()
        : base(
            value => value.HasValue ? value.Value.ToDateTime(TimeOnly.MinValue) : null,
            value => value.HasValue ? DateOnly.FromDateTime(value.Value) : null)
    {
    }
}
