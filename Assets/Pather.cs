﻿using System.Collections.Generic;
using UnityEngine;

public abstract class Searcher : ScriptableObject
{
    [SerializeField] internal string searcherName;
    public abstract void Begin();
    public abstract List<Ball> Search(List<Ball> balls);
    public SearchOver OnSearched;
    public delegate void SearchOver();
}