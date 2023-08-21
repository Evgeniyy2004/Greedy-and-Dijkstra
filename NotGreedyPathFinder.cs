using System;
using System.Collections.Generic;
using System.Linq;
using Greedy.Architecture;

namespace Greedy;

public class NotGreedyPathFinder : IPathFinder
{
	
	public List<Point> FindPathToCompleteGoal(State state)
	{	
			List<Point[]> all = new List<Point[]>();
			Point[] start = new Point[state.Chests.Count];
			MakePermutation(state, (start, 0), state.Chests.ToList(), all, 0);
			var c = all.OrderByDescending(v => v.Length ).First();
			var t = TryToGo(state, c);
			return t.points;
				
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
			
			var l=t.GetPathsByDijkstra(state, curr, new List<Point>(1) { w });
            if (l.Count()==0 || energy <l.First().Cost ) return new WayAndChests(points,chests);
			else
			{
				var j=l.First().Path.Skip(1);
				chests++;
				foreach (var e in j) points.Add(e);
				curr= w;
				energy-=l.First().Cost;
			}
		}

		return new WayAndChests(points,chests);
	}

	public static void MakePermutation(State state,(Point[],int) variant,List<Point>variants, List<Point[]> allways,int position=0)
	{
		if (allways.Count != 0 && allways[0].Length == variants.Count) return;
		if(position==variants.Count)
		{
			var curr=new Point[variant.Item1.Length];
			variant.Item1.CopyTo(curr,0);
			if (allways.Count == 0) allways.Add(curr);
			else allways[0] = curr ;
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

    //public List<Point> GreedyStrategy(State state)
    //{
    //    var startOfThis = state.Chests.ToHashSet();
    //    var maker = new DijkstraPathFinder();
    //    List<Point> result = new List<Point>();
    //    int count = 0;
    //    for (int j = 0; j < state.Chests.Count; j++)
    //    {
    //        foreach (var e in maker.GetPathsByDijkstra(state, state.Position, startOfThis))
    //        {
    //            if (state.Energy < e.Cost) break;
    //            result = result.Concat(e.Path.Skip(1)).ToList();
    //            state.Position = e.Path.Last();
    //            state.Energy -= e.Cost;
    //            startOfThis.Remove(state.Position);
    //            count++;
    //            break;
    //        }
    //    }
    //    return result;
    //}
}


public class WayAndChests
{
	public List<Point> points;
	public int chestscount;
	public WayAndChests(List<Point> points, int chestscount)
    {
        this.points = points;
        this.chestscount = chestscount;
    }
}
