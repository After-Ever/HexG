using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace HexG
{
    /// <summary>
    /// A collection of indices within a <see cref="IHexMap{T}"/>.
    /// </summary>
    public interface IRegion : ISet<HexPoint>
    {
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

    public struct Region : IRegion
    {
        HashSet<HexPoint> indices;

        public Region(IEnumerable<HexPoint> indices = null)
        {
            if (indices != null)
                this.indices = new HashSet<HexPoint>(indices);
            else
                this.indices = new HashSet<HexPoint>();
        }

        public int Count =>                                             indices.Count;
        public bool Add(HexPoint item) =>                               indices.Add(item);
        public void Clear() =>                                          indices.Clear();
        public bool Contains(HexPoint item) =>                          indices.Contains(item);
        public void CopyTo(HexPoint[] array, int arrayIndex) =>         indices.CopyTo(array, arrayIndex);
        public void ExceptWith(IEnumerable<HexPoint> other) =>          indices.ExceptWith(other);
        public IEnumerator<HexPoint> GetEnumerator() =>                 indices.GetEnumerator();
               IEnumerator IEnumerable.GetEnumerator() =>               indices.GetEnumerator();
        public void IntersectWith(IEnumerable<HexPoint> other) =>       indices.IntersectWith(other);
        public bool IsProperSubsetOf(IEnumerable<HexPoint> other) =>    indices.IsProperSubsetOf(other);
        public bool IsProperSupersetOf(IEnumerable<HexPoint> other) =>  indices.IsProperSupersetOf(other);
        public bool IsSubsetOf(IEnumerable<HexPoint> other) =>          indices.IsSubsetOf(other);
        public bool IsSupersetOf(IEnumerable<HexPoint> other) =>        indices.IsSupersetOf(other);
        public bool Overlaps(IEnumerable<HexPoint> other) =>            indices.Overlaps(other);
        public bool Remove(HexPoint item) =>                            indices.Remove(item);
        public bool SetEquals(IEnumerable<HexPoint> other) =>           indices.SetEquals(other);
        public void SymmetricExceptWith(IEnumerable<HexPoint> other) => indices.SymmetricExceptWith(other);
        public void UnionWith(IEnumerable<HexPoint> other) =>           indices.UnionWith(other);
               void ICollection<HexPoint>.Add(HexPoint item) =>         indices.Add(item);
        public bool IsReadOnly => false;

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
    }
}
