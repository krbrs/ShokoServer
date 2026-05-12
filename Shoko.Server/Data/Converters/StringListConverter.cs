using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Shoko.Server.Data.Converters;

/// <summary>
/// EF Core ValueConverter&lt;List&lt;string&gt;, string&gt; that serializes string lists
/// to triple-pipe delimited strings.
/// Format: "item1|||item2|||item3"
/// Preserves exact format from NHibernate StringListConverter.
/// </summary>
public class StringListConverter : ValueConverter<List<string>, string>
{
    public StringListConverter()
        : base(v => ToString(v), s => FromString(s))
    {
    }

    private static string ToString(List<string> value)
    {
        if (value == null || value.Count == 0) return string.Empty;
        return string.Join("|||", value);
    }

    private static List<string> FromString(string value)
    {
        if (string.IsNullOrEmpty(value)) return new List<string>();
        return value.Split("|||", StringSplitOptions.RemoveEmptyEntries).ToList();
    }
}
