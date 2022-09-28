using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace HexG
{
    public class HexBasis : IEquatable<HexBasis>
    {
        public Vector2 X, Y;

        /// <summary>
        /// The distance from a hex's center to the corner adjacent to
        /// <see cref="X"/> (ccw).
        /// 
        /// Used to disambiguate the shape of the hex, along with <see cref="armAngle"/>.
        /// 
        /// Set to zero by default.
        /// </summary>
        public float armLength;
        /// <summary>
        /// The angle between <see cref="X"/> and the vector from the hex's center
        /// to the corner adjacent to <see cref="X"/> (ccw).
        /// 
        /// Used to disambiguate the shape of the hex, along with <see cref="armLength"/>.
        /// 
        /// Set to zero by default.
        /// </summary>
        public float armAngle;

        public HexBasis(Vector2 x, Vector2 y, float armLength = 0, float armAngle = 0)
        {
            X = x;
            Y = y;
            this.armLength = armLength;
            this.armAngle = armAngle;
        }

        public const float sqrt3over2 = 0.8660254037844386f;
        public const float piOver6 = 0.52359877559f;
        public const float oneOverTwoCosPiOver6 = 0.57735026919f; // 1 / (2 * cos(pi/6))
        public static readonly HexBasis Standard = new HexBasis
        (
            x: new Vector2(1, 0),
            y: new Vector2(-0.5f, sqrt3over2),
            armLength: oneOverTwoCosPiOver6,
            armAngle: piOver6
        );

        public HexVec HexVecFromCartesian(Vector2 p)
            => new HexVec(this, p);

        public Vector2[] GetCorners()
        {
            var r = new Vector2[6];

            var b0n = X;
            b0n /= b0n.Length();

            r[0] = Rotate(b0n, armAngle) * armLength;
            r[1] = X - r[0];
            r[2] = -Y - r[1];
            r[3] = -r[0];
            r[4] = -r[1];
            r[5] = -r[2];

            return r;
        }

        Vector2 Rotate(Vector2 v, float a)
        {
            var ca = (float)Math.Cos(a);
            var sa = (float)Math.Sin(a);
            var r1 = new Vector2(ca, -sa);
            var r2 = new Vector2(sa, ca);

            return new Vector2(Vector2.Dot(v, r1), Vector2.Dot(v, r2));
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as HexBasis);
        }

        public bool Equals(HexBasis other)
        {
            return other != null &&
                   X.Equals(other.X) &&
                   Y.Equals(other.Y) &&
                   armLength == other.armLength &&
                   armAngle == other.armAngle;
        }

        public override int GetHashCode()
        {
            var hashCode = -1596527976;
            hashCode = hashCode * -1521134295 + EqualityComparer<Vector2>.Default.GetHashCode(X);
            hashCode = hashCode * -1521134295 + EqualityComparer<Vector2>.Default.GetHashCode(Y);
            hashCode = hashCode * -1521134295 + armLength.GetHashCode();
            hashCode = hashCode * -1521134295 + armAngle.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(HexBasis basis1, HexBasis basis2)
        {
            return EqualityComparer<HexBasis>.Default.Equals(basis1, basis2);
        }

        public static bool operator !=(HexBasis basis1, HexBasis basis2)
        {
            return !(basis1 == basis2);
        }
    }
}
