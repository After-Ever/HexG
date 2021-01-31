using System;
using System.Collections.Generic;
using System.Text;

namespace HexG
{
    public enum Direction
    {
        Right,      // +X
        Forward,    // +Z
        Up,         // +Y
        Left,       // -X
        Backwards,  // -Z
        Down,       // -Y
    }

    public static class HexDirection
    {
        static Random r = new Random();

        public static Direction[] Values = (Direction[])Enum.GetValues(typeof(Direction));

        /// <summary>
        /// Returns the <see cref="HexVec"/> which represents the given direction,
        /// wrt <paramref name="basis"/>.
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="basis">The basis to build the HexVec off of. If null is provided,
        /// will use <see cref="HexBasis.Standard"/>.</param>
        /// <returns></returns>
        public static HexPoint ToHexPoint(this Direction direction)
        {
            switch (direction)
            {
                case Direction.Right:
                    return new HexPoint(1, 0, 0);
                case Direction.Left:
                    return new HexPoint(-1, 0, 0);
                case Direction.Up:
                    return new HexPoint(0, 1, 0);
                case Direction.Down:
                    return new HexPoint(0, -1, 0);
                case Direction.Forward:
                    return new HexPoint(0, 0, 1);
                case Direction.Backwards:
                    return new HexPoint(0, 0, -1);
                default:
                    throw new NotSupportedException("Not a valid direction");
            }
        }

        /// <summary>
        /// Returns which direction the given <see cref="HexVec"/> favors.
        /// </summary>
        /// <param name="vec"></param>
        /// <param name="otherClosest">If the vector is perfectly between two directions, the one not returned
        /// will fill this value. Otherwise it will be set to null.</param>
        /// <returns></returns>
        public static Direction ClosestDirection(this HexVec vec, out Direction? otherClosest)
        {
            if (vec == new HexVec())
                throw new Exception("Zero vector has no direction.");

            otherClosest = null;

            var v = vec.Minimized;

            if (v.X == 0)
            {
                if (v.Y > 0)
                {
                    // Either forward, up, or both!
                    if (v.Y == v.Z)
                    {
                        otherClosest = Direction.Up;
                        return Direction.Forward;
                    }
                    return v.Y > v.Z ? Direction.Up : Direction.Forward;
                }
                if (v.Y < 0)
                {
                    // Either backwards, down, or both!
                    if (v.Y == v.Z)
                    {
                        otherClosest = Direction.Down;
                        return Direction.Backwards;
                    }
                    return v.Y < v.Z ? Direction.Down : Direction.Backwards;
                }
                // y == 0, and z != 0 by initial check.
                return v.Z > 0 ? Direction.Forward : Direction.Backwards;
            }
            if (v.Y == 0)
            {
                // x != 0
                if (v.X > 0)
                {
                    // Either forward, right, or both!
                    if (v.X == v.Z)
                    {
                        otherClosest = Direction.Forward;
                        return Direction.Right;
                    }
                    return v.X > v.Z ? Direction.Right : Direction.Forward;
                }
                // x < 0
                if (v.X == v.Z)
                {
                    otherClosest = Direction.Backwards;
                    return Direction.Left;
                }
                return v.X < v.Z ? Direction.Left : Direction.Backwards;
            }
            // x != 0
            // y != 0
            // z == 0
            if (v.X > 0)
            {
                // Either up, right, or both!
                if (v.X == -v.Y)
                {
                    otherClosest = Direction.Right;
                    return Direction.Down;
                }
                return v.X > -v.Y ? Direction.Right : Direction.Down; 
            }
            // x < 0
            // y > 0
            // z == 0
            // Either up, left, or both!
            if (v.X == -v.Y)
            {
                otherClosest = Direction.Left;
                return Direction.Up;
            }
            return v.X < -v.Y ? Direction.Left : Direction.Up;
        }

        /// <summary>
        /// In case of a vector perfectly inbetween two directions, the more clockwise one will be favored. 
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static Direction ClosestDirection(this HexVec vec)
        {
            Direction? _;
            return ClosestDirection(vec, out _);
        }

        /// <summary>
        /// Same as <see cref="ClosestDirection(HexVec, out Direction?)"/>, but returns the
        /// directions in sequence.
        /// </summary>
        /// <param name="vec"></param>
        /// <returns>The closest directions in counterclockwise order.</returns>
        public static IEnumerable<Direction> ClosestDirections(this HexVec vec)
        {
            Direction? other;
            yield return ClosestDirection(vec, out other);
            if (other != null)
                yield return other.Value;
        }

        /// <summary>
        /// Get the Direction vectors this point is between.
        /// Will return either the two closest directions, or just the one direction 
        /// on which this point lies, or zero if this is the zero vector.
        /// </summary>
        /// <param name="vec"></param>
        /// <returns>The directions <paramref name="vec"/> is between in
        /// counterclockwise order.</returns>
        public static IEnumerable<Direction> ContainingDirections(this HexVec vec)
        {
            if (vec == new HexVec())
                yield break;

            var v = vec.Minimized;

            if (v.X == 0)
            {
                if (v.Y > 0)
                {
                    if (v.Z == 0)
                    {
                        yield return Direction.Up;
                    }
                    else
                    {
                        yield return Direction.Forward;
                        yield return Direction.Up;
                    }
                }
                else if (v.Y < 0)
                {
                    if (v.Z == 0)
                    {
                        yield return Direction.Down;
                    }
                    else
                    {
                        yield return Direction.Backwards;
                        yield return Direction.Down;
                    }
                }
                // y == 0, and z != 0 by initial check.
                else
                {
                    yield return v.Z > 0 ? Direction.Forward : Direction.Backwards;
                }
            }
            else if (v.Y == 0)
            {
                // x != 0
                if (v.X > 0)
                {
                    if (v.Z == 0)
                    {
                        yield return Direction.Right;
                    }
                    else
                    {
                        yield return Direction.Right;
                        yield return Direction.Forward;
                    }
                }
                // x < 0
                else
                {
                    if (v.Z == 0)
                    {
                        yield return Direction.Left;
                    }
                    else
                    {
                        yield return Direction.Left;
                        yield return Direction.Backwards;
                    }
                }
            }
            // x != 0
            // y != 0
            // z == 0
            else if (v.X > 0)
            {
                yield return Direction.Down;
                yield return Direction.Right;
            }
            // x < 0
            // y > 0
            // z == 0
            else
            {
                yield return Direction.Up;
                yield return Direction.Left;
            }

        }

        /// <summary>
        /// If <paramref name="vec"/> is perfectly between two direction, the one returned will
        /// be selected randomly.
        /// </summary>
        /// <param name="random"></param>
        /// <returns></returns>
        public static Direction RandomClosestDirection(this HexVec vec, Random random = null)
        {
            Direction? other;
            var dir = ClosestDirection(vec, out other);

            if (other == null)
                return dir;

            return (random ?? r).Next() % 2 == 0 ? dir : other.Value;
        }

        public static IEnumerable<Direction> DirectionsFromCCW(Direction start)
        {
            int s = -1;
            for (int i = 0; i < 6; ++i)
            {
                if (Values[i] == start)
                {
                    s = i;
                }

                if (s != -1)
                    yield return Values[i];
            }
            for (int i = 0; i < s; ++i)
                yield return Values[i];
        }

        public static IEnumerable<Direction> DirectionsFromCW(Direction start)
        {

            int s = -1;
            for (int i = 5; i >= 0; --i)
            {
                if (Values[i] == start)
                {
                    s = i;
                }

                if (s != -1)
                    yield return Values[i];
            }
            for (int i = 5; i >= 0; --i)
                yield return Values[i];
        }
    }
}
