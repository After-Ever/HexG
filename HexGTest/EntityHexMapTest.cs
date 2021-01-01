using HexG;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace HexGTest
{
    [TestClass]
    public class EntityHexMapTest
    {
        [TestMethod]
        public void Add()
        {
            var map = new EntityHexMap(new HashHexMap<MapEntity>());

            Assert.IsNull(map[HexPoint.Zero]);

            map.Add(new MapEntity(), HexPoint.Zero);

            Assert.IsNotNull(map[HexPoint.Zero]);

            // Now an entity adding itself.
            Assert.IsNull(map[new HexPoint(1, 0, 0)]);

            new MapEntity().AddTo(map, new HexPoint(1, 0, 0));

            Assert.IsNotNull(map[new HexPoint(1, 0, 0)]);
        }

        [TestMethod]
        public void Move()
        {
            var map = new EntityHexMap(new HashHexMap<MapEntity>());
            var entity = new MapEntity();
            map.Add(entity, HexPoint.Zero);

            map.Move(HexPoint.Zero, new HexPoint(1, 2, 3));

            Assert.IsNull(map[HexPoint.Zero]);
            Assert.AreEqual(entity, map[new HexPoint(1, 2, 3)]);

            // Now an entity moving itself.
            entity.MoveTo(new HexPoint(-1, 0, 0));

            Assert.IsNull(map[new HexPoint(1, 2, 3)]);
            Assert.AreEqual(entity, map[new HexPoint(-1, 0, 0)]);
        }

        [TestMethod]
        public void AddReplaces()
        {
            var map = new EntityHexMap(new HashHexMap<MapEntity>());

            MapEntity entity = new MapEntity();
            MapEntity replaced;
            map.Add(entity, HexPoint.Zero, out replaced);

            Assert.IsNull(replaced);

            map.Add(new MapEntity(), HexPoint.Zero, out replaced);

            Assert.AreEqual(entity, replaced);
        }

        [TestMethod]
        public void MoveReplaces()
        {
            var a = new MapEntity();
            var b = new MapEntity();
            var map = new EntityHexMap(new HashHexMap<MapEntity>());

            map.Add(a, HexPoint.Zero);
            map.Add(b, new HexPoint(1, 0, 0));

            MapEntity moved, replaced;

            var moveSucceded = map.Move(HexPoint.Zero, new HexPoint(1, 0, 0), out moved, out replaced);
            Assert.IsTrue(moveSucceded);

            Assert.AreEqual(a, moved);
            Assert.AreEqual(b, replaced);
        }
    }
}
