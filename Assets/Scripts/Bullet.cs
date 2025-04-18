using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float speed = 30f; // Bullet speed
    private int bounceCount = 2; // Number of bounces allowed (set in Inspector)
    private int damage = 10;
    private Vector2 lastVelocity;
    public bool ignoreBoundary = false;
    private Rigidbody2D rb;
    private Collider2D bulletCollider;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bulletCollider = GetComponent<Collider2D>();

        bulletCollider.isTrigger = true;
    }

    void FixedUpdate()
    {
        Vector2 inVelocity = rb.linearVelocity;
        lastVelocity = inVelocity;
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
        SlimeController player = collision.collider.GetComponent<SlimeController>();
        if (player != null)
        {
            // Get the player's health component
            player.GetHurt(damage);
            // Destroy the bullet after hitting the player
            Destroy(gameObject);
        }
        // If bounces remain, reflect the bullet's velocity
        else if (bounceCount > 0)
        {
            // Reflect the velocity using the first contact point's normal
            Vector2 normal = collision.contacts[0].normal;
            Vector2 reflectVelocity = Vector2.Reflect(lastVelocity, normal);
            rb.linearVelocity = reflectVelocity;
            //Debug.Log(collision);
            //Debug.Log(lastVelocity);
            //Debug.Log(reflectVelocity);
            //Debug.Log(rb.linearVelocity);
            bounceCount--; // Use up one bounce
        }
        else
        {
            // No bounces remaining, so destroy the bullet
            ignoreBoundary = true;
            Destroy(gameObject);
        }
    }

}
