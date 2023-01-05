using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Serialization;

public class PathRecorder : MonoBehaviour
{
    [SerializeField] private string filename;
    [SerializeField] private PathManager pm;
    [SerializeField] private ExperimentManager em;
    private Recorder recorder;
    [SerializeField] private TestData td = new TestData();
    private Searcher prevSorter;
    private int algoIndex;

    [SerializeField] private List<float> milliseconds = new List<float>();

    private bool done = false;
    
    private void Start()
    {
        td = new TestData("hello", new List<int>(), new List<float>(), new List<float>(), new List<float>());
        recorder = Recorder.Get("BFS");
        prevSorter = pm.searcher;
        pm.searcher.OnSearched += OnSearched;
        pm.OnSearcherChange += OnSearcherChange;
        em.OnNextStep += OnNextStep;
        em.OnExperimentFinished += OnFinish;

        foreach (var amount in em.ballAmounts) { td.instances.Add(amount); }
    }

    private void OnEnable()
    {
        milliseconds = new List<float>();
        em.OnNextInstances += OnNextInstances;
    }

    private void OnSearcherChange(Searcher newSearcher)
    {
        prevSorter.OnSearched -= OnSearched;
        newSearcher.OnSearched += OnSearched;
        prevSorter = newSearcher;
        algoIndex++;
    }

    private void Update()
    {
        if (done) return;

        switch (algoIndex)
        {
            case 0:
                milliseconds.Clear();
                recorder = Recorder.Get("BFS");
                break;
            case 1:
                milliseconds.Clear();
                recorder = Recorder.Get("Dijkstras");
                break;
            case 2:
                milliseconds.Clear();
                recorder = Recorder.Get("Astar");
                break;
        }
    }

    private void OnSearched()
    {
        return;
        if (done) return;

        switch (algoIndex)
        {
            case 0:
                recorder = Recorder.Get("BFS");
                break;
            case 1:
                recorder = Recorder.Get("Dijkstras");
                break;
            case 2:
                recorder = Recorder.Get("Astar");
                break;
        }
        
        return;
        if (pm.searcher is BreadthFirst)
        {
            Debug.Log("I AM BFS");
            recorder = Recorder.Get("BFS");
            algoIndex = 0;
        }
        
        if (pm.searcher is Dijkstras)
        {
            Debug.Log("I AM DIJKSTRAS");
            recorder = Recorder.Get("Dijkstras");
            algoIndex = 1;
        }
        
        if (pm.searcher is Astar)
        {
            Debug.Log("I AM ASTAR");
            recorder = Recorder.Get("Astar");
            algoIndex = 2;
        }
    }

    private void OnDisable()
    {
        pm.searcher.OnSearched -= OnSearched;
        em.OnNextStep -= OnNextStep;
        pm.OnSearcherChange -= OnSearcherChange;
        em.OnExperimentFinished -= OnFinish;
        em.OnNextInstances -= OnNextInstances;
    }

    public void TakeSnapshot(int index, int maxIndex)
    {
        if (!recorder.isValid) return;
        milliseconds.Add(recorder.elapsedNanoseconds * 0.000001f);
        if (index < maxIndex) return;
        
        switch (algoIndex)
        {
            case 0:
                td.ms_BFS.Add(milliseconds.Average());
                break;
            case 1:
                td.ms_Dijkstras.Add(milliseconds.Average());
                break;
            case 2:
                td.ms_Astar.Add(milliseconds.Average());
                break;
        }
        
        milliseconds.Clear();
    }

    private void OnNextInstances(int index, int maxIndex)
    {
        TakeSnapshot(index, maxIndex);
    }
    
    private void OnNextStep(int index, int maxIndex)
    {
        //TODO this might fuck it up
        TakeSnapshot(index, maxIndex);
        
        return;
        if (recorder == null) return;
        if (recorder.isValid)
        {
            
            
            /*
            //milliseconds.Add(recorder.elapsedNanoseconds * 0.000001f);
            //if (milliseconds.Count > entriesForAvg)
            //{
            switch (algoIndex)
            {
                case 0:
                    td.ms_CS.Add(recorder.elapsedNanoseconds * 0.000001f);
                    //td.ms_CS.Add(milliseconds.Average());
                    break;
                case 1:
                    td.ms_Insert.Add(recorder.elapsedNanoseconds * 0.000001f);
                    break;
                case 2:
                    td.ms_Merge.Add(recorder.elapsedNanoseconds * 0.000001f);
                    break;
            }
            //milliseconds.Clear();
            //}
            */
        }
    }

    private void OnFinish()
    {
        pm.searcher.OnSearched -= OnSearched;
        WriteResultsToFile();
    }

    private void WriteResultsToFile()
    {
        using (StreamWriter streamWriter = new StreamWriter(filename))
        {
            streamWriter.Write("instances,BFS,Dijkstras,Astar");
            streamWriter.WriteLine(String.Empty);
            
            //DO NOT USE td.instancs.count - 1 IT IS TEMPORARY
            
            for (int i = 0; i < td.instances.Count - 1; i++)
            {
                streamWriter.Write($"{td.instances[i].ToString(CultureInfo.InvariantCulture)},{td.ms_BFS[i].ToString(CultureInfo.InvariantCulture)},{td.ms_Dijkstras[i].ToString(CultureInfo.InvariantCulture)},{td.ms_Astar[i].ToString(CultureInfo.InvariantCulture)}");
                streamWriter.WriteLine(string.Empty);
            }
        }
    }

    [System.Serializable]
    public struct TestData
    {
        public string tName;
        public List<int> instances;
        public List<float> ms_BFS;
        public List<float> ms_Dijkstras;
        public List<float> ms_Astar;

        public TestData(string n, List<int> inst, List<float> _ms_BFS, List<float> _ms_Dijkstras, List<float> _ms_Astar)
        {
            tName = n;
            instances = inst;
            ms_BFS = _ms_BFS;
            ms_Dijkstras = _ms_Dijkstras;
            ms_Astar = _ms_Astar;
        }
    }
}