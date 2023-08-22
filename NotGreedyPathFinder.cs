using System;
using System.Collections.Generic;
using System.Linq;
using Greedy.Architecture;

namespace Greedy;

public class NotGreedyPathFinder : IPathFinder
{
    static bool findall = false;
    public List<Point> FindPathToCompleteGoal(State state)
    {
        List<Point[]> all = new List<Point[]>();
        Point[] start = new Point[state.Chests.Count];
        MakePermutation(state, (start, 0), state.Chests.ToList(), all, 0);
        Point[] c;
        if(findall) 
            c = all.First();
        else c=all.OrderByDescending(v => v.Length).First();
        var t = TryToGo(state, c);
        findall = false;
        return t.Points;
    }

    public static WayAndChests TryToGo(State state, IEnumerable<Point> way)
    {
        List<Point> points = new List<Point>();
        int chests = 0;
        var curr = state.Position;
        int energy = state.InitialEnergy;
        var t = new DijkstraPathFinder();
        foreach (var w in way)
        {
            var l = t.GetPathsByDijkstra(state, curr, new List<Point>(1) { w });
            if (l.Count() == 0 || energy < l.First().Cost) return new WayAndChests(points, chests);       
            var j = l.First().Path.Skip(1);
            chests++;
            foreach (var e in j) 
                points.Add(e);
            curr = w;
            energy -= l.First().Cost;  
        }

        return new WayAndChests(points, chests);
    }

    public static void MakePermutation(State state, (Point[], int) variant, List<Point> variants, List<Point[]> allways, int position)
    {
        if (allways.Count != 0 && findall) return;
        if (position == variants.Count)
        {
            findall = true;
            var curr = new Point[variant.Item1.Length];
            variant.Item1.CopyTo(curr, 0);
            if (allways.Count == 0) allways.Add(curr);
            else allways[0] = curr;
            return;
        }

        for (int i = 0; i < variant.Item1.Length; i++)
        {
            var c = Array.IndexOf(variant.Item1, variants[i], 0, position);
            if (c != -1) continue;
            variant.Item1[position] = variants[i];
            if (variant.Item2 == state.InitialEnergy || state.InitialEnergy < variant.Item2 + state.CellCost[variant.Item1[position].X, variant.Item1[position].Y])
            {
                var curr = new Point[position];
                variant.Item1.Take(position).ToArray().CopyTo(curr, 0);
                allways.Add(curr);
            }
            else
            {
                int currcost;
                if (position == 0) currcost = new DijkstraPathFinder().GetPathsByDijkstra(state, state.Position, new List<Point>(1) { variant.Item1[position] }).First().Cost;
                else currcost = new DijkstraPathFinder().GetPathsByDijkstra(state, variant.Item1[position - 1], new List<Point>(1) { variant.Item1[position] }).First().Cost;
                if (variant.Item2 + currcost <= state.InitialEnergy) MakePermutation(state, (variant.Item1, currcost + variant.Item2), variants, allways, position + 1);
                else
                {
                    var curr = new Point[position];
                    variant.Item1.Take(position).ToArray().CopyTo(curr, 0);
                    allways.Add(curr);
                }
            }
        }
        return;
    }
}


public class WayAndChests
{
    public List<Point> Points;
    public int Chestscount;
    public WayAndChests(List<Point> points, int chestscount)
    {
        this.Points = points;
        this.Chestscount = chestscount;
    }
}
