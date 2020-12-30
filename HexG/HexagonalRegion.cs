using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HexG
{
    public struct HexagonalRegion : IRegion
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

            return Math.Abs(toItem.X) <= radius
                && Math.Abs(toItem.Y) <= radius
                && Math.Abs(toItem.Z) <= radius;
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

        // TODO: Good lord this needs some testing!
        class Enumerator : IEnumerator<HexPoint>
        {
            HexagonalRegion region;

            int curR;
            int curI;
            HexPoint curPoint;

            public HexPoint Current => curI != -1 ? curPoint : throw new Exception();
            object IEnumerator.Current => Current;

            public Enumerator(HexagonalRegion region)
            {
                this.region = region;
                Reset();
            }

            public void Dispose() { }

            public bool MoveNext()
            {
                ++curI;

                // Don't move if on the first index.
                if (curI > 0)
                {
                    var directions = (Direction[])Enum.GetValues(typeof(Direction));
                    var moveDir = directions[(curI - 1) / curR].ToHexPoint();

                    curPoint += moveDir;
                }

                if (curI > curR * 6)
                {
                    curI = -1;
                    ++curR;

                    var directions = (Direction[])Enum.GetValues(typeof(Direction));
                    curPoint += directions[4].ToHexPoint();
                }

                if (curR > region.radius)
                    return false;

                return true;
            }

            public void Reset()
            {
                curPoint = region.offset;
                curR = 0;
                curI = -1;
            }
        }
    }
}
