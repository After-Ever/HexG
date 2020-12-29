using System;
using System.Numerics;

namespace HexG
{
    /// <summary>
    /// Represents a hex vector, which is a linear combination of the standard basis vectors (XAxis, YAxis, and ZAxis).
    /// This creates the equivelance relation: (1,1,0) == (0,0,1), which can be seen from the basis vectors,
    /// so each HexVec belongs to an equivalence class of the form: (x - c, y - c, c) where c is any real number.
    /// When the Z value is zero, we call this a standard HexVec.
    /// 
    /// The third value is included to allow for more convinient specification, and to allow the minimum manhattan distance
    /// to always be expressible.
    /// </summary>
    public struct HexVec
    {
        public readonly HexBasis basis;
        Vector3 vec;

        /// <summary>
        /// Construct a new HexVec with the given values and basis.
        /// If <paramref name="basis"/> is null, then <see cref="HexBasis.Standard"/> will be used.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="basis"></param>
        public HexVec(float x = 0, float y = 0, float z = 0, HexBasis? basis = null)
        {
            this.basis = basis ?? HexBasis.Standard;
            vec = new Vector3(x,y,z);
        }

        public HexVec(Vector3 p, HexBasis? basis = null)
        {
            this.basis = basis ?? HexBasis.Standard;
            vec = new Vector3(p.X, p.Y, p.Z);
        }

        public float X
        {
            get => vec.X;
            set
            {
                vec.X = value;
            }
        }
        public float Y
        {
            get => vec.Y;
            set
            {
                vec.Y = value;
            }
        }
        public float Z
        {
            get => vec.Z;
            set
            {
                vec.Z = value;
            }
        }

        /// <summary>
        /// The straight line distance to the target.
        /// </summary>
        public float Distance()
        {
            var a = basis.X * vec.X;
            var b = basis.Y * vec.Y;
            var c = basis.Z * vec.Z;

            return (a + b + c).Length();
        }

        /// <summary>
        /// The sum of the absolute values of each coordinate.
        /// Notably this is not necessarily the minimum! Use <see cref="Minimize"/> to ensure min.
        /// </summary>
        public float ManhattanDistance() => Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);

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
            // TODO: Might be able to do this without as much work...
            //       Try to find the two axis the target is between. Those two will be the non-zero axis.

            // One of the following will be the min.
            var zeroX = new HexVec(0, Y - X, Z + X);
            var zeroY = new HexVec(X - Y, 0, Z + Y);
            var zeroZ = new HexVec(X + Z, Y + Z, 0);

            var zeroXDist = zeroX.ManhattanDistance();
            var zeroYDist = zeroY.ManhattanDistance();
            var zeroZDist = zeroZ.ManhattanDistance();

            if (zeroXDist < zeroYDist)
            {
                if (zeroXDist < zeroZDist)
                    vec = zeroX.vec;
                else
                    vec = zeroZ.vec;
            }
            else
            {
                if (zeroYDist < zeroZDist)
                    vec = zeroY.vec;
                else
                    vec = zeroZ.vec;
            }
        }

        public HexVec Minimized
        {
            get
            {
                // Make a copy.
                var r = this;
                r.Minimize();
                return r;
            }
        }

        public static HexVec operator +(HexVec a, HexVec b)
        {
            return new HexVec(a.vec + b.vec);
        }

        public static HexVec operator -(HexVec a, HexVec b)
        {
            return new HexVec(a.vec - b.vec);
        }

        public static HexVec operator *(HexVec v, float s)
        {
            return new HexVec(v.vec * s);
        }

        public static HexVec operator *(float s, HexVec v) => v * s;
        public static HexVec operator /(HexVec v, float s) => v * (1 / s);

        public static bool operator ==(HexVec a, HexVec b) => a.Equals(b);
        public static bool operator !=(HexVec a, HexVec b) => !a.Equals(b);
    }
}
