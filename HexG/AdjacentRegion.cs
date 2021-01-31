using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HexG
{
    public class AdjacentRegion : IReadOnlyRegion
    {
        public HexPoint Origin;
        public int Count => 6;

        public AdjacentRegion(HexPoint? origin = null)
        {
            Origin = origin ?? HexPoint.Zero;
        }

        public bool Contains(HexPoint item)
            => (item - Origin).MinManhattanDistance == 1;

        public int MaxInDirection(Direction direction)
            => Origin.InDirection(direction) + 1;

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
        public IEnumerator<HexPoint> GetEnumerator()
            => HexDirection.Values.Select(dir => dir.ToHexPoint() + Origin).GetEnumerator();
    }
}
