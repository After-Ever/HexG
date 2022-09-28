using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AEUtils;

namespace HexG
{
    public class BFSRegion : IReadOnlyRegion
    {
        public readonly HexPoint center;
        public readonly IReadOnlyRegion searchSpace;
        public readonly int size;
        public readonly Random randomSource;

        // Could return size, but not sure if it will actually be able to realize
        // that size, unless we do the search. So might as well wrap with Region.
        public int Count
            => throw new NotSupportedException("Wrap this with Region to use.");
            
        /// <summary>
        /// Create a region by performing a bfs on <paramref name="searchSpace"/>.
        /// </summary>
        /// <param name="center">Center.</param>
        /// <param name="searchSpace">Search space.</param>
        /// <param name="size">Max size. If this many hexes cannot be found,
        /// count will be less than this.</param>
        /// <param name="random">Optionally include to randomize search order. 
        /// Otherwise order will be indeterminate.</param>
        public BFSRegion(
            HexPoint center,
            IReadOnlyRegion searchSpace,
            int size,
            Random random = null)
        {
            this.center = center;
            this.searchSpace = searchSpace;
            this.size = size;
            this.randomSource = random;

            if (!searchSpace.Contains(center))
                throw new ArgumentException("center must be contained in searchSpace");
        }

        IEnumerable<HexPoint> Enumerate()
        {
            int yieldCount = 0;

            var found = new HashSet<HexPoint>();
            found.Add(center);

            var toSearch = new HashSet<HexPoint>();
            toSearch.Add(center);

            while (toSearch.Count > 0 && yieldCount < size)
            {
                var searching = toSearch;
                toSearch = new HashSet<HexPoint>();
                while (searching.Count > 0 && yieldCount < size)
                {
                    HexPoint p;
                    if (randomSource != null)
                        p = searching.TakeRandom(randomSource);
                    else
                        p = searching.First();
                    searching.Remove(p);

                    yield return p;
                    ++yieldCount;

                    foreach (var d in HexDirection.Values)
                    {
                        var s = p + d.ToHexPoint();
                        if (found.Contains(s) || !searchSpace.Contains(s))
                            continue;
                        found.Add(s);
                        toSearch.Add(s);
                    }
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
