using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Greedy.Architecture;

namespace Greedy;

public class NotGreedyPathFinder : IPathFinder
{
	public List<Point> FindPathToCompleteGoal(State state)
	{
	
		var numbers=Enumerable.Range(0,state.Chests.Count+1).ToArray();
		int left = -1;
		int right = numbers.Count();
		var lazyAndGreedy= new GreedyPathFinder();
		while (right-left!=1) 
		{ 
			var middle=(left+right)/2;
            var curr = new State(state.MazeName, state.InitialEnergy, state.Position, state.CellCost, numbers[middle],state.Chests);
			if (lazyAndGreedy.FindPathToCompleteGoal(curr).Count == 0)
			{
				right = middle;
			}
			else left = middle;
        }

		return lazyAndGreedy.FindPathToCompleteGoal(new State(state.MazeName, state.InitialEnergy, state.Position, state.CellCost, numbers[left], state.Chests));
	}
}
