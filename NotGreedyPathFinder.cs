using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Greedy.Architecture;
using Newtonsoft.Json;

namespace Greedy;

public class NotGreedyPathFinder : IPathFinder
{
	
	public List<Point> FindPathToCompleteGoal(State state)
	{


		//if (state.Chests.Count <=20)
		//{
			List<Point[]> all = new List<Point[]>();
			Point[] start = new Point[state.Chests.Count];
			MakePermutation(state,start, state.Chests.ToList(), all, 0);
			var c = all.OrderByDescending(x => x.Count()).First();
			return TryToGo(state, c.ToList()).points;
		//}

  //      var numbers = Enumerable.Range(1, state.Chests.Count ).ToArray();
  //      int left = -1;
  //      int right = numbers.Count();
  //      var lazyAndGreedy = new GreedyPathFinder();
  //      while (right - left != 1)
  //      {
  //          var middle = (left + right) / 2;
  //          var curr = new State(state.MazeName, state.InitialEnergy, state.Position, state.CellCost, numbers[middle], state.Chests);
  //          if (lazyAndGreedy.FindPathToCompleteGoal(curr).Count == 0)
  //          {
  //              right = middle;
  //          }
  //          else left = middle;
  //      }
		//state.Scores = numbers[left];
  //      return lazyAndGreedy.FindPathToCompleteGoal(new State(state.MazeName, state.InitialEnergy, state.Position, state.CellCost, numbers[left], state.Chests));

    }

	public static WayAndChests TryToGo(State state, List<Point> way)
	{
		List<Point> points = new List<Point>();
		int chests = 0;
		var curr = state.Position;
		int energy = state.InitialEnergy;
		for(int i=0;i<way.Count;i++)
		{
			var t = new DijkstraPathFinder().GetPathsByDijkstra(state, curr, new List<Point>(1) { way[i] });

            if (t.Count()==0 || energy <t.First().Cost) return new WayAndChests(points,chests);
			else
			{
				var j=t.First().Path.Skip(1);
				chests++;
				foreach (var e in j) points.Add(e);
				curr= way[i];
				energy-=t.First().Cost;
			}
		}

		return new WayAndChests(points,chests);
	}

	public static void MakePermutation(State state,Point[] variant,List<Point>variants, List<Point[]> allways,int position=0)
	{
		if(position==variant.Length)
		{
			var curr=new Point[variant.Length];
			variant.CopyTo(curr,0);
			if(allways.Count>0)
			allways[0] = curr;
			else
				allways.Add(curr);
			return;
		}

		for(int i=0;i<variant.Length;i++) 
		{
			var c = Array.IndexOf(variant, variants[i],0,position);
			if (c != -1) continue;
			variant[position] = variants[i];
			var v = TryToGo(state, variant.Take(position + 1).ToList());
			if (v.chestscount==position+1) 
				MakePermutation(state, variant, variants, allways, position + 1);
			else
			{
                
				var curr = new Point[position+1];
                variant.Take(position+1).ToArray().CopyTo(curr, 0);
				if (allways.Count == 0)
				{
					allways.Add(curr);
				}
				else
				{
					if (curr.Count() > allways[0].Count()) allways[0]=curr;
				}
			}
		}
		return;
	}
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
