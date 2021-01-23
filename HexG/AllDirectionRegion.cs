using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HexG
{
    public class AllDirectionRegion : IReadOnlyRegion
    {
        public int Count => distance * 6 + 1;

        HexPoint origin;
        int distance;

        public AllDirectionRegion(int distance, HexPoint? origin = null)
        {
            this.origin = origin ?? HexPoint.Zero;
            this.distance = distance;
        }

        public bool Contains(HexPoint item)
        {
            var v = (item - origin).Minimized;
            var d = v.ManhattanDistance;

            return d <= distance && (Math.Abs(v.X) == d || Math.Abs(v.Y) == d || Math.Abs(v.Z) == d);
        }

        public IEnumerator<HexPoint> GetEnumerator()
            => new Enumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int MaxInDirection(Direction direction)
            => distance;

        class Enumerator : IEnumerator<HexPoint>
        {
            AllDirectionRegion region;

            int i;
            int d;

            public Enumerator(AllDirectionRegion region)
            {
                this.region = region;
                i = -1;
                d = 0;
            }

            public HexPoint Current => HexDirection.Values[d].ToHexPoint() * i + region.origin;

            object IEnumerator.Current => Current;

            public void Dispose() { }

            public bool MoveNext()
            {
                if (++i > region.distance)
                {
                    if (++d >= HexDirection.Values.Length)
                        return false;
                    i = 0;
                }

                return true;
            }

            public void Reset()
            {
                i = -1;
                d = 0;
            }
        }
    }
}
