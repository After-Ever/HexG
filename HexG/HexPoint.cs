using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace HexG
{
    /// <summary>
    /// Same as <see cref="HexVec"/>, but with integer values.
    /// </summary>
    public struct HexPoint
    {
        public int X, Y, Z;

        /// <summary>
        /// Construct a new HexPoint with the given values and basis.
        /// If <paramref name="basis"/> is null, then <see cref="HexBasis.Standard"/> will be used.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="basis"></param>
        public HexPoint(int x = 0, int y = 0, int z = 0)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public HexVec ToVec()
        {
            return new HexVec(X, Y, Z);
        }

        public Vector2 ToCartesian(HexBasis basis)
        => X * basis.X + Y * basis.Y + Z * basis.Z;

        public static readonly HexPoint Zero = new HexPoint();

        /// <summary>
        /// The sum of the absolute values of each coordinate.
        /// Notably this is not necessarily the minimum! Use <see cref="Minimize"/> to ensure min.
        /// </summary>
        public int ManhattanDistance() => Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);

        public override bool Equals(object obj)
        {
            if (!(obj is HexPoint v))
                return false;

            var a = Standardized;
            var b = v.Standardized;

            return a.X == b.X && a.Y == b.Y;
        }

        public override int GetHashCode()
        {
            var v = Standardized;

            var hashCode = 373119288;
            hashCode = hashCode * -1521134295 + v.X.GetHashCode();
            hashCode = hashCode * -1521134295 + v.Y.GetHashCode();
            return hashCode;
        }

        /// <summary>
        /// Standardize this HexPoint.
        /// A Standard HexPoint is one where z == 0.
        /// </summary>
        public void Standardize()
        {
            X += Z;
            Y += Z;
            Z = 0;
        }

        /// <summary>
        /// Return a new HexPoint which has been standardized.
        /// A Standard HexPoint is one where z == 0.
        /// </summary>
        /// <returns></returns>
        public HexPoint Standardized
        {
            get
            {
                // Create a copy.
                var r = this;
                r.Standardize();
                return r;
            }
        }

        /// <summary>
        /// Makes this the HexPoint the smallest mannhattan distance from this HexPoint's equivalence class.
        /// </summary>
        public void Minimize()
        {
            var minimized = Minimized;

            X = minimized.X;
            Y = minimized.Y;
            Z = minimized.Z;
        }

        public HexPoint Minimized
        {
            get
            {
                // TODO: Might be able to do this without as much work...
                //       Try to find the two axis the target is between. Those two will be the non-zero axis.

                // One of the following will be the min.
                var zeroX = new HexPoint(0, Y - X, Z + X);
                var zeroY = new HexPoint(X - Y, 0, Z + Y);
                var zeroZ = new HexPoint(X + Z, Y + Z, 0);

                var zeroXDist = zeroX.ManhattanDistance();
                var zeroYDist = zeroY.ManhattanDistance();
                var zeroZDist = zeroZ.ManhattanDistance();

                if (zeroXDist < zeroYDist)
                {
                    if (zeroXDist < zeroZDist)
                        return zeroX;
                    else
                        return zeroZ;
                }
                else
                {
                    if (zeroYDist < zeroZDist)
                        return zeroY;
                    else
                        return zeroZ;
                }
            }
        }
        
        /// <summary>
        /// Return the value of this in the specified direction.
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public int InDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.Right:
                    return X;
                case Direction.Left:
                    return -X;
                case Direction.Up:
                    return Y;
                case Direction.Down:
                    return -Y;
                case Direction.Forward:
                    return Z;
                case Direction.Backwards:
                    return -Z;
                default:
                    throw new NotSupportedException("Invalid direction");
            }
        }

        public static HexPoint operator +(HexPoint a, HexPoint b)
        {
            return new HexPoint(
                a.X + b.X,
                a.Y + b.Y,
                a.Z + b.Z);
        }

        public static HexPoint operator -(HexPoint a, HexPoint b)
        {
            return new HexPoint(
                a.X - b.X,
                a.Y - b.Y,
                a.Z - b.Z);
        }

        public static HexPoint operator *(HexPoint v, int s)
        {
            return new HexPoint(s * v.X, s * v.Y, s * v.Z);
        }

        public static HexPoint operator *(int s, HexPoint v) => v * s;

        public static bool operator ==(HexPoint a, HexPoint b) => a.Equals(b);
        public static bool operator !=(HexPoint a, HexPoint b) => !a.Equals(b);
    }
}
