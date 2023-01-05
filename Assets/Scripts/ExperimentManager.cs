using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperimentManager : MonoBehaviour
{
    [SerializeField] private PathManager _pathManager;
    [SerializeField] private float experimentDuration = 60f;
    [SerializeField] private int amount = 100;
    [SerializeField] private Searcher[] sorters;
    public float waitTime = 0f;
    [SerializeField] private float timeScale = 1f;
    public List<int> ballAmounts = new List<int>();
    private int amountOfTimes = 0;
    [SerializeField] private int rerunsForAverage = 5;
    private int rerunIndex = 0;
    private int ballIndex = 0;

    public event OntoNextInstances OnNextInstances;

    public delegate void OntoNextInstances(int a, int b);

    public event OntoNextStep OnNextStep;

    public delegate void OntoNextStep(int a, int b);

    public event ExperimentFinished OnExperimentFinished;

    public delegate void ExperimentFinished();

    private void Awake()
    {
        Time.timeScale = timeScale;

        amountOfTimes = (int) (experimentDuration / waitTime);

        for (int i = 1; i < amountOfTimes + 2; i++)
        {
            ballAmounts.Add(amount * i);
        }
    }

    private void Start()
    {
        StartCoroutine(RunExperiment());
    }

    IEnumerator RunExperiment()
    {
        for (int i = 0; i < 3; i++)
        {
            if (i > 0)
            {
                _pathManager.ChangeSearcher(sorters[i]);
                ballIndex = 0;
            }

            for (int j = 0; j < (amountOfTimes + 1) * rerunsForAverage; j++)
            {
                NextInstances();

                if (rerunIndex >= rerunsForAverage)
                {
                    ballIndex++;
                    rerunIndex = 0;
                    NextStep(ballIndex);
                }

                rerunIndex++;
                yield return new WaitForSeconds(waitTime);
            }
        }

        EndExperiment();
    }

    private void NextInstances()
    {
        OnNextInstances?.Invoke(rerunIndex, rerunsForAverage);
    }

    private void NextStep(int index)
    {
        _pathManager.Clear();
        _pathManager.AddBalls(ballAmounts[index - 1]);
        OnNextStep?.Invoke(rerunIndex, rerunsForAverage);
    }

    private void StartExperiment()
    {
        _pathManager.AddBalls(amount);
    }

    private void EndExperiment()
    {
        ballIndex = 0;
        _pathManager.Clear();
        OnExperimentFinished?.Invoke();
    }
}