using System.Collections.Generic;
using System.Linq;
using Greedy.Architecture;

namespace Greedy;

public class GreedyPathFinder : IPathFinder
{
	public List<Point> FindPathToCompleteGoal(State state)
	{
		var startOfThis = state.Chests;
		var maker = new DijkstraPathFinder();
		List<Point>result= new List<Point>();
		int count = 0;
		for (int j = 0; j < state.Goal; j++)
		{
			foreach(var e in maker.GetPathsByDijkstra(state,state.Position,startOfThis))
			{
				if (state.Energy < e.Cost) break;
				result=result.Concat(e.Path.Skip(1)).ToList();
				state.Position=e.Path.Last();
				startOfThis.Remove(state.Position);
				count++;
				break;
			}
		}
		if (count != state.Goal) return new List<Point>();
		return result;
	}
}