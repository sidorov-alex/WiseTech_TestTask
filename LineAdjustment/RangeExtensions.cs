using System;

namespace LineAdjustment;

public static class RangeExtensions
{
    public static int GetLength(this Range range, int collectionLength)
    {
        var (_, length) = range.GetOffsetAndLength(collectionLength);
        return length;
    }
}