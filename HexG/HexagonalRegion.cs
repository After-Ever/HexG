using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HexG
{
    public class HexagonalRegion : IRegion
    {
        public int Count => 1 + 3 * radius * (radius + 1);
        public bool IsReadOnly => true;

        int radius;
        HexPoint offset;

        /// <summary>
        /// Create a region in a hexagonal shape, around <paramref name="offset"/>.
        /// </summary>
        /// <param name="radius">How many hexes from <paramref name="offset"/> does this region extend.
        /// A radius of 0 is a single hex.</param>
        /// <param name="offset"></param>
        public HexagonalRegion(int radius, HexPoint? offset = null)
        {
            if (radius < 0)
                throw new Exception("Radius must be non-negative.");

            this.radius = radius;
            this.offset = offset ?? new HexPoint();
        }

        public bool Contains(HexPoint item)
        {
            var toItem = (item - offset).Minimized;
            var md = toItem.ManhattanDistance;

            return md <= radius;
        }

        public void CopyTo(HexPoint[] array, int arrayIndex)
        {
            var e = GetEnumerator();

            while (e.MoveNext() && arrayIndex < array.Count())
            {
                array[arrayIndex++] = e.Current;
            }
        }

        public IEnumerator<HexPoint> GetEnumerator() => new Enumerator(this);
        IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

        public bool IsProperSubsetOf(IEnumerable<HexPoint> other)
        {
            HashSet<HexPoint> inOther = new HashSet<HexPoint>(other);
            return this.All(inOther.Contains) && inOther.Count > Count;
        }

        public bool IsProperSupersetOf(IEnumerable<HexPoint> other)
        {
            HashSet<HexPoint> inOther = new HashSet<HexPoint>();

            foreach (var i in other)
            {
                if (!Contains(i))
                    return false;

                inOther.Add(i);
            }

            return inOther.Count < Count;
        }

        public bool IsSubsetOf(IEnumerable<HexPoint> other)
        {
            HashSet<HexPoint> inOther = new HashSet<HexPoint>(other);
            return this.All(inOther.Contains);
        }

        public bool IsSupersetOf(IEnumerable<HexPoint> other)
            => other.All(Contains);

        public bool Overlaps(IEnumerable<HexPoint> other)
            => other.Any(Contains);

        public bool SetEquals(IEnumerable<HexPoint> other)
        {
            HashSet<HexPoint> inOther = new HashSet<HexPoint>();

            foreach (var i in other)
            {
                if (!Contains(i))
                    return false;

                inOther.Add(i);
            }

            return inOther.Count == Count;
        }

        public int MaxInDirection(Direction direction) => radius;


        // Readonly, so the following are not supported.

        public bool Add(HexPoint item)
        {
            throw new NotSupportedException();
        }

        void ICollection<HexPoint>.Add(HexPoint item)
        {
            throw new NotSupportedException();
        }

        public bool Remove(HexPoint item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public void UnionWith(IEnumerable<HexPoint> other)
        {
            throw new NotSupportedException();
        }

        public void IntersectWith(IEnumerable<HexPoint> other)
        {
            throw new NotSupportedException();
        }

        public void ExceptWith(IEnumerable<HexPoint> other)
        {
            throw new NotSupportedException();
        }

        public void SymmetricExceptWith(IEnumerable<HexPoint> other)
        {
            throw new NotSupportedException();
        }

        class Enumerator : IEnumerator<HexPoint>
        {
            HexagonalRegion region;

            int curR;
            int curI;
            HexPoint curPoint;

            public HexPoint Current => curI != -1 || curR != 0 ? curPoint : throw new Exception();
            object IEnumerator.Current => Current;

            public Enumerator(HexagonalRegion region)
            {
                this.region = region;
                Reset();
            }

            public void Dispose() { }

            public bool MoveNext()
            {
                var lastI = curI;
                var lastR = curR;
                
                if (curR == 0)
                {
                    if (++curI == 1)
                    {
                        if (region.radius == 0)
                            return false;

                        curI = 0;
                        curR = 1;
                    }
                }
                else
                {
                    if (++curI == curR * 6)
                    {
                        curI = 0;

                        if (++curR > region.radius)
                            return false;
                    }
                }


                curPoint = AdvancePoint(curPoint, lastI, lastR);
                return true;
            }

            public void Reset()
            {
                curPoint = region.offset;
                curR = 0;
                curI = -1;
            }

            HexPoint AdvancePoint(HexPoint point, int lastIndex, int lastRadius)
            {
                // If lastRadius is 0, then this is the first advance from the center, so just out-shift.
                if (lastRadius == 0)
                    return lastIndex == -1 ? point : point + HexDirection.Values[4].ToHexPoint();

                // If the last index completed the last perimeter, move back to the start of the perimeter, and out-shift.
                if (lastIndex + 1 == lastRadius * 6)
                    return point + HexDirection.Values[5].ToHexPoint() + HexDirection.Values[4].ToHexPoint();

                return point + HexDirection.Values[lastIndex / lastRadius].ToHexPoint();
            }
        }
    }
}
