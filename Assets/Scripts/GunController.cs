using Unity.VisualScripting;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public Transform slimeBody; // Reference to the slime
    public Transform firePoint; // Where bullets spawn
    public GameObject bulletPrefab; // Bullet prefab to instantiate
    public bool fullAuto = true;
    public float fireRate = 0.17f; // time between shots
    private float nextFireTime = 0f;

    void Update()
    {
        RotateGun();

        if (CanFire())
        {
            FireBullet();
            nextFireTime = Time.time + fireRate;
        }
    }

    bool CanFire()
    {
        return (fullAuto ? Input.GetMouseButton(0) : Input.GetButtonDown("Fire1"))
            && Time.time >= nextFireTime;
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
        Vector2 direction = ((Vector2)mousePos3D - (Vector2)slimeBody.position).normalized;
        Vector2 slimeSpeed = slimeBody.GetComponent<Rigidbody2D>().linearVelocity;
        // Spawn bullet just outside the slime's collider edge
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.GetComponent<Bullet>().Fire(direction, slimeSpeed); // Launch the bullet in the direction
    }
}
