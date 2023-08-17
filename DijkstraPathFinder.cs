using System;
using System.Collections.Generic;
using System.Linq;
using Greedy.Architecture;

namespace Greedy;

public class DijkstraPathFinder
{
    public IEnumerable<PathWithCost> GetPathsByDijkstra(State state, Point start, IEnumerable<Point> targets)
    {
        Dictionary<Point, DijkstraData> track = new Dictionary<Point, DijkstraData>();
        HashSet<Point> notvisited = new HashSet<Point>();
        track[start] = new DijkstraData { Mark = 0, Previous = null, Length = 0 };
        notvisited.Add(start);
        while (true)
        {
            Point? toOpen = null;
            double topPrice = double.PositiveInfinity;
            foreach (var node in notvisited)
            {
                if (track.ContainsKey(node) && track[node].Mark <= topPrice)
                {
                    toOpen = node;
                    topPrice = track[node].Mark;
                }
            }

            if (toOpen == null) yield break;

            if (state.Chests.Contains(toOpen.Value))
            {
                Point[] points = new Point[track[toOpen.Value].Length + 1];
                points[track[toOpen.Value].Length] = toOpen.Value;
                var result = track[toOpen.Value].Mark;
                var now = track[toOpen.Value].Previous;
                while (now != null)
                {
                    points[track[now.Value].Length] = now.Value;
                    now = track[now.Value].Previous;
                }
                yield return new PathWithCost(result, points);
            }

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if ((j == 0 && i == 0) || (j != 0 && i != 0)) continue;
                    if (!state.InsideMap(new Point(toOpen.Value.X + i, toOpen.Value.Y + j))) continue;
                    if (track[toOpen.Value].Previous != null && track[toOpen.Value].Previous.Value == new Point(toOpen.Value.X + i, toOpen.Value.Y + j)) continue;
                    if (state.IsWallAt(new Point(toOpen.Value.X + i, toOpen.Value.Y + j))) continue;
                    var currprice = (int)topPrice + state.CellCost[toOpen.Value.X + i, toOpen.Value.Y + j];
                    if (!notvisited.Contains(new Point(toOpen.Value.X + i, toOpen.Value.Y + j)) && !track.ContainsKey(new Point(toOpen.Value.X + i, toOpen.Value.Y + j))) notvisited.Add(new Point(toOpen.Value.X + i, toOpen.Value.Y + j));
                    if (!track.ContainsKey(new Point(toOpen.Value.X + i, toOpen.Value.Y + j)) || track[new Point(toOpen.Value.X + i, toOpen.Value.Y + j)].Mark > currprice) track[new Point(toOpen.Value.X + i, toOpen.Value.Y + j)] = new DijkstraData { Mark = currprice, Previous = toOpen.Value, Length = track[toOpen.Value].Length + 1 };
                }
            }

            notvisited.Remove(toOpen.Value);
        }
    }
}

public class DijkstraData
{
    public int Mark;
    public Point? Previous;
    public int Length;
}