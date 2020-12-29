using System;
using System.Collections.Generic;
using System.Text;

namespace HexG
{
    public enum Direction
    {
        Right,      // +X
        Left,       // -X
        Up,         // +Y
        Down,       // -Y
        Forward,    // +Z
        Backwards,  // -Z
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
        public static HexVec ToHexVec(this Direction direction, HexBasis? basis = null)
        {
            switch (direction)
            {
                case Direction.Right:
                    return new HexVec(1, 0, 0, basis);
                case Direction.Left:
                    return new HexVec(-1, 0, 0, basis);
                case Direction.Up:
                    return new HexVec(0, 1, 0, basis);
                case Direction.Down:
                    return new HexVec(0, -1, 0, basis);
                case Direction.Forward:
                    return new HexVec(0, 0, 1, basis);
                case Direction.Backwards:
                    return new HexVec(0, 0, -1, basis);
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
