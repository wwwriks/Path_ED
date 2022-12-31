using System;
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
    public bool IsExplored { get; private set; }

    //Set in editor
    [SerializeField] private int substeps = 1;
    [SerializeField] private float radius = 0.5f;
    [SerializeField] private float collisionThreshold = 0.1f;
    [SerializeField] private int neighborAmount = 3;
    private SpriteRenderer sRend = null;
    private float timeSinceLastCollision = 0f;

    //Calculated
    public float DstFromTarget { set; get; }
    private float realRadius;

    private void OnEnable()
    {
        transform.localScale = Vector3.one * radius * 0.5f;
        timeSinceLastCollision = collisionThreshold;
        sRend = GetComponentInChildren<SpriteRenderer>();
        realRadius = radius * 0.5f;
    }

    private void Start()
    {
        direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        speed = 5;
    }

    public void SetExplored()
    {
        IsExplored = true;
    }

    public void ResetExplored()
    {
        IsExplored = false;
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
    
    public void FindNeighbors()
    {
        var allBalls = pm.balls;
        
        //FIND ALL NEIGHBOURS THEN CUT OFF AT AROUND 3
        for (int i = 0; i < allBalls.Count; i++)
        {
            
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
