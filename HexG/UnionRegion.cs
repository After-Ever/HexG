using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HexG
{
    public class UnionRegion : IReadOnlyRegion
    {
        public readonly IReadOnlyRegion baseRegionA;
        public readonly IReadOnlyRegion baseRegionB;

        public UnionRegion(IReadOnlyRegion baseRegionA, IReadOnlyRegion baseRegionB)
        {
            this.baseRegionA = baseRegionA;
            this.baseRegionB = baseRegionB;
        }
        
        public int Count 
            => baseRegionA.Count + baseRegionB.Count - baseRegionA.Intersect(baseRegionB).Count();

        public bool Contains(HexPoint point)
            => baseRegionA.Contains(point) || baseRegionB.Contains(point);

        public IEnumerator<HexPoint> GetEnumerator()
            => baseRegionA.Union(baseRegionB).GetEnumerator();

        public int MaxInDirection(Direction direction)
            => Math.Max(baseRegionA.MaxInDirection(direction), baseRegionB.MaxInDirection(direction));

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
