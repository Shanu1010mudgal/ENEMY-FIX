//using UnityEngine;

//public class EnemyPatrol : MonoBehaviour
//{
//    public Transform pointA; // First patrol point
//    public Transform pointB; // Second patrol point
//    public Transform firePoint; // Fire point reference
//    public float speed = 2f; // Patrol speed

//    private Transform target; // Current target point
//    private float yPosition; // Fixed y position of the enemy

//    void Start()
//    {
//        // Set the initial target to point A
//        target = pointA;

//        // Store the initial y position of the enemy
//        yPosition = transform.position.y;
//    }

//    void Update()
//    {
//        // Calculate the next position along the x-axis
//        Vector2 nextPosition = Vector2.MoveTowards(
//            new Vector2(transform.position.x, yPosition), // Current x position and fixed y
//            new Vector2(target.position.x, yPosition),   // Target x position and fixed y
//            speed * Time.deltaTime
//        );

//        // Update the enemy's position
//        transform.position = new Vector3(nextPosition.x, yPosition, transform.position.z);

//        // Flip the sprite and rotate fire point based on direction
//        FlipSpriteAndFirePoint();

//        // Check if the enemy has reached the target
//        if (Mathf.Abs(transform.position.x - target.position.x) < 0.1f)
//        {
//            // Switch target between point A and point B
//            target = target == pointA ? pointB : pointA;
//        }
//    }

//    void FlipSpriteAndFirePoint()
//    {
//        // Flip the sprite based on the direction of movement
//        if (target == pointA && transform.localScale.x > 0) // Moving towards point A
//        {
//            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

//            // Rotate the fire point 180 degrees in the y-axis
//            if (firePoint != null)
//            {
//                firePoint.localRotation = Quaternion.Euler(0, 180, 0);
//            }
//        }
//        else if (target == pointB && transform.localScale.x < 0) // Moving towards point B
//        {
//            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

//            // Reset the fire point rotation
//            if (firePoint != null)
//            {
//                firePoint.localRotation = Quaternion.Euler(0, 0, 0);
//            }
//        }
//    }

//    private void OnDrawGizmos()
//    {
//        // Draw lines between patrol points in the editor for visualization
//        if (pointA != null && pointB != null)
//        {
//            Gizmos.color = Color.red;
//            Gizmos.DrawLine(pointA.position, pointB.position);
//        }
//    }
//}

using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public Transform pointA; // First patrol point
    public Transform pointB; // Second patrol point
    public Transform firePoint; // Fire point reference
    public GameObject glaserPrefab; // The Glaser projectile prefab
    public float speed = 2f; // Patrol speed
    public float detectionRange = 5f; // Detection range to spot the player
    public float shootingRange = 3f; // Shooting range to start shooting
    public float projectileSpeed = 5f; // Speed of the projectile

    private Transform target; // Current target point
    private float yPosition; // Fixed y position of the enemy
    private Transform player; // Reference to the player
    private bool isPlayerInRange = false; // To check if the player is in detection range

    void Start()
    {
        // Set the initial target to point A
        target = pointA;

        // Store the initial y position of the enemy
        yPosition = transform.position.y;

        // Find the player in the scene
        player = GameObject.FindWithTag("Player").transform;  // Assumes player has the "Player" tag
    }

    void Update()
    {
        // If the player is in range, stop patrolling and shoot
        if (isPlayerInRange)
        {
            ShootAtPlayer();
        }
        else
        {
            Patrol();
        }
    }

    // Patrol movement: Move between point A and point B
    void Patrol()
    {
        // Calculate the next position along the x-axis
        Vector2 nextPosition = Vector2.MoveTowards(
            new Vector2(transform.position.x, yPosition), // Current x position and fixed y
            new Vector2(target.position.x, yPosition),   // Target x position and fixed y
            speed * Time.deltaTime
        );

        // Update the enemy's position
        transform.position = new Vector3(nextPosition.x, yPosition, transform.position.z);

        // Flip the sprite and fire point based on direction
        FlipSpriteAndFirePoint();

        // Check if the enemy has reached the target
        if (Mathf.Abs(transform.position.x - target.position.x) < 0.1f)
        {
            // Switch target between point A and point B
            target = target == pointA ? pointB : pointA;
        }
    }

    // Detect the player when they enter the collider range
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log("Player detected in range! Switching to shooting mode.");
        }
    }

    // Detect when the player leaves the collider range
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            Debug.Log("Player out of range. Returning to patrolling.");
        }
    }

    // Shoot at the player if within shooting range
    void ShootAtPlayer()
    {
        if (player == null || firePoint == null) return;

        // Calculate the distance to the player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        Debug.Log("Distance to player for shooting: " + distanceToPlayer);

        // Only shoot if the player is within the shooting range
        if (distanceToPlayer <= shootingRange)
        {
            Debug.Log("Player in shooting range. Shooting projectile.");
            Shoot();
        }
    }

    // Create and fire the projectile towards the player
    void Shoot()
    {
        if (glaserPrefab != null && firePoint != null)
        {
            // Instantiate the projectile at the firePoint
            GameObject projectile = Instantiate(glaserPrefab, firePoint.position, Quaternion.identity);

            // Calculate the direction to the player
            Vector3 shootDirection = (player.position - firePoint.position).normalized;

            // Draw a debug line showing the shooting direction
            Debug.DrawLine(firePoint.position, player.position, Color.red, 1f);  // Red line for shooting direction

            // Get the Rigidbody2D component and set its velocity to shoot the projectile
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = new Vector2(shootDirection.x * projectileSpeed, shootDirection.y * projectileSpeed);
                Debug.Log("Projectile shot in direction: " + shootDirection);
            }
            else
            {
                Debug.LogError("Projectile Rigidbody2D not found!");
            }
        }
        else
        {
            Debug.LogError("Glaser prefab or firePoint not assigned!");
        }
    }

    // Flip the sprite and fire point based on movement direction
    void FlipSpriteAndFirePoint()
    {
        // Flip the sprite based on the direction of movement
        if (target == pointA && transform.localScale.x > 0) // Moving towards point A
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

            // Rotate the fire point 180 degrees in the y-axis
            if (firePoint != null)
            {
                firePoint.localRotation = Quaternion.Euler(0, 180, 0);
            }
        }
        else if (target == pointB && transform.localScale.x < 0) // Moving towards point B
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

            // Reset the fire point rotation
            if (firePoint != null)
            {
                firePoint.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }

    private void OnDrawGizmos()
    {
        // Draw lines between patrol points in the editor for visualization
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(pointA.position, pointB.position);
        }

        // Optionally, draw detection and shooting ranges
        if (player != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, detectionRange); // Detection range
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, shootingRange); // Shooting range
        }
    }
}







