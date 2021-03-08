using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using HexG;

namespace HexG.Pathing
{
    /// <summary>
    /// A region containing all the points reachable from some point
    /// with respects to a max distance, and allowed and disallowed regions.
    /// </summary>
    public class ReachableRegion : IReadOnlyRegion
    {
        readonly Region baseRegion;
        
        public ReachableRegion(
            HexPoint origin,
            int maxDistance,
            IReadOnlyRegion allowed = null,
            IReadOnlyRegion disallowed = null)
        {
            var search = Pathfinder.AStarSearch(
                origin,
                maxDistance,
                _ => 0,
                (a, b) => (a - b).MinManhattanDistance,
                point => new AdjacentRegion(point)
                    .Where(p => (allowed?.Contains(p) ?? true)
                        && !(disallowed?.Contains(p) ?? false)))
                .Select(node => node.Item1);

            baseRegion = new Region(search);
        }

        public int Count => baseRegion.Count;

        public bool Contains(HexPoint point)
            => baseRegion.Contains(point);

        public IEnumerator<HexPoint> GetEnumerator()
            => baseRegion.GetEnumerator();

        public int MaxInDirection(Direction direction)
            => baseRegion.MaxInDirection(direction);

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
