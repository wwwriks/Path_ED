using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

[CreateAssetMenu(menuName = "Searching Algorithm/Dijkstras")]
public class Dijkstras : Searcher
{
    public override void Begin()
    {
        sampler = CustomSampler.Create("Dijkstras");
    }

    public override List<Ball> Search(Ball root, Ball goal, List<Ball> allBalls)
    {
        if (root == null || goal == null) return null;
        
        for (int i = 0; i < allBalls.Count; i++)
        {
            allBalls[i].Reset();
        }
        
        sampler.Begin();
        var runDijkstras = RunDijkstras(root, goal, allBalls);
        sampler.End();
        
        return runDijkstras;
    }

    private List<Ball> RunDijkstras(Ball root, Ball goal, List<Ball> allBalls)
    {
        if (root == null || goal == null) return null;
        
        HashSet<Ball> open = new HashSet<Ball>();
        HashSet<Ball> closed = new HashSet<Ball>();
        open.Clear();
        closed.Clear();

        Ball currentBall = root;
        currentBall.gCost = 0;
        currentBall.parent = null;

        open.Add(currentBall);

        while (open.Count > 0)
        {
            currentBall = open.OrderBy(ball => ball.gCost).First();

            if (currentBall == goal)
            {
                return ConstructPath(goal);
            }

            open.Remove(currentBall);
            closed.Add(currentBall);

            for (int i = 0; i < currentBall.neighbors.Count; i++)
            {
                var neighbor = currentBall.neighbors[i];
                if (closed.Contains(neighbor)) continue;
                if (!open.Contains(neighbor))
                {
                    open.Add(neighbor);
                    neighbor.parent = currentBall;
                    neighbor.gCost  = currentBall.gCost + Vector2.Distance(currentBall.transform.position, neighbor.transform.position);
                }
                else
                {
                    if (currentBall.gCost > neighbor.gCost + Vector2.Distance(currentBall.transform.position, neighbor.transform.position) && currentBall.parent != neighbor)
                    {
                        neighbor.parent = currentBall;
                        neighbor.gCost = currentBall.gCost + Vector2.Distance(currentBall.transform.position, neighbor.transform.position);
                    }
                }
            }


            /*
            foreach (Ball neighbor in currentTile.neighbors)
            {
                if (closed.Contains(neighbor)) continue;

                int allCost = currentTile.savedDistance + currentTile.cost;
                if (allCost < neighbor.savedDistance)
                {
                    neighbor.savedDistance = allCost;
                    open[neighbor.coordinate] = allCost;
                    neighbor.prevTile = currentTile;
                }
            }
            */
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
}