//https://gist.github.com/hermesespinola/15cf66af8edf059df9f38c6c879db0cb

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Searching Algorithm/Depth First")]
public class DepthFirst : Searcher
{
    public override void Begin()
    {
        throw new System.NotImplementedException();
    }

    public override List<Ball> Search(Ball start, Ball goal)
    {
        return DFS(start, goal);
    }

    private static List<Ball> DFS(Ball start, Ball goal)
    {
        Stack<Ball> work = new Stack<Ball>();
        List<Ball> visited = new List<Ball>();
        work.Push(start);
        visited.Add(start);
        start.history = new List<Ball>();
        while (work.Count > 0)
        {
            Ball current = work.Pop();
            if (current == goal)
            {
                List<Ball> result = current.history;
                result.Add(current);
                return result;
            }

            for (int i = 0; i < current.neighbors.Count; i++)
            {
                Ball currentChild = current.neighbors[i];
                if (!visited.Contains(currentChild))
                {
                    work.Push(currentChild);
                    visited.Add(currentChild);
                    currentChild.history = new List<Ball>(current.history);
                    currentChild.history.Add(current);
                }
            }
        }
        
        return null;
    }
}
