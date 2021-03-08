using System;
using System.Collections.Generic;
using System.Linq;
using HexG;
using FibonacciHeap;

namespace HexG.Pathing
{
    public static class Pathfinder
    {
        public class AStarKey : IComparable<AStarKey>
        {
            public readonly int heuristic;
            public int distance;
            public bool searched = false;

            public AStarKey(int heuristic, int distance)
            {
                this.heuristic = heuristic;
                this.distance = distance;
            }

            public int CompareTo(AStarKey other)
                => distance + heuristic - other.distance - other.heuristic;
        }

        /// <summary>
        /// Enumerates through an A-Star search, emitting a
        /// <typeparamref name="NodeData"/> as they are searched.
        /// 
        /// The order of events is as follows:
        /// 1. Take the next node, emit it.
        /// 2. Run <paramref name="onNode"/> on the data. For each data returned,
        ///    the distance from the current node will be calculated with <paramref name="distanceCalculator"/>.
        ///   a. Those that have already been found will have their keys updated if a shorter path has been found.
        ///   b. New data will be added to the queue.
        /// </summary>
        /// <typeparam name="NodeData"></typeparam>
        /// <param name="start">The first data to search.</param>
        /// <param name="maxDistance"></param>
        /// <param name="heuristicCalculator"></param>
        /// <param name="distanceCalculator"></param>
        /// <param name="onNode"></param>
        /// <param name="connected">Called when two nodes are "connected" in the search.
        /// The first argument will be the "from" node. The connection is not necessarily final; a
        /// path with a shorter connection may be found.</param>
        /// <param name="shouldStop"></param>
        /// <returns></returns>
        public static IEnumerable<(NodeData, AStarKey)> AStarSearch<NodeData>(
            NodeData start,
            int maxDistance,
            Func<NodeData, int> heuristicCalculator,
            // TODO Something on whether it should stop.
            Func<NodeData, NodeData, int> distanceCalculator,
            Func<NodeData, IEnumerable<NodeData>> onNode,
            Action<NodeData, NodeData> connected = null)
        {
            // Map of visited nodes to whether they have been searched.
            Dictionary<NodeData, FibonacciHeapNode<NodeData, AStarKey>> visited
                = new Dictionary<NodeData, FibonacciHeapNode<NodeData, AStarKey>>();
            var queue = new FibonacciHeap<NodeData, AStarKey>(new AStarKey(0, 0));
            var startHeuristic = heuristicCalculator(start);
            var startNode = new FibonacciHeapNode<NodeData, AStarKey>(start, new AStarKey(startHeuristic, 0));
            queue.Insert(startNode);
            visited[start] = startNode;

            while (!queue.IsEmpty())
            {
                var current = queue.RemoveMin();
                current.Key.searched = true;

                yield return (current.Data, current.Key);

                var connectedToCurrent = onNode(current.Data);
                foreach (var nodeData in connectedToCurrent)
                {
                    var distTo = distanceCalculator(current.Data, nodeData) + current.Key.distance;
                    if (distTo > maxDistance)
                        continue;

                    if (visited.TryGetValue(nodeData, out var node))
                    {
                        if (node.Key.searched)
                            continue;

                        if (distTo < node.Key.distance)
                        {
                            var nKey = new AStarKey(node.Key.heuristic, distTo);
                            queue.DecreaseKey(node, nKey);

                            connected?.Invoke(current.Data, nodeData);
                        }
                    }
                    else
                    {
                        var heuristic = heuristicCalculator(nodeData);
                        var nKey = new AStarKey(heuristic, distTo);
                        var nNode = new FibonacciHeapNode<NodeData, AStarKey>(nodeData, nKey);
                        visited[nodeData] = nNode;

                        queue.Insert(nNode);

                        connected?.Invoke(current.Data, nodeData);
                    }
                }
            }
        }


        class HexInfo
        {
            public readonly HexPoint point;
            public HexInfo parent;

            public HexInfo(
                HexPoint point)
            {
                this.point = point;
            }

            public void UpdateStretchToParent(HexInfo parent)
                => this.parent = parent;
        }

        // TODO: This is getting bloated, handling a lot more than just connecting points A and B.
        //   Should write more general traversal algos, and make this a more proper utility class...
        /// <summary>
        /// Find a path between <paramref name="origin"/> and <paramref name="destination"/>.
        /// 
        /// If no path can be found, null is returned.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="destination">A region where any contained point can be the destination.
        /// The actual destination point will be the one reachable by the shortest path.</param>
        /// <param name="maxDistance">the max length of the returned path.</param>
        /// <param name="allowedRegion"></param>
        /// <param name="disallowedRegion"></param>
        /// <param name="exceptEnd">When true, the end of the path is allowed to
        /// be outside the allowed region, and inside the disallowed region.</param>
        /// <param name="acceptPartial">When true, even if <paramref name="destination"/> is
        /// not found, will return the path which goes <paramref name="maxDistance"/>, and is
        /// closest to <paramref name="destination"/>.</param>
        /// <param name="onReachable">If provided, this will be called for each reachable
        /// point, until <paramref name="destination"/> is reached.</param>
        /// <returns></returns>
        public static HexPath FindPath(
            HexPoint origin,
            IReadOnlyRegion destination,
            int maxDistance,
            IReadOnlyRegion allowedRegion = null,
            IReadOnlyRegion disallowedRegion = null,
            bool exceptStart = true,
            bool exceptEnd = true,
            bool acceptPartial = false)
        {
            if (!exceptStart
                && !(allowedRegion?.Contains(origin) ?? true)
                && (disallowedRegion?.Contains(origin) ?? false))
                throw new Exception("The start of the path is in disallowed region, and exceptStart is false.");

            var search = AStarSearch(
                start: new HexInfo(origin),
                maxDistance: maxDistance,
                heuristicCalculator: info => GetHeuristic(info.point, destination),
                distanceCalculator: (a, b) => (a.point - b.point).MinManhattanDistance,
                onNode: info => new AdjacentRegion(info.point)
                    .Where(p => (destination.Contains(p) && exceptEnd)
                                || ((allowedRegion?.Contains(p) ?? true)
                                    && !(disallowedRegion?.Contains(p) ?? false)))
                    .Select(p => new HexInfo(p)),
                connected: (a, b) =>
                {
                    b.UpdateStretchToParent(a);
                });

            (HexInfo, AStarKey) closest = (null, null);
            HexInfo dest = null;
            foreach ((var hexInfo, var aStarKey) in search)
            {
                if (destination.Contains(hexInfo.point))
                {
                    dest = hexInfo;
                    break;
                }

                if (closest.Item1 == null
                    || aStarKey.distance > closest.Item2.distance
                    || (aStarKey.distance == closest.Item2.distance
                        && aStarKey.heuristic < closest.Item2.heuristic))
                    closest = (hexInfo, aStarKey);
            }

            if (dest != null)
                return UnWrapPathFind(dest, origin);

            if (acceptPartial)
                return UnWrapPathFind(closest.Item1, origin);
            
            // No path could be found.
            return null;
        }

        /// <summary>
        /// Find a path between <paramref name="origin"/> and <paramref name="destination"/>.
        /// 
        /// If no path can be found, null is returned.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="destination"></param>
        /// <param name="maxDistance">the max length of the returned path.</param>
        /// <param name="allowedRegion"></param>
        /// <param name="disallowedRegion"></param>
        /// <param name="exceptEnds">When true, the ends of the path are allowed to
        /// be outside the allowed region, and inside the disallowed region.</param>
        /// <returns></returns>
        public static HexPath FindPath(
            HexPoint origin,
            HexPoint destination,
            int maxDistance,
            IReadOnlyRegion allowedRegion = null,
            IReadOnlyRegion disallowedRegion = null,
            bool exceptStart = true,
            bool exceptEnd = true,
            bool acceptPartial = false)
            => FindPath(
                origin,
                destination.AsRegion(),
                maxDistance,
                allowedRegion,
                disallowedRegion,
                exceptStart,
                exceptEnd,
                acceptPartial);

        public static HexPath FindRoute(
            IEnumerable<HexPoint> wayPoints,
            int maxDistance,
            IReadOnlyRegion allowedRegion = null,
            IReadOnlyRegion disallowedRegion = null)
            => new HexPath(PathsInRoute(wayPoints, maxDistance, allowedRegion, disallowedRegion));

        public static IEnumerable<HexPath> PathsInRoute(
            IEnumerable<HexPoint> wayPoints,
            int maxDistance,
            IReadOnlyRegion allowedRegion = null,
            IReadOnlyRegion disallowedRegion = null)
        {
            var last = wayPoints.First();
            foreach (var p in wayPoints.Skip(1))
            {
                yield return FindPath(last, p, maxDistance, allowedRegion, disallowedRegion);
                last = p;
            }
        }

        static int GetHeuristic(HexPoint source, IReadOnlyRegion destination)
            => destination.Min(point => (source - point).MinManhattanDistance);

        static HexPath UnWrapPathFind(HexInfo endInfo, HexPoint origin)
        {

            List<(Direction, int)> stretches = new List<(Direction, int)>(ReverseStretchesToRoot(endInfo));
            stretches.Reverse();

            return new HexPath(origin, stretches);
        }

        static IEnumerable<(Direction, int)> ReverseStretchesToRoot(HexInfo end)
        {
            while (end?.parent != null)
                yield return StretchTo(end, out end);
        }

        static (Direction, int) StretchTo(HexInfo end, out HexInfo start)
        {
            if (end.parent == null)
            {
                start = null;
                return (default, 0);
            }

            var stretchToParent = StretchTo(end.parent, out start);
            var dirToMe = (end.point - end.parent.point).ToVec().ClosestDirection();
            if (dirToMe == stretchToParent.Item1)
            {
                return (dirToMe, stretchToParent.Item2 + 1);
            }
            else
            {
                start = end.parent;
                return (dirToMe, 1);
            }
        }
    }
}
