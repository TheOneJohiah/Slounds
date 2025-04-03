using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float speed = 10f; // Bullet speed
    private int bounceCount = 2; // Number of bounces allowed (set in Inspector)
    private int damage = 10;

    private Rigidbody2D rb;
    private Collider2D bulletCollider;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bulletCollider = GetComponent<Collider2D>();

        bulletCollider.isTrigger = true;
    }

    public void Fire(Vector2 direction, Vector2 playerSpeed)
    {
        // Add a force to the bullet in the given direction
        rb.linearVelocity = (direction * speed) + playerSpeed;
        Debug.Log((direction * speed) + playerSpeed);
    }

    // Once the bullet exits the slime's collider, enable collisions normally
    void OnTriggerExit2D(Collider2D other)
    {
        // Check if we're leaving the firing slime (set the appropriate tag, e.g., "Player" or "Slime")
        if(other.CompareTag("Player"))
        {
            // Now that we've left the slime, enable normal collision detection
            bulletCollider.isTrigger = false;
        }
    }

    // Handle collisions normally once collider is not a trigger
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            // Get the player's health component
            //collision.GetHurt(damage);
            // Destroy the bullet after hitting the player
            Destroy(gameObject);
        }
        // If bounces remain, reflect the bullet's velocity
        else if (bounceCount > 0)
        {
            // Reflect the velocity using the first contact point's normal
            Vector2 inVelocity = rb.linearVelocity;
            Vector2 normal = collision.contacts[0].normal;
            Vector2 reflectVelocity = Vector2.Reflect(inVelocity, normal);
            rb.linearVelocity = reflectVelocity;
            Debug.Log(collision);
            Debug.Log(inVelocity);
            Debug.Log(reflectVelocity);
            Debug.Log(rb.linearVelocity);
            bounceCount--; // Use up one bounce
        }
        else
        {
            // No bounces remaining, so destroy the bullet
            Destroy(gameObject);
        }
    }

}
