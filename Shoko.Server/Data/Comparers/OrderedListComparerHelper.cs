#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shoko.Server.Data.Comparers;

internal static class OrderedListComparerHelper
{
    public static bool AreEqual<T>(List<T>? left, List<T>? right, Func<T, T, bool> elementEquals)
    {
        if (ReferenceEquals(left, right))
            return true;

        if (left is null || right is null || left.Count != right.Count)
            return false;

        for (var i = 0; i < left.Count; i++)
        {
            if (!elementEquals(left[i], right[i]))
                return false;
        }

        return true;
    }

    public static int GetHashCode<T>(List<T> value, Func<T, int> elementHashCode)
    {
        var hashCode = new HashCode();
        foreach (var item in value)
            hashCode.Add(elementHashCode(item));

        return hashCode.ToHashCode();
    }

    public static List<T> Snapshot<T>(List<T> value, Func<T, T> snapshotElement)
        => value.Select(snapshotElement).ToList();
}
