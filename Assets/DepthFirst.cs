using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthFirst : Searcher
{
    public override void Begin()
    {
        throw new System.NotImplementedException();
    }

    public override List<Ball> Search(List<Ball> balls)
    {
        DFS();

        return balls;
    }

    private static void DFS()
    {
        
    }
}
