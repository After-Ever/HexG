using System;
using System.Collections.Generic;
using System.Text;
using HexG;

namespace HexGTest
{
    public struct HexVecTestData
    {
        public HexVec vec;
        public HexVec standard;
        public HexVec minimum;
        public float distance;
        public float manhattanDistance;

        // TODO: Add much more test data!
        public static HexVecTestData[] testData =
        {
            new HexVecTestData
            {
                vec = new HexVec(1, 0, 0),
                standard = new HexVec(1, 0, 0),
                minimum = new HexVec(1, 0, 0),
                distance = 1,
                manhattanDistance = 1,
            },
            new HexVecTestData
            {
                vec = new HexVec(0, 1, 0),
                standard = new HexVec(0, 1, 0),
                minimum = new HexVec(0, 1, 0),
                distance = 1,
                manhattanDistance = 1,
            },
            new HexVecTestData
            {
                vec = new HexVec(0, 0, 1),
                standard = new HexVec(1, 1, 0),
                minimum = new HexVec(0, 0, 1),
                distance = 1,
                manhattanDistance = 1,
            },
        };
    }
}
