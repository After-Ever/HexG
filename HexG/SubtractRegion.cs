using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HexG
{
    public class SubtractRegion : IReadOnlyRegion
    {
        public readonly IReadOnlyRegion baseRegionA;
        public readonly IReadOnlyRegion baseRegionB;

        public SubtractRegion(IReadOnlyRegion baseRegionA, IReadOnlyRegion baseRegionB)
        {
            this.baseRegionA = baseRegionA;
            this.baseRegionB = baseRegionB;
        }

        public int Count
            => this.Count();
        public bool Contains(HexPoint point)
            => baseRegionA.Contains(point) && !baseRegionB.Contains(point);

        public IEnumerator<HexPoint> GetEnumerator()
            => Enumerable().GetEnumerator();

        // TODO Implement!
        public int MaxInDirection(Direction direction)
            => throw new NotImplementedException();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        IEnumerable<HexPoint> Enumerable()
        {
            foreach (var point in baseRegionA)
                if (!baseRegionB.Contains(point))
                    yield return point;
        }
    }
}
