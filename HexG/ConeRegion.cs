using System;
using System.Collections;
using System.Collections.Generic;
<<<<<<< HEAD
using System.Linq;
=======
using System.Text;
>>>>>>> 8059c319e6e576558841b4fa6504425f76e973c2

namespace HexG
{
    /// <summary>
    /// A region defined by a start and end <see cref="Direction"/>, and a radius.
    /// 
    /// It is the region starting at start, and going counterclockwise until end.
    /// </summary>
    public class ConeRegion : IReadOnlyRegion
    {
        public int Count => throw new NotImplementedException();

        /// <summary>
        /// The clockwise bounding direction.
        /// </summary>
        public readonly Direction start;
        /// <summary>
        /// The counterclockwise bounding direction.
        /// </summary>
        public readonly Direction end;
        public readonly int radius;

        public readonly HexPoint origin;

        public ConeRegion(Direction start, Direction end, int radius, HexPoint? origin = null)
        {
            if (start == end)
                throw new Exception(
                    "Cone region start and end must not be equal! Use a DirectionRegion instead.");

            this.start = start;
            this.end = end;
            this.radius = radius;
            this.origin = origin ?? HexPoint.Zero;
        }

        public bool Contains(HexPoint point)
        {
            var to = point - origin;
            to.Minimize();

            if (to.ManhattanDistance > radius)
                return false;

            var dirEnumerator = to.ToVec().ContainingDirections().GetEnumerator();
            // Always at least one value, so move to it.
            dirEnumerator.MoveNext();
            foreach (var dir in HexDirection.DirectionsFromCCW(start))
            {
                if (dir == dirEnumerator.Current
                    && !dirEnumerator.MoveNext())
                    return true;

                if (dir == end)
                    return false;
            }

            return false;
        }

        public IEnumerator<HexPoint> GetEnumerator()
        {
            int angles = 0;
            foreach(var dir in HexDirection.DirectionsFromCCW(start).Skip(1))
            {
                ++angles;
                if (dir == end)
                    return new HexagonalRegion.Enumerator(
                        origin,
                        radius,
                        start,
                        angles);
            }

            throw new Exception("Couldn't find end!");
	}

        public int MaxInDirection(Direction direction)
        {
            foreach (var dir in HexDirection.DirectionsFromCCW(start))
            {
                if (dir == direction)
                    return origin.InDirection(direction) + radius;

                if (dir == end)
                    break;
            }

            return origin.InDirection(direction);
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
