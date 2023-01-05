using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

[CreateAssetMenu(menuName = "Searching Algorithm/Breadth First")]
public class BreadthFirst : Searcher
{
    public override void Begin()
    {
        sampler = CustomSampler.Create("BFS");
    }

    public override List<Ball> Search(Ball root, Ball goal, List<Ball> allBalls)
    {
        if (root == null || goal == null) return null;
        
        //sampler.Begin();
        var bfs = BFS(root, goal);
        //sampler.End();
        
        return bfs;
    }

    private static List<Ball> BFS(Ball root, Ball goal)
    {
        if (root == null || goal == null) return null;
        
        var start = root;
        List<Ball> result = new List<Ball>();
        List<Ball> visited = new List<Ball>();
        Queue<Ball> work = new Queue<Ball>();
        start.history = new List<Ball>();
        visited.Add(start);
        work.Enqueue(start);

        while (work.Count > 0)
        {
            Ball current = work.Dequeue();
            if (current == goal)
            {
                result = current.history;
                result.Add(current);
                return result;
            }

            for (int i = 0; i < current.neighbors.Count; i++)
            {
                Ball neighbor = current.neighbors[i];
                if (!visited.Contains(neighbor))
                {
                    neighbor.history = new List<Ball>(current.history);
                    neighbor.history.Add(current);
                    visited.Add(neighbor);
                    work.Enqueue(neighbor);
                }
            }
        }

        return null;
    }
}
