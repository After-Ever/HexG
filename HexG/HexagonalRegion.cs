using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HexG
{
    public class RectangularRegion : IReadOnlyRegion
    {
        public readonly HexPoint start;
        public readonly int mainAxisSize;
        public readonly int crossAxisSize;

        readonly Direction mainAxis;
        readonly Direction crossAxis;
        readonly HexPoint crossOffset;

        public int Count => mainAxisSize * crossAxisSize;

        public RectangularRegion(
            HexPoint start, 
            int mainAxisSize, 
            int crossAxisSize, 
            Direction mainAxis, 
            bool ccwCrossAxis, 
            bool startClose)
        {
            this.start = start;
            this.mainAxisSize = mainAxisSize;
            this.crossAxisSize = crossAxisSize;

            this.mainAxis = mainAxis;
            if (startClose)
            {
                crossAxis = ccwCrossAxis
                    ? mainAxis.Rotate(DirectionOffset.OneCCW)
                    : mainAxis.Rotate(DirectionOffset.OneCW);

                crossOffset = -this.mainAxis.ToHexPoint();
            }
            else
            {
                crossAxis = ccwCrossAxis
                    ? mainAxis.Rotate(DirectionOffset.TwoCCW)
                    : mainAxis.Rotate(DirectionOffset.TwoCW);

                crossOffset = this.mainAxis.ToHexPoint();
            }
        }

        public bool Contains(HexPoint point)
        {
            var fromStart = point - start;
            var inCross = fromStart.InDirection(crossAxis);
            if (inCross < 0 || inCross >= crossAxisSize)
                return false;

            var minPoint = crossAxis.ToHexPoint() * inCross + crossOffset * (inCross / 2);
            var minOnMain = minPoint.InDirection(mainAxis);
            var inMain = fromStart.InDirection(mainAxis);

            return inMain >= minOnMain && inMain < minOnMain + mainAxisSize;
        }

        public int MaxInDirection(Direction direction)
        {
            throw new NotImplementedException("I sure hope you don't need this...");
        }


        public IEnumerator<HexPoint> GetEnumerator()
            => Enumerate().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        IEnumerable<HexPoint> Enumerate()
        {
            for (int c = 0; c < crossAxisSize; ++c)
            {
                var basePoint = start + crossAxis.ToHexPoint() * c + crossOffset * (c / 2);
                for (int m = 0; m < mainAxisSize; ++m)
                {
                    yield return basePoint + mainAxis.ToHexPoint() * m;
                }
            }
        }
    }

    public class HexagonalRegion : IReadOnlyRegion
    {
        public int Count => 1 + 3 * radius * (radius + 1);

        int radius;
        HexPoint offset;

        /// <summary>
        /// Create a region in a hexagonal shape, around <paramref name="offset"/>.
        /// </summary>
        /// <param name="radius">How many hexes from <paramref name="offset"/> does this region extend.
        /// A radius of 0 is a single hex.</param>
        /// <param name="offset"></param>
        public HexagonalRegion(int radius, HexPoint? offset = null)
        {
            if (radius < 0)
                throw new Exception("Radius must be non-negative.");

            this.radius = radius;
            this.offset = offset ?? new HexPoint();
        }

        public bool Contains(HexPoint item)
        {
            var toItem = (item - offset).Minimized;
            var md = toItem.ManhattanDistance;

            return md <= radius;
        }

        public IEnumerator<HexPoint> GetEnumerator() => new Enumerator(offset, radius);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int MaxInDirection(Direction direction) => radius;

        internal class Enumerator : IEnumerator<HexPoint>
        {
            readonly HexPoint origin;
            readonly int radius;
            readonly Direction start;
            readonly Direction wrapStartDir;
            readonly int angles;

            HexPoint curPoint;
            // The current ring.
            // Once > radius, enumeration is complete.
            int currentRing;
            // Stretches left in current ring.
            // Resets to angles on wrap.
            int stretchesLeftInRing;
            // Hexes left in the current stretch of the current ring.
            // Resets to currentRing on wrap.
            int hexesLeftInStretch;
            IEnumerator<Direction> directionToNext; 

            public HexPoint Current => currentRing <= radius ? curPoint : throw new Exception();
            object IEnumerator.Current => Current;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="origin">Center of region.</param>
            /// <param name="radius">How many rings out to go.</param>
            /// <param name="start">Direction vector to start rings from.</param>
            /// <param name="angles">How many 60 degree sections to span rings.</param>
            public Enumerator(
                HexPoint origin,
                int radius,
                Direction start = Direction.Right,
                int angles = 6)
            {
                this.origin = origin;
                this.radius = radius;
                this.angles = angles;
                this.start = start;

                wrapStartDir = HexDirection.DirectionsFromCCW(start).ElementAt(2);
                Reset();
            }

            public void Dispose() { }

            public bool MoveNext()
            {
                if (hexesLeftInStretch-- == 0)
                {
                    if (--stretchesLeftInRing == 0)
                    {
                        if (++currentRing > radius)
                            return false;

                        curPoint = origin + start.ToHexPoint() * currentRing;
                        stretchesLeftInRing = angles;
                        hexesLeftInStretch = currentRing;
                        directionToNext = HexDirection.DirectionsFromCCW(wrapStartDir).GetEnumerator();
                        directionToNext.MoveNext();
                        return true;
                    }

                    hexesLeftInStretch = currentRing - 1;
                    if (!directionToNext.MoveNext())
                        throw new Exception("Should never be reached.");
                }

                curPoint += directionToNext.Current.ToHexPoint();
                return true;
            }

            public void Reset()
            {
                currentRing = 0;
                stretchesLeftInRing = 1;
                hexesLeftInStretch = 1;
                directionToNext = HexDirection.DirectionsFromCCW(wrapStartDir).GetEnumerator();
                directionToNext.MoveNext();

                curPoint = origin;
                curPoint -= wrapStartDir.ToHexPoint();
            }
        }
    }
}
