using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace HexG
{
    //TODO: Add a check to ensure a given basis supports the required equality.
    public struct HexBasis
    {
        const float sqrt3over2 = 0.8660254037844386f;

        public Vector2 X, Y, Z;

        public static readonly HexBasis Standard = new HexBasis
        {
            X = new Vector2(1, 0),
            Y = new Vector2(-0.5f, sqrt3over2),
            Z = new Vector2(0.5f, sqrt3over2)
        };
    }
}
