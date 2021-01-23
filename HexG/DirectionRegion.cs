using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace HexG
{
    public class DirectionRegion : IReadOnlyRegion
    {
        public int Count => distance + 1;

        HexPoint origin;
        int distance;
        Direction direction;

        public DirectionRegion(int distance, Direction direction, HexPoint? origin = null)
        {
            this.origin = origin ?? HexPoint.Zero;
            this.distance = distance;
            this.direction = direction;
        }

        public bool Contains(HexPoint item)
        {
            var v = (item - origin).Minimized;
            var d = v.ManhattanDistance;

            return d <= distance
                && v == direction.ToHexPoint() * d;
        }

        public IEnumerator<HexPoint> GetEnumerator()
            => new Enumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int MaxInDirection(Direction direction)
        {
            if (direction == this.direction)
            {
                return distance;
            }
            return 0;
        }

        class Enumerator : IEnumerator<HexPoint>
        {
            DirectionRegion region;

            int i;

            public Enumerator(DirectionRegion region)
            {
                this.region = region;
                i = -1;
            }

            public HexPoint Current => region.direction.ToHexPoint() * i + region.origin;

            object IEnumerator.Current => Current;

            public void Dispose() { }

            public bool MoveNext()
                => ++i > region.distance;

            public void Reset()
            {
                i = -1;
            }
        }
    }
}
