using System.Collections.Generic;
using UnityEngine;

public abstract class Searcher : ScriptableObject
{
    [SerializeField] internal string searcherName;
    public abstract void Begin();
    public abstract List<Ball> Search(Ball root, Ball goal);
    public SearchOver OnSearched;
    public delegate void SearchOver();
}