using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HexG;

namespace HexG.Pathing
{
    public class HexPath : IEnumerable<HexPoint>
    {
        public HexPoint Origin;
        public HexPoint Destination { get; private set; }
        readonly List<(Direction, int)> stretches;

        public IReadOnlyList<(Direction, int)> Stretches => stretches;

        /// <summary>
        /// How many <see cref="HexPoint"/>s are contained in the path.
        /// </summary>
        public int Count => TotalDist + 1;
        /// <summary>
        /// The total distance of the path.
        /// </summary>
        public int TotalDist => stretches.Aggregate(0, (total, stretch) => total + stretch.Item2);

        public HexPath(
            HexPoint origin,
            List<(Direction, int)> stretches = null)
        {
            Origin = origin;
            this.stretches = stretches ?? new List<(Direction, int)>();
            Destination = Points().Last();
        }

        public HexPath(IEnumerable<HexPath> paths)
        {
            if (!paths.Any())
                throw new Exception("Must include at least one path.");

            Origin = paths.First().Origin;
            stretches = new List<(Direction, int)>();

            var p = Origin;

            foreach (var path in paths)
            {
                if (p != path.Origin)
                    throw new Exception("All path ends must meet.");

                foreach ((var dir, var len) in path.stretches)
                {
                    stretches.Add((dir, len));
                    p += dir.ToHexPoint() * len;
                }
            }

            Destination = p;
        }

        /// <summary>
        /// Return a new path starting at <paramref name="start"/>,
        /// spanning the min of <paramref name="maxLength"/> and <see cref="Count"/> hexes (past the first).
        /// </summary>
        /// <param name="maxLength"></param>
        /// <param name="start">The hex index to start the path from.</param>
        /// <returns></returns>
        public HexPath Partial(int maxLength, int start = 0)
        {
            HexPoint origin;
            var stretches = GetPartialStretches(maxLength, out origin, start);
            return new HexPath(origin, stretches);
        }

        List<(Direction, int)> GetPartialStretches(
            int maxLength,
            out HexPoint newOrigin,
            int start = 0)
        {
            var l = new List<(Direction, int)>();

            var length = 0;
            newOrigin = Origin;
            foreach (var stretch in stretches)
            {
                if (start >= 0)
                {
                    if (start >= stretch.Item2)
                    {
                        start -= stretch.Item2;
                        newOrigin += stretch.Item1.ToHexPoint() * stretch.Item2;
                    }
                    else
                    {
                        newOrigin += stretch.Item1.ToHexPoint() * start;
                        length = Math.Min(stretch.Item2 - start, maxLength);
                        l.Add((stretch.Item1, length));
                        if (length == maxLength)
                            break;
                        start = -1;
                    }

                    continue;
                }

                if (stretch.Item2 + length > maxLength)
                {
                    l.Add((stretch.Item1, maxLength - length));
                    break;
                }
                l.Add(stretch);
                length += stretch.Item2;
                if (length == maxLength)
                    break;
            }

            return l;
        }

        /// <summary>
        /// Add a stretch to the path.
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="length"></param>
        public void AddStretch(Direction direction, int length)
        {
            if (length == 0)
                throw new ArgumentOutOfRangeException("Length must be non-zero.");

            stretches.Add((direction, length));
            Destination += direction.ToHexPoint() * length;
        }

        /// <summary>
        /// Clear the path. The origin will be maintained.
        /// </summary>
        public void Clear()
        {
            stretches.Clear();
            Destination = Origin;
        }

        /// <summary>
        /// Returns all ends of the stretches of this path.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<HexPoint> StretchPoints()
        {
            yield return Origin;

            var point = Origin;
            foreach ((var dir, var length) in stretches)
            {
                yield return point += dir.ToHexPoint() * length;
            }
        }


        // IEnumerable:

        public IEnumerator<HexPoint> GetEnumerator()
            => Points().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public IEnumerable<HexPoint> Points()
        {
            yield return Origin;

            var point = Origin;

            for (int i = 0; i < stretches.Count; ++i)
            {
                (var dir, var length) = stretches[i];
                var dirVec = dir.ToHexPoint();
                for (int j = 0; j < length; ++j)
                {
                    yield return point += dirVec;
                }
            }
        }
    }
}
