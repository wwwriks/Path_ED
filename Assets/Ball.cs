using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Ball : MonoBehaviour
{
    //Set elsewhere
    private float speed;
    private Vector2 direction;
    private Transform target;
    private Vector2 bounds = new Vector2(5, 5);
    private PathManager pm;
    public List<Ball> history;
    public bool specialBall = false;

    //Set in editor
    [SerializeField] private int substeps = 1;
    [SerializeField] private float radius = 0.5f;
    [SerializeField] private float collisionThreshold = 0.1f;
    [SerializeField] private float neighborRadius = 3f;
    [SerializeField] private SpriteRenderer sRend = null;
    [SerializeField] private SpriteRenderer sRendReach = null;
    [SerializeField] private CircleCollider2D circCol = null;
    private float timeSinceLastCollision = 0f;

    //Calculated
    private float realRadius;
    public List<Ball> neighbors = new List<Ball>();

    private void OnEnable()
    {
        sRend.transform.localScale = Vector3.one * radius * 0.5f;
        timeSinceLastCollision = collisionThreshold;
        realRadius = radius * 0.5f;
        sRendReach.transform.localScale = Vector3.one * neighborRadius * 0.5f + Vector3.one;
        circCol = GetComponent<CircleCollider2D>();
        circCol.radius = realRadius * 0.5f;
    }

    private void Start()
    {
        direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        speed = 5;
    }

    private void Update()
    {
        FindNeighbors();
        if (specialBall) return;
        if (neighbors.Count == 0)
        {
            SetColor(Color.gray);
        }
        else
        {
            SetColor(Color.white);
        }
        
        DrawNeighbors();
    }

    private void DrawNeighbors()
    {
        for (int i = 0; i < neighbors.Count; i++)
        {
            Debug.DrawLine(transform.position, neighbors[i].transform.position, Color.red);
        }
    }
    
    internal virtual void FixedUpdate()
    {
        timeSinceLastCollision += Time.deltaTime;

        Vector3 position = transform.position;
        
        for (int i = 0; i < substeps; i++)
        {
            if (transform.position.x + realRadius > bounds.x)
            {
                float delta =  bounds.x - (transform.position.x + realRadius);
                position = new Vector3(position.x + delta, position.y);
                direction *= Collided(new Vector2(-1, 1));
            }

            if (transform.position.x - realRadius < -bounds.x)
            {
                float delta =  -bounds.x - (transform.position.x - realRadius);
                position = new Vector3(position.x + delta, position.y);
                direction *= Collided(new Vector2(-1, 1));
            }

            if (transform.position.y + realRadius > bounds.y)
            {
                float delta =  bounds.y - (transform.position.y + realRadius);
                position = new Vector3(position.x, position.y + delta);
                direction *= Collided(new Vector2(1, -1));
            }

            if (transform.position.y - realRadius < -bounds.y)
            {
                float delta =  -bounds.y - (transform.position.y - realRadius);
                position = new Vector3(position.x, position.y + delta);
                direction *= Collided(new Vector2(1, -1));
            }
        }
        
        position += (Vector3) (direction * (speed * Time.deltaTime));
        transform.position = position;
    }

    private void FindNeighbors()
    {
        neighbors.Clear();
        List<Collider2D> col = Physics2D.OverlapCircleAll(transform.position, neighborRadius).ToList();
        for (int i = 0; i < col.Count; i++)
        {
            if (col[i] == circCol)
            {
                col.RemoveAt(i);
            }
        }
        
        col.OrderBy(x => Vector3.Distance(x.transform.position, transform.position));
        
        for (int i = 0; i < col.Count; i++)
        {
            neighbors.Add(col[i].GetComponent<Ball>());
        }
    }
    
    private Vector2 Collided(Vector2 dir)
    {
        if (timeSinceLastCollision >= collisionThreshold)
        {
            timeSinceLastCollision = 0f;

            return dir;
        }

        return new Vector2(1, 1);
    }
    
    public void InitBall(float _speed, Vector2 _direction, Vector2 _bounds, PathManager _pathManager)
    {
        speed = _speed;
        direction = _direction;
        bounds = _bounds;
        pm = _pathManager;
    }

    public void SetColor(Color _color)
    {
        sRend.color = _color;
    }
}
