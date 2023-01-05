using System;
using System.Collections;
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
    [FormerlySerializedAs("sm")] [SerializeField] private PathManager pm;
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
        recorder = Recorder.Get("CS_Default");
        prevSorter = pm.searcher;
        pm.searcher.OnSearched += OnSorted;
        pm.OnSearcherChange += OnSorterChanged;
        em.OnNextStep += OnNextStep;
        em.OnExperimentFinished += OnFinish;

        foreach (var amount in em.ballAmounts) { td.instances.Add(amount); }
    }

    private void OnEnable()
    {
        milliseconds = new List<float>();
        em.OnNextInstances += OnNextInstances;
    }

    private void OnSorterChanged(Searcher newSorter)
    {
        prevSorter.OnSearched -= OnSorted;
        newSorter.OnSearched += OnSorted;
        prevSorter = newSorter;
        algoIndex++;
    }
    
    private void OnSorted()
    {
        if (done) return;
        
        if (pm.searcher is BreadthFirst)
        {
            recorder = Recorder.Get("CS_Default");
            algoIndex = 0;
        }
        
        if (pm.searcher is Dijkstras)
        {
            recorder = Recorder.Get("Insertion");
            algoIndex = 1;
        }
        
        if (pm.searcher is Astar)
        {
            recorder = Recorder.Get("Merge");
            algoIndex = 2;
        }
    }

    private void OnDisable()
    {
        pm.searcher.OnSearched -= OnSorted;
        em.OnNextStep -= OnNextStep;
        pm.OnSearcherChange -= OnSorterChanged;
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
                td.ms_CS.Add(milliseconds.Average());
                break;
            case 1:
                td.ms_Insert.Add(milliseconds.Average());
                break;
            case 2:
                td.ms_Merge.Add(milliseconds.Average());
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
        pm.searcher.OnSearched -= OnSorted;
        WriteResultsToFile();
    }

    private void WriteResultsToFile()
    {
        using (StreamWriter streamWriter = new StreamWriter(filename))
        {
            streamWriter.Write("instances,CS_Default,Insert,Merge");
            streamWriter.WriteLine(String.Empty);
            
            //DO NOT USE td.instancs.count - 1 IT IS TEMPORARY
            
            for (int i = 0; i < td.instances.Count - 1; i++)
            {
                streamWriter.Write($"{td.instances[i].ToString(CultureInfo.InvariantCulture)},{td.ms_CS[i].ToString(CultureInfo.InvariantCulture)},{td.ms_Insert[i].ToString(CultureInfo.InvariantCulture)},{td.ms_Merge[i].ToString(CultureInfo.InvariantCulture)}");
                streamWriter.WriteLine(string.Empty);
            }
        }
    }

    [System.Serializable]
    public struct TestData
    {
        public string tName;
        public List<int> instances;
        public List<float> ms_CS;
        public List<float> ms_Insert;
        public List<float> ms_Merge;

        public TestData(string n, List<int> inst, List<float> _ms_CS, List<float> _ms_Insert, List<float> _ms_Merge)
        {
            tName = n;
            instances = inst;
            ms_CS = _ms_CS;
            ms_Insert = _ms_Insert;
            ms_Merge = _ms_Merge;
        }
    }
}