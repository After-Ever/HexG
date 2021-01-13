using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HexG
{
    public class AllDirectionRegion : IRegion
    {
        public int Count => distance * 6 + 1;
        public bool IsReadOnly => true;

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
            var d = v.ManhattanDistance();

            return d <= distance && (Math.Abs(v.X) == d || Math.Abs(v.Y) == d || Math.Abs(v.Z) == d);
        }

        public void CopyTo(HexPoint[] array, int arrayIndex)
        {
            int i = arrayIndex;
            foreach(var point in this.Skip(arrayIndex))
            {
                array[i++] = point;
            }
        }

        public IEnumerator<HexPoint> GetEnumerator()
            => new Enumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool IsProperSubsetOf(IEnumerable<HexPoint> other)
        {
            throw new NotImplementedException();
        }

        public bool IsProperSupersetOf(IEnumerable<HexPoint> other)
        {
            throw new NotImplementedException();
        }

        public bool IsSubsetOf(IEnumerable<HexPoint> other)
        {
            throw new NotImplementedException();
        }

        public bool IsSupersetOf(IEnumerable<HexPoint> other)
        {
            throw new NotImplementedException();
        }

        public int MaxInDirection(Direction direction)
        {
            return distance;
        }

        public bool Overlaps(IEnumerable<HexPoint> other)
        {
            throw new NotImplementedException();
        }


        // Readonly, so the following are not supported.

        public bool Add(HexPoint item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public void ExceptWith(IEnumerable<HexPoint> other)
        {
            throw new NotImplementedException();
        }

        public void IntersectWith(IEnumerable<HexPoint> other)
        {
            throw new NotImplementedException();
        }

        public bool Remove(HexPoint item)
        {
            throw new NotImplementedException();
        }

        public bool SetEquals(IEnumerable<HexPoint> other)
        {
            throw new NotImplementedException();
        }

        public void SymmetricExceptWith(IEnumerable<HexPoint> other)
        {
            throw new NotImplementedException();
        }

        public void UnionWith(IEnumerable<HexPoint> other)
        {
            throw new NotImplementedException();
        }

        void ICollection<HexPoint>.Add(HexPoint item)
        {
            throw new NotImplementedException();
        }

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
