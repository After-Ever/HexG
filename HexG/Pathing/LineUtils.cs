using System;
using System.Collections.Generic;
using System.Linq;

using HexG;

namespace HexG.Pathing
{
    public static class LineUtils
    {
        public static bool IsClear(
            this HexLine line,
            IReadOnlyRegion allowed = null,
            IReadOnlyRegion disallowed = null)
        {
            if (allowed == null
                && disallowed == null)
                return true;

            var furthest = line.FurthestClearPoint(allowed, disallowed);
            return furthest.First() == line.end;
        }

        public static LinePoint FurthestClearPoint(
            this HexLine line,
            IReadOnlyRegion allowed = null,
            IReadOnlyRegion disallowed = null)
        {
            if (allowed == null
                && disallowed == null)
                return line.end;
            return line.WhileClear(allowed, disallowed).Last();
        }

        public static IEnumerable<LinePoint> WhileClear(
            this HexLine line,
            IReadOnlyRegion allowed = null,
            IReadOnlyRegion disallowed = null)
        {
            if (allowed == null
                && disallowed == null)
                foreach (var l in line)
                    yield return l;

            foreach (var l in line)
            {
                var good = GoodPoint(l, allowed, disallowed);
                if (good == null)
                    break;
                yield return good.Value;
            }
        }

        static LinePoint? GoodPoint(
            LinePoint linePoint,
            IReadOnlyRegion allowed = null,
            IReadOnlyRegion disallowed = null)
        {
            var goodPoints = linePoint.Where(p => IsPointGood(p, allowed, disallowed)).ToList();

            switch (goodPoints.Count)
            {
                case 0:
                    return null;
                case 1:
                    return new LinePoint(goodPoints[0]);
                case 2:
                    return new LinePoint(goodPoints[0], goodPoints[1]);
                default:
                    throw new NotSupportedException();
            }
        }

        static bool IsPointGood(
            HexPoint p,
            IReadOnlyRegion allowed = null,
            IReadOnlyRegion disallowed = null)
            => (allowed?.Contains(p) ?? true)
            && !(disallowed?.Contains(p) ?? false);
    }
}
