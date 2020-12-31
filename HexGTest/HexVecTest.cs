using HexG;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace HexGTest
{
    [TestClass]
    public class HexVecTest
    {
        const double delta = 0.001;

        [TestMethod]
        public void ManhattanDistance()
        {
            foreach(var td in HexVecTestData.testData)
            {
                Assert.AreEqual(td.manhattanDistance, td.vec.ManhattanDistance(), delta);
            }
        }

        [TestMethod]
        public void Distance()
        {
            foreach (var td in HexVecTestData.testData)
            {
                Assert.AreEqual(td.distance, td.vec.Distance(HexVecTestData.testBasis), delta);
                
                // Other variants of the same vector should be the same distance.
                Assert.AreEqual(td.distance, td.vec.Standardized.Distance(HexVecTestData.testBasis), delta);
                Assert.AreEqual(td.distance, td.vec.Minimized.Distance(HexVecTestData.testBasis), delta);
            }
        }

        [TestMethod]
        public void Equality()
        {
            // First, some simple baselines.
            var a = new HexVec(1, 0, 0);
            var b = new HexVec(1, 0, 0);
            var c = new HexVec(0, 1, 0);
            var d = new HexVec(0, -1, 1);

            Assert.AreEqual(a, a);
            Assert.AreEqual(a, b);
            Assert.AreEqual(a, d);

            Assert.AreNotEqual(a, c);
            Assert.AreNotEqual(b, c);
            Assert.AreNotEqual(d, c);

            // Then go through all the test data, compairing variants of each.
            foreach (var td in HexVecTestData.testData)
            {
                Assert.AreEqual(td.vec, td.vec.Minimized);
                Assert.AreEqual(td.vec, td.vec.Standardized);
                Assert.AreEqual(td.vec.Standardized, td.vec.Minimized);
            }
        }

        [TestMethod]
        public void Standardize()
        {
            foreach (var td in HexVecTestData.testData)
            {
                Assert.AreEqual(td.vec.Standardized, td.standard);
            }
        }

        [TestMethod]
        public void Minimize()
        {
            foreach (var td in HexVecTestData.testData)
            {
                Assert.AreEqual(td.vec.Minimized, td.minimum);
            }
        }
    }
}
