using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Shoko.Server.Data.Comparers;

public sealed class StringListComparer : ValueComparer<List<string>>
{
    public static readonly StringListComparer Instance = new();

    private StringListComparer()
        : base(
            (left, right) => OrderedListComparerHelper.AreEqual(left, right, (a, b) => string.Equals(a, b, System.StringComparison.Ordinal)),
            value => OrderedListComparerHelper.GetHashCode(value, item => item == null ? 0 : item.GetHashCode()),
            value => OrderedListComparerHelper.Snapshot(value, item => item))
    {
    }
}
