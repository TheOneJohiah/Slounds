using UnityEngine;

public class MapBoundary : MonoBehaviour
{
    private Bounds bounds;
    void Awake()
    {
        bounds = GetComponent<Collider2D>().bounds;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Ignore the player
        if (other.CompareTag("Player"))
        {
            Transform player = other.GetComponent<Transform>();
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            // Teleport back to (0,0)
            player.position = Vector2.zero;
            // Optionally, reset velocity:
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // Only despawn if it exited left or right
        float x = other.transform.position.x;
        float y = other.transform.position.y;

        if (x < bounds.min.x || x > bounds.max.x || x < bounds.min.y)
        {
            Bullet bullet = other.GetComponent<Bullet>();
            if (bullet != null && bullet.ignoreBoundary)
                return;

            Debug.Log($"Despawning {other.name} through walls or floor.");
            Destroy(other.gameObject);
        }
        else
        {
            // Do nothing if they leave through top or bottom
            Debug.Log($"{other.name} left through top/bottom, ignoring.");
        }

    }
}
