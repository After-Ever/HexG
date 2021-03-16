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
            // TODO: Should probably have some tolerance...
            if (x + y != z)
                throw new Exception("These basis vectors don't meet requirements: " + x + " " + y + " " + z + " " + x + y);

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
    }
}
