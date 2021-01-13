using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace HexG
{
    public class DirectionRegion : IRegion
    {
        public int Count => distance + 1;
        public bool IsReadOnly => true;

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
            var d = v.ManhattanDistance();

            return d <= distance
                && v == direction.ToHexPoint() * d;
        }

        public void CopyTo(HexPoint[] array, int arrayIndex)
        {
            for (int i = arrayIndex; i < Count; ++i)
            {
                array[i] = direction.ToHexPoint() * i + origin;
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
            if (direction == this.direction)
            {
                return distance;
            }
            return 0;
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
