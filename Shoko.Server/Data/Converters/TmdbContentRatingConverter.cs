using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Shoko.Server.Models.TMDB;

namespace Shoko.Server.Data.Converters;

/// <summary>
/// EF Core ValueConverter&lt;List&lt;TMDB_ContentRating&gt;, string&gt; that serializes
/// TMDB_ContentRating lists to pipe-delimited strings.
/// Format: "CountryCode,Rating|CountryCode,Rating|..."
/// Preserves exact format from NHibernate TmdbContentRatingConverter.
/// </summary>
public class TmdbContentRatingConverter : ValueConverter<List<TMDB_ContentRating>, string>
{
    public TmdbContentRatingConverter()
        : base(v => ToString(v), s => FromString(s))
    {
    }

    private static string ToString(List<TMDB_ContentRating> value)
    {
        if (value == null || value.Count == 0) return string.Empty;
        return string.Join('|', value.Select(r => r.ToString()));
    }

    private static List<TMDB_ContentRating> FromString(string value)
    {
        if (string.IsNullOrEmpty(value)) return new List<TMDB_ContentRating>();
        return value.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(TMDB_ContentRating.FromString)
            .ToList();
    }
}
