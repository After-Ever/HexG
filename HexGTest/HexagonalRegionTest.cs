using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using HexG;

namespace HexGTest
{
    [TestClass]
    public class HexagonalRegionTest
    {
        [TestMethod]
        public void ContainsExpectedPoints()
        {
            var expected = new Region();

            expected.Add(new HexPoint(0, 0, 0));

            var h = new HexagonalRegion(0);

            Assert.IsTrue(expected.SetEquals(new HexagonalRegion(0)));

            expected.Add(new HexPoint(1, 0, 0));
            expected.Add(new HexPoint(0, 0, 1));
            expected.Add(new HexPoint(0, 1, 0));
            expected.Add(new HexPoint(-1, 0, 0));
            expected.Add(new HexPoint(0, 0, -1));
            expected.Add(new HexPoint(0, -1, 0));

            Assert.IsTrue(expected.SetEquals(new HexagonalRegion(1)));

            expected.Add(new HexPoint(2, 0, 0));
            expected.Add(new HexPoint(2, 1, 0));
            expected.Add(new HexPoint(0, 0, 2));
            expected.Add(new HexPoint(-1, 0, 2));
            expected.Add(new HexPoint(0, 2, 0));
            expected.Add(new HexPoint(0, 2, -1));
            expected.Add(new HexPoint(-2, 0, 0));
            expected.Add(new HexPoint(-2, -1, 0));
            expected.Add(new HexPoint(0, 0, -2));
            expected.Add(new HexPoint(1, 0, -2));
            expected.Add(new HexPoint(0, -2, 0));
            expected.Add(new HexPoint(0, -2, 1));

            Assert.IsTrue(expected.SetEquals(new HexagonalRegion(2)));
        }
    }
}
