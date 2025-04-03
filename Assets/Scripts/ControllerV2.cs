using System.Collections;
using UnityEngine;

public class ControllerV2 : MonoBehaviour
{
    public float movementSpeed = 40f;
    public float gravityFactor = 1f;
    public float dashForce = 400;
    private Vector2 velocity; // Final velocity
    private Vector2 inputVector; // Movement input (A/D, W/S)
    private Vector2 dashVector; // Dash impulse
    private Vector2 gravityVector; // Gravity effect
    private Vector2 jumpVector;
    private bool isHardened = false;
    private float hardnessPeak = 0.5f;
    private float hardnessMin = 0.1f;
    private float hardnessDecay = 0.75f;
    private float hardnessInstant = 0.0f;
    private float hardnessPassive = 0.0f;
    private float hardenTime = 0.2f;
    private float hardenTimer;
    private bool isGrounded;
    private bool isNearWall;
    public float jumpForce = 60f;
    public float wallJumpForce = 60f;
    private bool isDashing = false;
    private float dashDuration = 0.5f; // How long the dash lasts
    private float dashTimeRemaining;



    [Header("INPUT")] [Tooltip("Makes all Input snap to an integer. Prevents gamepads from walking slowly. Recommended value is true to ensure gamepad/keybaord parity.")]
    public bool SnapInput = true;

    [Tooltip("Minimum input required before you mount a ladder or climb a ledge. Avoids unwanted climbing using controllers"), Range(0.01f, 0.99f)]
    public float VerticalDeadZoneThreshold = 0.3f;

    [Tooltip("Minimum input required before a left or right is recognized. Avoids drifting with sticky controllers"), Range(0.01f, 0.99f)]
    public float HorizontalDeadZoneThreshold = 0.1f;

    [Tooltip("The gravity multiplier added when jump is released early")]
    public float JumpEndEarlyGravityModifier = 2;

    [Tooltip("The time before coyote jump becomes unusable. Coyote jump allows jump to execute even after leaving a ledge")]
    public float CoyoteTime = .15f;

    [Tooltip("The amount of time we buffer a jump. This allows jump input before actually hitting the ground")]
    public float JumpBuffer = .2f;

    private Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Handle Dash Input
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && isGrounded)
        {
            StartDash(); // Start dash when spacebar is pressed (or another key you choose)
        }
    }

    void FixedUpdate()
    {
        GetHard();
        CheckGrounded();
        CheckWall();
        HandleInput();
        ApplyForces();
    }

    void GetHard(){
        // Harden on holding right-click
        if (Input.GetMouseButton(1)) // Holding right-click
        {
            hardenTimer += Time.fixedDeltaTime;
            if (hardenTimer >= hardenTime & isHardened == false)
            {
                isHardened = true;
                hardnessInstant = hardnessPeak;
            }
            if(isHardened){
                hardnessInstant *= (1 - (hardnessDecay * Time.fixedDeltaTime));
                hardnessInstant = Mathf.Clamp(hardnessInstant, hardnessMin, hardnessPeak); // Prevent going too low
                //Debug.Log(hardnessInstant);
            }
        }
        else
        {
            // Reset harden state and speed when releasing right-click
            isHardened = false;
            hardenTimer = 0;
            hardnessInstant = Mathf.Lerp(hardnessInstant, hardnessPassive, Time.fixedDeltaTime * 1f);
            //Debug.Log(hardnessInstant);
        }
    }

    void HandleInput()
    {
        // Horizontal movement input
        float horizontal = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right
        inputVector = new Vector2(horizontal, 0) * movementSpeed;

        // Vertical input for modifying gravity (W/S or Up/Down)
        float vertical = Input.GetAxisRaw("Vertical") * 0.5f;
        gravityVector = new Vector2(0, gravityFactor * (1 - vertical) * (1 + hardnessInstant));
        Debug.Log(gravityVector);

        // Jump input
        if (Input.GetKeyDown(KeyCode.Space) & (isGrounded || isNearWall)) // Allow jump if grounded or near a wall
        {
            if (isGrounded)
            {
                jumpVector = new Vector2(0, jumpForce * (1 - hardnessInstant)); // Jump force when grounded
                Debug.Log(jumpVector);
            }
            else if (isNearWall)
            {
                // Wall jump
                jumpVector = new Vector2(-Mathf.Sign(transform.localScale.x) * movementSpeed * (1 - hardnessInstant), 0); // Move away from the wall
                jumpVector.y = wallJumpForce * (1 - hardnessInstant); // Apply a vertical force for wall jump
            }
        }
    }

    void ApplyForces()
    {
        // Reset velocity to zero before adding contributions
        velocity = Vector2.zero;

        // Add inputs
        velocity += inputVector;

        // Add gravity
        if(!isDashing)
        {
            velocity += gravityVector;
        }

        // Add dash impulse if dashing
        if (isDashing)
        {
            velocity += dashVector;
            dashTimeRemaining -= Time.fixedDeltaTime;
            if (dashTimeRemaining <= 0)
            {
                isDashing = false; // Stop dashing after the dash duration
            }
        }

        velocity += jumpVector;
        jumpVector = Vector2.zero;

        Debug.Log(velocity);
        // Apply the resultant velocity to the Rigidbody
        rb.AddForce(velocity);
    }

    void CheckGrounded()
    {
        // Cast a small raycast downward to check if the slime is grounded
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.05f);
        isGrounded = hit.collider != null; // If there’s a collider below the slime, it’s grounded
        Debug.Log(isGrounded);
    }

    void CheckWall()
    {
        // Cast rays to check if the slime is near a wall
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, 0.05f);
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, 0.05f);

        isNearWall = hitLeft.collider != null || hitRight.collider != null; // If there's a wall nearby
    }

    void StartDash()
    {
        // Dash in the direction of the mouse
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        dashVector = (mousePosition - (Vector2)transform.position).normalized * dashForce * (1 - hardnessInstant);
        dashTimeRemaining = dashDuration;
        isDashing = true;
    }

}
