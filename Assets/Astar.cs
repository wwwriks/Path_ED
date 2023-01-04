//https://github.com/davecusatis/A-Star-Sharp/blob/master/Astar.cs

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Searching Algorithm/Astar")]
public class Astar : Searcher
{
    public override void Begin()
    {
        throw new System.NotImplementedException();
    }

    public override List<Ball> Search(Ball root, Ball goal)
    {
        return AstarPath(root, goal);
    }

    private static List<Ball> AstarPath(Ball start, Ball goal)
    {
        Stack<Ball> path = new Stack<Ball>();
        Queue<Ball> openSet = new Queue<Ball>();
        List<Ball> visited = new List<Ball>();
        List<Ball> adjancencies;
        Ball current = start;
        
        openSet.Enqueue(start);
        openSet.OrderBy(x => x.distanceToTarget).First();
        
        
        while (openSet.Count != 0 && !visited.Exists(x => goal))
        {
            current = openSet.Dequeue();
            visited.Add(current);
            adjancencies = current.neighbors;

            foreach (Ball b in adjancencies)
            {
                //If we havent already visited this ball...
                if (!visited.Contains(b))
                {
                    //Check if this ball is the goal
                    bool isFound = b == goal;
                    
                    if (!isFound)
                    {
                        b.parent = current;
                        b.distanceToTarget = Vector2.Distance(b.transform.position, goal.transform.position); 
                        b.cost = b.weight + b.parent.cost;
                        openSet.Enqueue(b);
                        openSet.OrderBy(x => x.F).First();
                    }
                }
            }
        }
        
        if (!visited.Exists(x => x == goal))
        {
            return null;
        }
            
        Ball temp = visited[visited.IndexOf(current)];
        if (temp == null) return null;
        do
        {
            path.Push(temp);
            temp = temp.parent;
        } while (temp != start && temp != null);
        
        return path.ToList();
    }
}
