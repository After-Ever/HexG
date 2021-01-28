using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace HexG
{
    public class PointRegion : IReadOnlyRegion
    {
        public readonly HexPoint Point;

        public PointRegion(HexPoint point)
        {
            Point = point;
        }

        public int Count => 1;

        public bool Contains(HexPoint point)
            => Point == point;

        public IEnumerator<HexPoint> GetEnumerator()
            => PointAsEnumerable().GetEnumerator();

        public int MaxInDirection(Direction direction)
            => Point.InDirection(direction);

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        IEnumerable<HexPoint> PointAsEnumerable()
        {
            yield return Point;
        }
    }

    public static class PointRegionUtil
    {
        public static PointRegion AsRegion(this HexPoint point)
            => new PointRegion(point);
    }
}
