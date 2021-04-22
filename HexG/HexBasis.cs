using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace HexG
{
    public class HexBasis
    {
        public Vector2 X, Y, Z;

        public HexBasis(Vector2 x, Vector2 y, Vector2 z)
        {

            var s = x + y;
            // TODO: Should probably have some tolerance...
            if (s != z)
                throw new Exception("These basis vectors don't meet requirements: " +
                    "X(" + x + ") + Y(" + y + ") != Z(" + z + ")");

            X = x;
            Y = y;
            Z = z;
        }

        const float sqrt3over2 = 0.8660254037844386f;
        public static readonly HexBasis Standard = new HexBasis
        (
            x: new Vector2(1, 0),
            y: new Vector2(-0.5f, sqrt3over2),
            z: new Vector2(0.5f, sqrt3over2)
        );

        public override bool Equals(object obj)
        {
            return obj is HexBasis basis &&
                   X.Equals(basis.X) &&
                   Y.Equals(basis.Y) &&
                   Z.Equals(basis.Z);
        }

        public override int GetHashCode()
        {
            int hashCode = -307843816;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            hashCode = hashCode * -1521134295 + Z.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(HexBasis left, HexBasis right)
        {
            return EqualityComparer<HexBasis>.Default.Equals(left, right);
        }

        public static bool operator !=(HexBasis left, HexBasis right)
        {
            return !(left == right);
        }
    }
}
