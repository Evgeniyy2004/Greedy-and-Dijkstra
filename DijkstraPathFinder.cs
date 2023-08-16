using System;
using System.Collections.Generic;
using System.Linq;
using Greedy.Architecture;

namespace Greedy;

public class DijkstraPathFinder
{
	public IEnumerable<PathWithCost> GetPathsByDijkstra(State state, Point start,IEnumerable<Point> targets)
	{
        Dictionary<Point, DijkstraData> track = new Dictionary<Point, DijkstraData>();
        HashSet<Point> notvisited = new HashSet<Point>();
        track[start] = new DijkstraData { metka = 0, Previous = null };
        for (int i = 0; i < state.CellCost.GetLength(0); i++)
        {
            for (int j = 0; j < state.CellCost.GetLength(1); j++) notvisited.Add(new Point(i, j));
        }
        var goalcopy = targets.ToHashSet();
        
            while (goalcopy.Count > 0)
            {
                Point? toOpen = null;
                double topPrice = double.PositiveInfinity;
                foreach (var e in notvisited)
                {
                    if (track.ContainsKey(e) && track[e].metka < topPrice)
                    {
                        toOpen = e;
                        topPrice = track[e].metka;
                    }
                }

                if (toOpen == null) break;

                if (goalcopy.Contains(toOpen.Value))
                {
                    goalcopy.Remove(toOpen.Value);
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
                        if (!track.ContainsKey(new Point(toOpen.Value.X + i, toOpen.Value.Y + j)) || track[new Point(toOpen.Value.X + i, toOpen.Value.Y + j)].metka > currprice) track[new Point(toOpen.Value.X + i, toOpen.Value.Y + j)] = new DijkstraData { metka = currprice, Previous = toOpen.Value };
                    }
                }

                notvisited.Remove(toOpen.Value);
            }
        
        List<PathWithCost> path = new List<PathWithCost>();
        foreach (var e in targets)
        {

            if (track.ContainsKey(e))
            {
                List<Point> points = new List<Point>() { e };
                var result = track[e].metka;
                var now = track[e].Previous;
                while (now != null)
                {
                    points.Add(now.Value);
                    now = track[now.Value].Previous;
                }
                points.Reverse();
                path.Add(new PathWithCost(result, points.ToArray()));
            }
        }

        foreach (var e in path.OrderBy(x => x.Cost))
        {
            yield return e;
        }
    }

	
}

public class DijkstraData
{
	public int metka;
	public Point? Previous;
}