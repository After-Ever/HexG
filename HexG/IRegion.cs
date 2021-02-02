using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HexG
{
    public interface IReadOnlyRegion : IEnumerable<HexPoint>
    {
        int Count { get; }
        bool Contains(HexPoint point);

        /// <summary>
        /// The largest index needed to represent the indices in <paramref name="direction"/>.
        /// 
        /// To get the min, specify the direction in the opposite direction, and negate the return.
        /// e.g. min(right) : -MaxInDirection(Direction.Left)
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        int MaxInDirection(Direction direction);
    }

    public interface IRegion : IReadOnlyRegion
    {
        /// <summary>
        /// Add the given point to the region.
        /// </summary>
        /// <param name="point"></param>
        /// <returns>True if this point was not previously in the region.</returns>
        bool Add(HexPoint point);
        /// <summary>
        /// Remove the given point from the region.
        /// </summary>
        /// <param name="point"></param>
        /// <returns>True if this point was previously in the region.</returns>
        bool Remove(HexPoint point);
        void Clear();
    }

    public class Region : IRegion
    {
        HashSet<HexPoint> indices;

        public Region(IEnumerable<HexPoint> indices = null)
        {
            if (indices != null)
                this.indices = new HashSet<HexPoint>(indices);
            else
                this.indices = new HashSet<HexPoint>();
        }

        public int Count =>                    indices.Count;
        public bool Add(HexPoint item) =>      indices.Add(item);
        public void Clear() =>                 indices.Clear();
        public bool Contains(HexPoint item) => indices.Contains(item);
        public bool Remove(HexPoint item) =>   indices.Remove(item);

        public int MaxInDirection(Direction direction)
        {
            int max = int.MinValue;

            foreach (var i in indices)
            {
                var inDir = i.Minimized.InDirection(direction);
                if (inDir > max)
                    max = inDir;
            }

            return max;
        }

        public IEnumerator<HexPoint> GetEnumerator()
            => indices.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }

    public static class RegionUtils
    {
        public static IReadOnlyRegion UnionRegion(this IReadOnlyRegion a, IReadOnlyRegion b)
            => new UnionRegion(a, b);
        public static IReadOnlyRegion IntersectRegion(this IReadOnlyRegion a, IReadOnlyRegion b)
            => new IntersectRegion(a, b);
        public static IReadOnlyRegion SubtractRegion(this IReadOnlyRegion a, IReadOnlyRegion b)
            => new SubtractRegion(a, b);
    }
}
