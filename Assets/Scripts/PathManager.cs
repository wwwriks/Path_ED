using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class PathManager : MonoBehaviour
{
    public bool Simulate = false;
    [Header("Searching algorithm")] public Searcher searcher;
    
    private Ball goal;
    private Ball start;

    [Header("Ball")] public GameObject ball;
    public float minSpeed = 0f;
    public float maxSpeed = 2f;

    [Header("Bounds")] public Vector2 bounds;

    [Header("Amount of balls")] public int minAmount;
    public int maxAmount;

    public List<Ball> balls;
    
    public event SearcherChange OnSearcherChange;
    public delegate void SearcherChange(Searcher searcher);
    
    public void StartSimulation()
    {
        GenerateBalls();
        searcher.Begin();
    }

    private void Start()
    {
        searcher.Begin();
        
        if (!Simulate) return;
        GenerateBalls();
    }

    private void Update()
    {
        Path();
    }

    private void Path()
    {
        var path = searcher.Search(start, goal, balls);
        DrawPath(path);
    }

    private void DrawPath(List<Ball> pos)
    {
        if (pos == null) return;
        for (int i = 0; i < pos.Count; i++)
        {
            Vector3 holder = Vector3.zero;
            if (i == pos.Count - 1) return;
            if (pos[i + 1] == null)
            {
                holder = pos[i].transform.position;
            }
            else
            {
                holder = pos[i + 1].transform.position;
            }
            Debug.DrawLine(pos[i].transform.position + Vector3.up * 0.125f, holder + Vector3.up * 0.125f, Color.green);
        }
    }
    
    public void GenerateBalls()
    {
        int length = Random.Range(minAmount, maxAmount);
        balls = new List<Ball>();
        for (int i = 0; i < length; i++)
        {
            var theball = Instantiate(ball, Vector3.zero, Quaternion.identity);
            theball.transform.parent = transform;

            var ballComponent = theball.GetComponent<Ball>();
            
            ballComponent.InitBall(
                Random.Range(minSpeed, maxSpeed),
                new Vector2(
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f)).normalized,
                bounds, this);

            balls.Add(ballComponent);
        }

        goal = balls[Random.Range(0, balls.Count)];
        goal.SetColor(Color.yellow);
        goal.specialBall = true;
        /*
        if (balls.Count < 2)
        {
            Debug.LogError("There always needs to be two balls present");
            return;
        }
        */
        List<Ball> ballListWithoutGoal = balls;
        ballListWithoutGoal.Remove(goal);
        start = ballListWithoutGoal[Random.Range(0, ballListWithoutGoal.Count)];
        start.SetColor(Color.green);
        start.specialBall = true;
    }

    public void AddBalls(int amount)
    {
        int length = amount;
        balls = new List<Ball>();
        for (int i = 0; i < length; i++)
        {
            var theball = Instantiate(ball, Vector3.zero, Quaternion.identity);
            theball.transform.parent = transform;

            var ballComponent = theball.GetComponent<Ball>();
            
            ballComponent.InitBall(
                Random.Range(minSpeed, maxSpeed),
                new Vector2(
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f)).normalized,
                bounds, this);
            theball.transform.position = new Vector2(Random.Range(-bounds.x, bounds.x), Random.Range(-bounds.y, bounds.y));

            balls.Add(ballComponent);
        }

        goal = balls[Random.Range(0, balls.Count)];
        goal.SetColor(Color.yellow);
        goal.specialBall = true;
        List<Ball> ballListWithoutGoal = balls;
        ballListWithoutGoal.Remove(goal);
        start = ballListWithoutGoal[Random.Range(0, ballListWithoutGoal.Count)];
        start.SetColor(Color.green);
        start.specialBall = true;
    }
    
    public void ChangeSearcher(Searcher _searcher)
    {
        searcher = _searcher;
        searcher.Begin();
        OnSearcherChange?.Invoke(_searcher);
    }
    
    public void Clear()
    {
        for (int i = transform.childCount - 1; i > 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        balls.Clear();
    }
}
