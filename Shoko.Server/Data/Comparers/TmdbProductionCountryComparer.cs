#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Shoko.Server.Models.TMDB;

namespace Shoko.Server.Data.Comparers;

/// <summary>
/// EF Core comparer for TMDB production country collections.
///
/// The values are stored as ordered lists in a single string column, so change tracking
/// needs structural equality and a deep snapshot copy.
/// </summary>
public sealed class TmdbProductionCountryComparer : ValueComparer<List<TMDB_ProductionCountry>>
{
    public static readonly TmdbProductionCountryComparer Instance = new();

    private TmdbProductionCountryComparer()
        : base(
            (left, right) => AreEqual(left, right),
            value => CalculateHashCode(value),
            value => CreateSnapshot(value))
    {
    }

    private static bool AreEqual(List<TMDB_ProductionCountry>? left, List<TMDB_ProductionCountry>? right)
    {
        if (ReferenceEquals(left, right))
            return true;

        if (left == null || right == null || left.Count != right.Count)
            return false;

        return left.Zip(right, (l, r) => l.CountryCode == r.CountryCode && l.CountryName == r.CountryName)
            .All(equal => equal);
    }

    private static int CalculateHashCode(List<TMDB_ProductionCountry> value)
    {
        var hashCode = new HashCode();
        foreach (var country in value)
        {
            hashCode.Add(country.CountryCode);
            hashCode.Add(country.CountryName);
        }

        return hashCode.ToHashCode();
    }

    private static List<TMDB_ProductionCountry> CreateSnapshot(List<TMDB_ProductionCountry> value)
        => value.Select(country => new TMDB_ProductionCountry(country.CountryCode, country.CountryName)).ToList();
}
