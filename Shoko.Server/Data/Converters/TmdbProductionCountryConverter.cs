using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Shoko.Server.Models.TMDB;

namespace Shoko.Server.Data.Converters;

/// <summary>
/// EF Core ValueConverter&lt;List&lt;TMDB_ProductionCountry&gt;, string&gt; that serializes
/// TMDB_ProductionCountry lists to pipe-delimited strings.
/// Format: "CountryCode,CountryName|CountryCode,CountryName|..."
/// Preserves exact format from NHibernate TmdbProductionCountryConverter.
/// </summary>
public class TmdbProductionCountryConverter : ValueConverter<List<TMDB_ProductionCountry>, string>
{
    public TmdbProductionCountryConverter()
        : base(v => ToString(v), s => FromString(s))
    {
    }

    private static string ToString(List<TMDB_ProductionCountry> value)
    {
        if (value == null || value.Count == 0) return string.Empty;
        return string.Join('|', value.Select(r => r.ToString()));
    }

    private static List<TMDB_ProductionCountry> FromString(string value)
    {
        if (string.IsNullOrEmpty(value)) return new List<TMDB_ProductionCountry>();
        return value.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(TMDB_ProductionCountry.FromString)
            .ToList();
    }
}
