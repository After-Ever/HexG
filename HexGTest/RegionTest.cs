using HexG;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace HexGTest
{
    [TestClass]
    public class RegionTest
    {
        [TestMethod]
        public void HandlesDuplicates()
        {
            var region = new Region();

            region.Add(new HexPoint(1, 2, 3));

            Assert.AreEqual(1, region.Count);

            region.Add(new HexPoint(2, 3, 2));

            Assert.AreEqual(1, region.Count);
            Assert.IsTrue(region.Remove(new HexPoint(10, 11, -6)));
            Assert.AreEqual(0, region.Count);
        }

        [TestMethod]
        public void MaxInDirection()
        {
            var region = new Region();

            region.Add(new HexPoint(-20, 10, 0));

            Assert.AreEqual(-20, region.MaxInDirection(Direction.Right));
            Assert.AreEqual(10, region.MaxInDirection(Direction.Up));
            Assert.AreEqual(0, region.MaxInDirection(Direction.Forward));

            region.Add(new HexPoint(0, 12, 5));

            Assert.AreEqual(0, region.MaxInDirection(Direction.Right));
            Assert.AreEqual(12, region.MaxInDirection(Direction.Up));
            Assert.AreEqual(5, region.MaxInDirection(Direction.Forward));

            region.Add(new HexPoint(2, 0, 6));

            Assert.AreEqual(2, region.MaxInDirection(Direction.Right));
            Assert.AreEqual(12, region.MaxInDirection(Direction.Up));
            Assert.AreEqual(6, region.MaxInDirection(Direction.Forward));
        }
    }
}
