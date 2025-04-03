using UnityEngine;

public class MapBoundary : MonoBehaviour
{
    void OnTriggerExit2D(Collider2D other)
    {
        // Ignore the player
        if (other.CompareTag("Player"))
            return;

        Debug.Log($"Despawning {other.name} as it left the map boundaries.");
        Destroy(other.gameObject);
    }
}
