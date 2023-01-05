using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public abstract class Searcher : ScriptableObject
{
    [SerializeField] internal string searcherName;
    internal CustomSampler sampler;
    public abstract void Begin();
    public abstract List<Ball> Search(Ball root, Ball goal, List<Ball> allBalls);
    public SearchOver OnSearched;
    public delegate void SearchOver();
}