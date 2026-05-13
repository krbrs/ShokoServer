using System;
using System.ComponentModel;
using ShokoDateOnlyConverter = Shoko.Server.Databases.NHibernate.DateOnlyConverter;
using Xunit;

namespace Shoko.Tests;

public class DateOnlyConverterTests
{
    private readonly ShokoDateOnlyConverter _converter = new();

    [Fact]
    public void ConvertFrom_LegacyDateString_ReturnsDateOnly()
    {
        var result = _converter.ConvertFrom("2021-08-31");

        Assert.Equal(new DateOnly(2021, 8, 31), Assert.IsType<DateOnly>(result));
    }

    [Fact]
    public void ConvertFrom_NumericDayNumber_ReturnsDateOnly()
    {
        var result = _converter.ConvertFrom(18881);

        Assert.Equal(DateOnly.FromDayNumber(18881), Assert.IsType<DateOnly>(result));
    }

    [Fact]
    public void ConvertFrom_NumericDayNumberString_ReturnsDateOnly()
    {
        var result = _converter.ConvertFrom("18881");

        Assert.Equal(DateOnly.FromDayNumber(18881), Assert.IsType<DateOnly>(result));
    }

    [Fact]
    public void ConvertFrom_DateTime_ReturnsDateOnly()
    {
        var result = _converter.ConvertFrom(new DateTime(2021, 8, 31, 13, 45, 0, DateTimeKind.Utc));

        Assert.Equal(new DateOnly(2021, 8, 31), Assert.IsType<DateOnly>(result));
    }

    [Fact]
    public void ConvertFrom_EmptyString_ReturnsNull()
    {
        var result = _converter.ConvertFrom(string.Empty);

        Assert.Null(result);
    }

    [Fact]
    public void ConvertFrom_Null_ReturnsNull()
    {
        var result = _converter.ConvertFrom(null);

        Assert.Null(result);
    }

    [Fact]
    public void ConvertTo_DateOnly_ReturnsDateTime()
    {
        var result = _converter.ConvertTo(null, null, new DateOnly(2021, 8, 31), typeof(DateTime));

        Assert.Equal(new DateTime(2021, 8, 31), Assert.IsType<DateTime>(result));
    }
}
