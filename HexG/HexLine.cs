using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace HexG
{
    public struct LinePoint : IEnumerable<HexPoint>
    {
        internal readonly HexPoint a;
        internal readonly HexPoint? b;

        public LinePoint(HexPoint a, HexPoint? b = null)
        {
            this.a = a;
            this.b = b;
        }

        public IEnumerator<HexPoint> GetEnumerator()
            => Enumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        IEnumerable<HexPoint> Enumerable()
        {
            yield return a;
            if (b != null)
                yield return b.Value;
        }

        public static implicit operator LinePoint(HexPoint p)
            => new LinePoint(p);
    }

    /// <summary>
    /// A line of <see cref="HexPoint"/>s between two points.
    /// 
    /// Sometimes, two points will be equally close to the line, and so each will be provided,
    /// hense why <see cref="LinePoint"/> is an <see cref="IEnumerable"/>. Typically, either
    /// point can be considered "within" the line for things like line of sight.
    /// </summary>
    public class HexLine : IEnumerable<LinePoint>
    {
        public readonly HexPoint start;
        public readonly HexPoint end;
        /// <summary>
        /// How much the distance from the line two points can vary, 
        /// and be considered the same distance.
        /// </summary>
        public readonly float sameDistanceTolerance;

        readonly List<HexPoint> directionVectors;

        public HexLine(HexPoint start, HexPoint end, float sameDistanceTolerance = 0.1f)
        {
            this.start = start;
            this.end = end;
            this.sameDistanceTolerance = sameDistanceTolerance;

            if (start != end)
                directionVectors = (end - start).ToVec()
                    .ContainingDirections()
                    .Select(dir => dir.ToHexPoint())
                    .ToList();
        }

        public IEnumerator<LinePoint> GetEnumerator()
            => Enumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public int MaxInDirection(Direction direction)
            => Math.Max(start.InDirection(direction), end.InDirection(direction));

        IEnumerable<LinePoint> Enumerable()
        {
            yield return start;
            if (start == end)
                yield break;

            var p = new LinePoint(start);
            do
            {
                var nextPointsToConsider = p
                    .SelectMany(b => directionVectors
                        .Select(d =>
                        {
                            var point = b + d;
                            var dist = DistanceFromLine(point);
                            return (point, dist);
                        }))
                    .ToList();

                nextPointsToConsider.Sort((a, b) => Math.Sign(a.Item2 - b.Item2));

                if (nextPointsToConsider.Count > 1
                    && Math.Abs(nextPointsToConsider[0].Item2 - nextPointsToConsider[1].Item2) <= sameDistanceTolerance)
                {
                    yield return p = new LinePoint(
                        nextPointsToConsider[0].Item1,
                        nextPointsToConsider[1].Item1);
                }
                else
                {
                    yield return p = nextPointsToConsider[0].Item1;
                }
            } 
            while (p.a != end);
        }

        // This is not euclidean distance, but rather a skewed distance.
        // Comparing results should be that same as comparing the actual distance.
        float DistanceFromLine(HexPoint p)
        {
            var lineHexVec = end - start;
            lineHexVec.Minimize();
            var pointHexVec = p - start;
            pointHexVec.Minimize();

            // Convert from the redundant "Hex space" to a skewed euclidean space (affine).
            Vector2 lineVec;
            Vector2 pointVec;
            if (lineHexVec.X == 0)
            {
                lineVec.X = lineHexVec.Y;
                lineVec.Y = lineHexVec.Z;
                pointVec.X = pointHexVec.Y;
                pointVec.Y = pointHexVec.Z;
            }
            else if (lineHexVec.Y == 0)
            {
                lineVec.X = lineHexVec.X;
                lineVec.Y = lineHexVec.Z;
                pointVec.X = pointHexVec.X;
                pointVec.Y = pointHexVec.Z;
            }
            else
            {
                lineVec.X = lineHexVec.X;
                lineVec.Y = lineHexVec.Y;
                pointVec.X = pointHexVec.X;
                pointVec.Y = pointHexVec.Y;
            }

            var lineNorm = Vector2.Normalize(lineVec);
            return (pointVec - Vector2.Dot(pointVec, Vector2.Normalize(lineVec)) * lineNorm).Length();
        }
    }
}
