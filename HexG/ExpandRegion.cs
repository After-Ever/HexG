using System;
using System.Collections;
using System.Collections.Generic;

namespace HexG
{
    public class ExpandRegion : IReadOnlyRegion
    {
        public readonly IReadOnlyRegion baseRegion;
        public readonly IReadOnlyRegion searchSpace;
        public readonly int radius;
        public readonly bool includeBase;

        public int Count
            => throw new NotSupportedException("Wrap this with Region to use.");

        public ExpandRegion(IReadOnlyRegion baseRegion, IReadOnlyRegion searchSpace, int radius, bool includeBase)
        {
            this.baseRegion = baseRegion
                ?? throw new ArgumentNullException(nameof(baseRegion));
            this.searchSpace = searchSpace
                ?? throw new ArgumentNullException(nameof(searchSpace));
            this.radius = radius;
            this.includeBase = includeBase;
        }

        IEnumerable<HexPoint> Enumerate()
        {
            var found = new HashSet<HexPoint>(baseRegion);

            var toSearch = new Queue<HexPoint>();
            void AddToSearch(HexPoint p)
            {
                foreach (var d in HexDirection.Values)
                {
                    var s = p + d.ToHexPoint();
                    if (found.Contains(s))
                        continue;
                    found.Add(s);
                    toSearch.Enqueue(s);
                }
            }

            // Add the first "ring"
            foreach (var b in baseRegion)
                AddToSearch(b);
            if (includeBase)
                foreach (var b in baseRegion)
                    yield return b;

            for (int r = 0; r < radius; ++r)
            {
                if (toSearch.Count == 0)
                    yield break;

                var searching = toSearch;
                toSearch = new Queue<HexPoint>();
                while (searching.Count > 0)
                {
                    HexPoint p = searching.Dequeue();
                    yield return p;
                    AddToSearch(p);
                }
            }
        }

        public bool Contains(HexPoint point)
            => throw new NotSupportedException("Wrap this with Region to use.");

        public IEnumerator<HexPoint> GetEnumerator()
            => Enumerate().GetEnumerator();

        public int MaxInDirection(Direction direction)
            => throw new NotSupportedException("Wrap this with Region to use.");

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
