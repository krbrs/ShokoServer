using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Shoko.Abstractions.Extensions;
using Shoko.Abstractions.Metadata.Enums;

namespace Shoko.Server.Data.Converters;

/// <summary>
/// EF Core ValueConverter&lt;TitleType, string&gt; that serializes TitleType to/from
/// lowercase string representation using the same format as NHibernate TitleTypeConverter.
/// </summary>
public class TitleTypeConverter : ValueConverter<TitleType, string>
{
    public TitleTypeConverter()
        : base(v => ToString(v), s => FromString(s))
    {
    }

    private static string ToString(TitleType value)
    {
        return value.GetString();
    }

    private static TitleType FromString(string value)
    {
        if (value == null) return TitleType.None;
        return value.GetTitleType();
    }
}
