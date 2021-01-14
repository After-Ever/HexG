using System;
using System.Numerics;

namespace HexG
{
    /// <summary>
    /// Represents a hex vector, which is a linear combination of some hex-grid basis vectors (XAxis, YAxis, and ZAxis).
    /// XAxis + YAxis = ZAxis must be true for the basis vectors.
    /// This means each HexVec belongs to an equivalence class of the form: (x - c, y - c, c) where c is any real number.
    /// When the c is zero, we call this a standard HexVec.
    /// 
    /// The third value is included to allow for more convinient specification, and to allow the minimum manhattan distance
    /// to always be expressible.
    /// </summary>
    public struct HexVec
    {
        public float X, Y, Z;

        /// <summary>
        /// Construct a new HexVec with the given values and basis.
        /// If <paramref name="basis"/> is null, then <see cref="HexBasis.Standard"/> will be used.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public HexVec(float x = 0, float y = 0, float z = 0)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public HexVec(Vector3 p)
        {
            X = p.X;
            Y = p.Y;
            Z = p.Z;
        }

        public HexVec(HexBasis basis, float cartesianX, float cartesianY)
            : this(basis, new Vector2(cartesianX, cartesianY)) { }

        /// <summary>
        /// Convert from a cartesian vector to a HexVec, based on <paramref name="basis"/>.
        /// </summary>
        /// <param name="basis"></param>
        /// <param name="cartesianVec"></param>
        public HexVec(HexBasis basis, Vector2 cartesianVec)
        {
            var det = basis.X.X * basis.Y.Y - basis.X.Y * basis.Y.X;
            if (det == 0)
                throw new DivideByZeroException("Basis vectors have determinant = 0.");

            var invDet = 1 / det;
            var invXYbasisRow1 = invDet * new Vector2(basis.Y.Y, -basis.Y.X);
            var invXYbasisRow2 = invDet * new Vector2(basis.X.Y, basis.X.X);

            var hexVec = new Vector2(Vector2.Dot(invXYbasisRow1, cartesianVec), Vector2.Dot(invXYbasisRow2, cartesianVec));

            X = hexVec.X;
            Y = hexVec.Y;
            Z = 0;
        }

        public HexPoint ToNearestPoint()
            => new HexPoint((int)Math.Round(X), (int)Math.Round(Y), (int)Math.Round(Z));

        public Vector2 ToCartesian(HexBasis basis)
            => X * basis.X + Y * basis.Y + Z * basis.Z;

        /// <summary>
        /// The sum of the absolute values of each coordinate.
        /// Notably this is not necessarily the minimum! Use <see cref="Minimize"/> to ensure min.
        /// </summary>
        public float ManhattanDistance => Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);
        public float MinManhattanDistance => Minimized.ManhattanDistance;

        public override bool Equals(object obj)
        {
            if (!(obj is HexVec v))
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
        /// Standardize this HexVec.
        /// A Standard HexVec is one where z == 0.
        /// </summary>
        public void Standardize()
        {
            X += Z;
            Y += Z;
            Z = 0;
        }

        /// <summary>
        /// Return a new HexVec which has been standardized.
        /// A Standard HexVec is one where z == 0.
        /// </summary>
        /// <returns></returns>
        public HexVec Standardized
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
        /// Makes this the HexVec the smallest mannhattan distance from this HexVec's equivalence class.
        /// </summary>
        public void Minimize()
        {
            var minimized = Minimized;

            X = minimized.X;
            Y = minimized.Y;
            Z = minimized.Z;
        }

        public HexVec Minimized
        {
            get
            {
                // TODO: Might be able to do this without as much work...
                //       Try to find the two axis the target is between. Those two will be the non-zero axis.

                // One of the following will be the min.
                var zeroX = new HexVec(0, Y - X, Z + X);
                var zeroY = new HexVec(X - Y, 0, Z + Y);
                var zeroZ = new HexVec(X + Z, Y + Z, 0);

                var zeroXDist = zeroX.ManhattanDistance;
                var zeroYDist = zeroY.ManhattanDistance;
                var zeroZDist = zeroZ.ManhattanDistance;

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
        public float InDirection(Direction direction)
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

        public static HexVec operator +(HexVec a, HexVec b)
        {
            return new HexVec(
                a.X + b.X,
                a.Y + b.Y,
                a.Z + b.Z);
        }

        public static HexVec operator -(HexVec a, HexVec b)
        {
            return new HexVec(
                a.X - b.X,
                a.Y - b.Y,
                a.Z - b.Z);
        }

        public static HexVec operator *(HexVec v, float s)
        {
            return new HexVec(s * v.X, s * v.Y, s * v.Z);
        }

        public static HexVec operator *(float s, HexVec v) => v * s;
        public static HexVec operator /(HexVec v, float s) => v * (1 / s);

        public static bool operator ==(HexVec a, HexVec b) => a.Equals(b);
        public static bool operator !=(HexVec a, HexVec b) => !a.Equals(b);
    }
}
