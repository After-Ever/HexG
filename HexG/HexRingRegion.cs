using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace HexG
{
    public class HexRingRegion : IReadOnlyRegion
    {
        public readonly int radius;
        public readonly HexPoint offset;

        public HexRingRegion(int radius, HexPoint? offset)
        {
            if (radius <= 0)
                throw new ArgumentOutOfRangeException("Radius must be greater than zero.");

            this.radius = radius;
            this.offset = offset ?? HexPoint.Zero;
        }


        public int Count => radius * 6;

        public bool Contains(HexPoint point)
            => (point - offset).MinManhattanDistance == radius;

        public IEnumerator<HexPoint> GetEnumerator()
            => new Enumerator(this);
        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public int MaxInDirection(Direction direction)
            => offset.InDirection(direction) + radius;

        class Enumerator : IEnumerator<HexPoint>
        {
            readonly HexRingRegion region;
            int d = 0;
            int i = -1;

            public Enumerator(HexRingRegion region)
            {
                this.region = region;
                Reset();
            }

            public HexPoint Current { get; private set; }
            object IEnumerator.Current => Current;

            public void Dispose() { }

            public bool MoveNext()
            {
                var dir = HexDirection.Values[(d + 2) % 6];
                Current += dir.ToHexPoint();
                if (++i == region.radius)
                {
                    if (++d == 6)
                        return false;
                    i = 0;
                }
                return true;
            }

            public void Reset()
            {
                d = 0;
                i = -1;
                Current = region.offset
                    + HexDirection.Values[0].ToHexPoint() * region.radius
                    + HexDirection.Values[5].ToHexPoint();
            }
        }
    }
}
