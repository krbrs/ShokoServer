using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Shoko.Server.Data.Converters;

/// <summary>
/// EF Core ValueConverter&lt;DateOnly, int&gt; that maps DateOnly to/from integer days since Unix epoch.
/// Matches NHibernate DateOnlyConverter behavior of storing DateOnly as an integer in the database.
/// </summary>
public class DateOnlyConverter : ValueConverter<DateOnly, int>
{
    public DateOnlyConverter()
        : base(v => ToInt(v), i => FromInt(i))
    {
    }

    private static int ToInt(DateOnly value)
    {
        var dt = new DateTime(value.Year, value.Month, value.Day);
        return (int)dt.Subtract(DateTime.UnixEpoch).TotalDays;
    }

    private static DateOnly FromInt(int value)
    {
        return DateOnly.FromDateTime(DateTime.UnixEpoch.AddDays(value));
    }
}
