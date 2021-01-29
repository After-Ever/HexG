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
    }
}
