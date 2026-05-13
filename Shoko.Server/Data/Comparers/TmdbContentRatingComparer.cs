using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Shoko.Server.Models.TMDB;

namespace Shoko.Server.Data.Comparers;

public sealed class TmdbContentRatingComparer : ValueComparer<List<TMDB_ContentRating>>
{
    public static readonly TmdbContentRatingComparer Instance = new();

    private TmdbContentRatingComparer()
        : base(
            (left, right) => OrderedListComparerHelper.AreEqual(left, right, AreEqual),
            value => OrderedListComparerHelper.GetHashCode(value, GetHashCode),
            value => OrderedListComparerHelper.Snapshot(value, Snapshot))
    {
    }

    private static bool AreEqual(TMDB_ContentRating left, TMDB_ContentRating right)
        => string.Equals(left.CountryCode, right.CountryCode, System.StringComparison.Ordinal)
            && string.Equals(left.Rating, right.Rating, System.StringComparison.Ordinal);

    private static int GetHashCode(TMDB_ContentRating value)
    {
        var hashCode = new System.HashCode();
        hashCode.Add(value.CountryCode);
        hashCode.Add(value.Rating);
        return hashCode.ToHashCode();
    }

    private static TMDB_ContentRating Snapshot(TMDB_ContentRating value)
        => new(value.CountryCode, value.Rating);
}
