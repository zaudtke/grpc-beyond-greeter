using System;
using System.Collections.Generic;
using Grpc.Core;

namespace Grpc.BeyondGreeter.UnitTest.Testing;

public class MetaDataComparer : IEqualityComparer<Metadata>
{
    public bool Equals(Metadata? left, Metadata? right)
    {
        if (left == null || right == null)
            return false;

        if (left.Count != right.Count) return false;

        var trailersEqual = false;
        for (var i = 0; i < left.Count; i++)
        {
            trailersEqual = left[i].Key == right[i].Key && left[i].Value == right[i].Value;
            if (!trailersEqual)
                break;
        }

        return trailersEqual;
    }

    public int GetHashCode(Metadata obj)
    {
        return HashCode.Combine(obj.Count, obj.IsReadOnly);
    }
}