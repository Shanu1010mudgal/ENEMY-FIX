using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 5f; // Speed of the bullet
    public float lifetime = 5f; // Lifetime before the bullet is destroyed

    void Start()
    {
        // Destroy the bullet after a set lifetime
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Move the bullet along the x-axis
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Destroy the bullet if it collides with "Enemy" or "Ground"
        if (collision.CompareTag("Enemy") || collision.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
