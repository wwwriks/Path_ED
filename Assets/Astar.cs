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
        /*
        nodes.Clear();
        for (int i = 0; i < balls.Count; i++)
        {
            balls[i].GetNearbyColliders();
            nodes.Add(balls[i]);
            nodes[i].hCost = 0;
            nodes[i].gCost = 0;
            nodes[i].parent = null;
        }
        */
    }

    public override List<Ball> Search(Ball root, Ball goal, List<Ball> allBalls)
    {
        for (int i = 0; i < allBalls.Count; i++)
        {
            allBalls[i].Reset();
        }
        
        return AstarPath(root, goal);
    }

    private static List<Ball> AstarPath(Ball start, Ball goal)
    {
        HashSet<Ball> open = new HashSet<Ball>();
        HashSet<Ball> closed = new HashSet<Ball>();
        Ball current = start;
        
        current.hCost = 0;
        current.gCost = 0;
        current.parent = null;
        
        open.Add(start);

        while (open.Count > 0)
        {
            current = open.OrderBy(ball => ball.fCost).First();
            
            if (current == goal)
            {
                return ConstructPath(goal);
            }

            open.Remove(current);
            closed.Add(current);
            
            for (int i = 0; i < current.neighbors.Count; i++)
            {
                var neighbor = current.neighbors[i];
                if (closed.Contains(neighbor)) continue;
                if (!open.Contains(neighbor))
                {
                    open.Add(neighbor);
                    neighbor.parent = current;
                    neighbor.gCost  = current.gCost + Vector2.Distance(current.transform.position, neighbor.transform.position);
                    neighbor.hCost  = Vector2.Distance(goal.transform.position, neighbor.transform.position);
                }
                else
                {
                    if (current.gCost > neighbor.gCost + Vector2.Distance(current.transform.position, neighbor.transform.position) && current.parent != neighbor)
                    {
                        neighbor.parent = current;
                        neighbor.gCost = current.gCost + Vector2.Distance(current.transform.position, neighbor.transform.position);
                    }
                }
            }
        }
        
        return null;
    }

    private static List<Ball> ConstructPath(Ball goal)
    {
        List<Ball> temp = new List<Ball>();
        Ball parent = goal;
        temp.Add(parent);

        while (parent != null)
        {
            parent = parent.parent;
            temp.Add(parent);
        }
        
        return temp;
    }
    
    /*
    private static List<Ball> AstarPathOld(Ball start, Ball goal)
    {
        Stack<Ball> path = new Stack<Ball>();
        Queue<Ball> openSet = new Queue<Ball>();
        List<Ball> visited = new List<Ball>();
        List<Ball> adjancencies;
        Ball current = start;
        
        openSet.Enqueue(start);
        openSet.OrderBy(x => x.weight + x.parent.cost + x.distanceToTarget);

        while (openSet.Count != 0 && !visited.Exists(x => goal))
        {
            current = openSet.Dequeue()
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
                        b.cost = b.weight + b.parent.cost + b.distanceToTarget;
                        openSet.Enqueue(b);
                        openSet.OrderBy(x => x.weight + x.parent.cost + x.distanceToTarget);
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
    */
}
