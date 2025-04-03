using System.Collections;
using UnityEngine;

public class ControllerV1 : MonoBehaviour
{
    public float movementSpeed = 5f;
    public float gravityFactor = 1f;
    public float dashForce = 10f;
    public float maxSpeed = 10f;

    private Vector2 velocity; // Final velocity
    private Vector2 inputVector; // Movement input (A/D, W/S)
    private Vector2 dashVector; // Dash impulse
    private Vector2 gravityVector; // Gravity effect
    private Rigidbody2D rb;
    private bool isHardened = false;
    private float hardenTime = 0.2f;
    private float hardenTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        HandleInput();

        // Harden on holding right-click
        if (Input.GetMouseButton(1)) // Holding right-click
        {
            hardenTimer += Time.deltaTime;
            if (hardenTimer >= hardenTime)
            {
                isHardened = true;
                // Apply visual/behavioral changes (e.g., slower movement)
                movementSpeed *= 0.5f;
            }
        }
        else
        {
            // Reset harden state and speed when releasing right-click
            isHardened = false;
            hardenTimer = 0;
            movementSpeed = 5f; // Default speed
        }
    }

    void FixedUpdate()
    {
        ApplyForces(); // Calculate and apply forces
    }

    void HandleInput()
    {
        // Horizontal movement input
        float horizontal = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right
        inputVector = new Vector2(horizontal, 0) * movementSpeed;

        // Vertical input for modifying gravity (W/S or Up/Down)
        float vertical = Input.GetAxisRaw("Vertical");
        gravityVector = new Vector2(0, -Physics2D.gravity.y * gravityFactor * (1 + vertical));

        // Dash on right-click (impulse toward mouse)
        if (Input.GetMouseButtonDown(1)) // Right-click
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            dashVector = (mousePosition - (Vector2)transform.position).normalized * dashForce;
        }
        else
        {
            dashVector = Vector2.zero; // Reset dash vector if not dashing
        }
    }

    void ApplyForces()
    {
        // Reset velocity to zero before adding contributions
        velocity = Vector2.zero;

        // Add inputs
        velocity += inputVector;

        // Add gravity
        velocity += gravityVector;

        // Add dash impulse
        velocity += dashVector;

        // Clamp velocity to max speed
        velocity = Vector2.ClampMagnitude(velocity, maxSpeed);

        // Apply the resultant velocity to the Rigidbody
        rb.linearVelocity = velocity;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Apply collision forces (e.g., slime ramming)
        if (collision.gameObject.CompareTag("Slime"))
        {
            Vector2 collisionForce = collision.relativeVelocity * 0.5f; // Example force
            velocity += collisionForce;
        }
    }

}
