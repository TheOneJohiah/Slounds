using Unity.VisualScripting;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public Transform slimeBody; // Reference to the slime
    public Transform firePoint; // Where bullets spawn
    public GameObject bulletPrefab; // Bullet prefab to instantiate
    private float fireRate = 0.1f; // Rate of fire
    private float nextFireTime = 0f;

    void Update()
    {
        RotateGun();

        if (Input.GetButtonDown("Fire1") && nextFireTime <= 0) // Fire when left mouse button is pressed
        {
            FireBullet();
            nextFireTime = fireRate;
        }
    }

    void FixedUpdate()
    {
        nextFireTime = nextFireTime - Time.fixedDeltaTime;
    }

    void RotateGun()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePos - slimeBody.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Flip the gun if aiming left (prevents upside-down gun)
        if (mousePos.x < slimeBody.position.x)
            transform.localScale = new Vector3(0.1f, -0.1f, 0.1f); // Flip vertically
        else
            transform.localScale = new Vector3(0.1f, 0.1f, 0.1f); // Normal

        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void FireBullet()
    {
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos3D.x, mousePos3D.y);
        Vector3 slimeBody3D = slimeBody.position;
        Vector2 slimeBody2D = new Vector2(slimeBody3D.x, slimeBody3D.y);
        Vector2 direction = (mousePos2D - slimeBody2D).normalized; // Get direction towards mouse
        Vector2 slimeSpeed = slimeBody.GetComponent<Rigidbody2D>().linearVelocity;
        // Spawn bullet just outside the slime's collider edge
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.GetComponent<Bullet>().Fire(direction, slimeSpeed); // Launch the bullet in the direction
    }
/*
TODO: Second player/multiplayer/dummy
TODO: Sound effects
TODO: Wall jumps, dashes
TODO: Bullet/ramming damage
*/
}
