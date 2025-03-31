using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("AI Parameters")]
    public Transform target; 
    public float moveSpeed = 3.0f;
    public float stoppingDistance = 3.0f;
    public float chaseRange = 5.0f;
    public float attackRange = 3f;
    public float attackCooldown = 1.5f;

    [Header("Patrol Parameters")]
    public Transform[] patrolPoints;
    private int currentPatrolIndex = 0;
    public float patrolSpeed = 2.0f;
    public float waypointThreshold = 0.3f;

    [Header("Shooting Parameters")]
    public GameObject bulletPrefab; 
    public Transform firePoint; 
    public float bulletSpeed = 10f;

    private bool isFacingRight = true;
    private float lastAttackTime;

    private enum State { Patrolling, Chasing, Attacking }
    private State currentState;

    private void Start()
    {
        
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            target = player.transform;
        }

        currentState = State.Patrolling;
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Patrolling:
                Patrol();
                break;
            case State.Chasing:
                Chase();
                break;
            case State.Attacking:
                Attack();
                break;
        }

        CheckState();
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Transform targetPoint = patrolPoints[currentPatrolIndex];

        
        transform.position = new Vector2(
            Mathf.MoveTowards(transform.position.x, targetPoint.position.x, patrolSpeed * Time.deltaTime),
            transform.position.y
        );

        if (Mathf.Abs(transform.position.x - targetPoint.position.x) < waypointThreshold)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }

        FlipCheck(targetPoint.position.x);
    }

    private void Chase()
    {
        if (target == null) return;

        float distanceToTarget = Vector2.Distance(transform.position, target.position);

        
        if (distanceToTarget > stoppingDistance)
        {
            
            transform.position = new Vector2(
                Mathf.MoveTowards(transform.position.x, target.position.x, moveSpeed * Time.deltaTime),
                transform.position.y
            );

            FlipCheck(target.position.x);
        }
    }

    private void Attack()
    {
        
        float distanceToTarget = Vector2.Distance(transform.position, target.position);
        if (distanceToTarget > attackRange)
        {
            currentState = State.Chasing;
            return;
        }

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            Debug.Log("Enemy attacks!");
            FireBullet();
            lastAttackTime = Time.time;
        }
    }

    private void CheckState()
    {
        if (target != null)
        {
            float distanceToTarget = Vector2.Distance(transform.position, target.position);

            if (distanceToTarget <= attackRange)
            {
                currentState = State.Attacking;
            }
            else if (distanceToTarget <= chaseRange)
            {
                currentState = State.Chasing;
            }
            else
            {
                currentState = State.Patrolling;
            }
        }
    }

    private void FlipCheck(float targetX)
    {
        if ((targetX < transform.position.x && isFacingRight) || (targetX > transform.position.x && !isFacingRight))
        {
            Flip();
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    private void FireBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            float direction = isFacingRight ? 1f : -1f;
            rb.linearVelocity = new Vector2(direction * bulletSpeed, 0);
        }
    }
}
