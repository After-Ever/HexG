using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HexG
{
    public class IntersectRegion : IReadOnlyRegion
    {
        public readonly IReadOnlyRegion baseRegionA;
        public readonly IReadOnlyRegion baseRegionB;

        public IntersectRegion(IReadOnlyRegion baseRegionA, IReadOnlyRegion baseRegionB)
        {
            this.baseRegionA = baseRegionA;
            this.baseRegionB = baseRegionB;
        }

        public int Count
            => baseRegionA.Intersect(baseRegionB).Count();

        public bool Contains(HexPoint point)
            => baseRegionA.Contains(point) && baseRegionB.Contains(point);

        public IEnumerator<HexPoint> GetEnumerator()
            => baseRegionA.Intersect(baseRegionB).GetEnumerator();

        // TODO implement!
        public int MaxInDirection(Direction direction)
            => throw new NotImplementedException();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
