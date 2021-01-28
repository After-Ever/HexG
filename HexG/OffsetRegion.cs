using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HexG
{
    public class OffsetRegion : IReadOnlyRegion
    {
        public readonly IReadOnlyRegion baseRegion;
        public readonly HexPoint offset;

        public OffsetRegion(IReadOnlyRegion baseRegion, HexPoint offset)
        {
            this.baseRegion = baseRegion ?? throw new ArgumentNullException();
            this.offset = offset;
        }

        public int Count => baseRegion.Count;

        public bool Contains(HexPoint point) => baseRegion.Contains(point - offset);

        public IEnumerator<HexPoint> GetEnumerator()
            => baseRegion.Select(basePoint => basePoint + offset).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public int MaxInDirection(Direction direction)
            => baseRegion.MaxInDirection(direction) + offset.InDirection(direction);
    }
}
