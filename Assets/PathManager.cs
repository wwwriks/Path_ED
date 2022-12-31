using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class PathManager : MonoBehaviour
{
    [Header("Searching algorithm")] public Searcher searcher;
    
    [Header("Goal ball")] public Ball goal;
    [Header("Start ball")] public Ball start;
    
    [Header("Ball")] public GameObject ball;
    public float minSpeed = 0f;
    public float maxSpeed = 2f;

    [Header("Bounds")] public Vector2 bounds;

    [Header("Amount of balls")] public int minAmount;
    public int maxAmount;

    public List<Ball> balls;
    
    private void Start()
    {
        GenerateBalls();
    }

    private void Path()
    {
        searcher.Search(balls);
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
    }
}
