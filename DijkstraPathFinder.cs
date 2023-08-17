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
        track[start] = new DijkstraData { metka = 0, Previous = null, length=0};
        
        //var goalcopy = targets.ToHashSet();
        int find = 0;
        List<PathWithCost> all= new List<PathWithCost>();
        foreach (var e in targets)
        {
            if(find==0)
            {
                for (int i = 0; i < state.CellCost.GetLength(0); i++)
                {
                    for (int j = 0; j < state.CellCost.GetLength(1); j++) notvisited.Add(new Point(i, j));
                }
                find++;
            }
            if(!notvisited.Contains(e))
            {
                Point[] points = new Point[track[e].length+1];
                points[track[e].length] = e;
                var result = track[e].metka;
                var now = track[e].Previous;
                while (now != null)
                {
                    points[track[now.Value].length]=now.Value;
                    now = track[now.Value].Previous;
                }
                /*if(result<=state.InitialEnergy)*/ all.Add(new PathWithCost(result, points));
                continue;
            }

            else while (true)
            {
                Point? toOpen = null;
                double topPrice = double.PositiveInfinity;
                foreach (var node in notvisited)
                {
                    if (track.ContainsKey(node) && track[node].metka < topPrice)
                    {
                        toOpen = node;
                        topPrice = track[node].metka;
                    }
                }

                if (toOpen == null) yield break;

                if (toOpen==e)
                {
                        Point[] points = new Point[track[e].length+1];
                        points[track[e].length] = e;
                        var result = track[e].metka;
                        var now = track[e].Previous;
                        while (now != null)
                        {
                            points[track[now.Value].length] = now.Value;
                            now = track[now.Value].Previous;
                        }
                        /*if (result <= state.InitialEnergy)*/ all.Add(new PathWithCost(result, points));
                        break;
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
                        if (!track.ContainsKey(new Point(toOpen.Value.X + i, toOpen.Value.Y + j)) || track[new Point(toOpen.Value.X + i, toOpen.Value.Y + j)].metka > currprice) track[new Point(toOpen.Value.X + i, toOpen.Value.Y + j)] = new DijkstraData { metka = currprice, Previous = toOpen.Value, length = track[toOpen.Value].length+1 };
                    }
                }

                notvisited.Remove(toOpen.Value);
            }
        }
        //Console.WriteLine(find);
        //Console.WriteLine(targets.Count());
        var c = all.OrderBy(x => x.Cost);
        foreach(var e in c) yield return e;
    }

	
}

public class DijkstraData
{
	public int metka;
	public Point? Previous;
    public int length;
}