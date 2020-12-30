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
        /// <returns></returns>
        public static Direction ClosestDirection(this HexVec vec)
        {
            // Which ever axis has the largest absolute value when minimized is
            // the most "favored."
            var v = vec.Minimized;
            var x = Math.Abs(v.X);
            var y = Math.Abs(v.Y);
            var z = Math.Abs(v.Z);

            if (x > y)
            {
                if (x > z)
                    return v.X > 0 ? Direction.Right : Direction.Left;
            }
            else if (y > z)
            {
                return v.Y > 0 ? Direction.Up : Direction.Down;
            }

            return v.Z > 0 ? Direction.Forward : Direction.Backwards;
        }
    }
}
