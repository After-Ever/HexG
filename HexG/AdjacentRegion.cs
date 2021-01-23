using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HexG
{
    public class AdjacentRegion : IReadOnlyRegion
    {
        public HexPoint Origin;
        public override int Count => 6;

        public AdjacentRegion(HexPoint? origin)
        {
            Origin = origin ?? HexPoint.Zero;
        }


        public override bool Contains(HexPoint item)
            => (item - Origin).MinManhattanDistance == 1;

        public override void CopyTo(HexPoint[] array, int arrayIndex)
        {
            for (int i = arrayIndex; i < Count; ++i)
            {
                array[i] = HexDirection.Values[i].ToHexPoint() + Origin;
            }
        }

        public override IEnumerator<HexPoint> GetEnumerator()
            => HexDirection.Values.Select(dir => dir.ToHexPoint() + Origin).GetEnumerator();

        public override int MaxInDirection(Direction direction)
            => Origin.InDirection(direction) + 1;

        // TODO The rest of these I guess...
        public override bool IsProperSubsetOf(IEnumerable<HexPoint> other)
        {
            throw new NotImplementedException();
        }

        public override bool IsProperSupersetOf(IEnumerable<HexPoint> other)
        {
            throw new NotImplementedException();
        }

        public override bool IsSubsetOf(IEnumerable<HexPoint> other)
        {
            throw new NotImplementedException();
        }

        public override bool IsSupersetOf(IEnumerable<HexPoint> other)
        {
            throw new NotImplementedException();
        }

        public override bool Overlaps(IEnumerable<HexPoint> other)
        {
            throw new NotImplementedException();
        }

        public override bool SetEquals(IEnumerable<HexPoint> other)
        {
            throw new NotImplementedException();
        }
    }
}
