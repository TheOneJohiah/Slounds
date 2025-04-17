using System.Collections;
using UnityEngine;

public class SlimeController : MonoBehaviour
{
    public float movementSpeed = 10f;
    public float gravityFactor = 1f;
    public float dashForce = 10f;
    private Vector2 targetVelocity; // Final targetVelocity
    private Vector2 inputVector; // Movement input (A/D, W/S)
    private Vector2 dashVector; // Dash impulse
    private Vector2 gravityVector; // Gravity effect
    private Vector2 jumpVector = Vector2.zero;
    private bool isHardened = false;
    private float hardnessPeak = 0.5f;
    private float hardnessMin = 0.1f;
    private float hardnessDecay = 0.75f;
    private float hardnessInstant = 0.0f;
    private float hardnessPassive = 0.0f;
    private float hardenTime = 0.2f;
    private float hardenTimer;
    private bool isGrounded;
    private bool isNearWallLeft;
    private bool isNearWallRight;
    public float jumpForce = 1000f;
    public float wallJumpForce = 10f;
    public Transform groundCheck;
    public Transform wallCheckLeft;
    public Transform wallCheckRight;
    public LayerMask groundLayer;
    public LayerMask wallLayer;
    public float groundCheckRadius = 0.4f; // Adjust as needed
    public float wallCheckRadius = 0.4f;
    private bool isDashing = false;
    private bool isJumping = false;
    private bool isWallJumping = false;
    private float dashDuration = 0.5f; // How long the dash lasts
    private float dashTimeRemaining;
    private float timeSinceGround;
    private float timeSinceWall;
    private float timeSinceWallRight;
    private float timeSinceWallLeft;
    private float CoyoteTime = .15f;
    private float JumpBuffer = .2f;
    private int health = 100;
    private int currentHealth;

    [Header("INPUT")] [Tooltip("Makes all Input snap to an integer. Prevents gamepads from walking slowly. Recommended value is true to ensure gamepad/keybaord parity.")]
    public bool SnapInput = true;

    [Tooltip("Minimum input required before you mount a ladder or climb a ledge. Avoids unwanted climbing using controllers"), Range(0.01f, 0.99f)]
    public float VerticalDeadZoneThreshold = 0.3f;

    [Tooltip("Minimum input required before a left or right is recognized. Avoids drifting with sticky controllers"), Range(0.01f, 0.99f)]
    public float HorizontalDeadZoneThreshold = 0.1f;    

    private Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = health;
    }

    void Update()
    {
        // Handle Dash Input
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing)
        {
            StartDash(); // Start dash when spacebar is pressed (or another key you choose)
        }
        // Handle Jump Input
        HandleJump();
    }

    void FixedUpdate()
    {
        GetHard();
        CheckGrounded();
        CheckWall();
        HandleInput();
        ApplyForces();

        // Check if the slime is out of bounds
        CheckBounds();
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
                hardnessInstant = hardnessInstant * (1 - (hardnessDecay * Time.fixedDeltaTime));
                hardnessInstant = Mathf.Clamp(hardnessInstant, hardnessMin, hardnessPeak); // Prevent going too low
                //Debug.Log(hardnessInstant);
            }
        }
        else
        {
            // Reset harden state and speed when releasing right-click
            isHardened = false;
            hardenTimer = 0;
            hardnessInstant = Mathf.Lerp(hardnessInstant, hardnessPassive, Time.fixedDeltaTime * 10f);
            //Debug.Log(hardnessInstant);
        }
    }

    void HandleJump()
    {
        // Jump input
        if (Input.GetKeyDown(KeyCode.Space) & (isGrounded || isNearWallLeft || isNearWallRight)) // Allow jump if grounded or near a wall
        {
            if (isGrounded)
            {
                jumpVector = new Vector2(0, jumpForce * (1 - hardnessInstant)); // Jump force when grounded
                isJumping = true;
                timeSinceGround = 0;
                Debug.Log(jumpVector);
            }
            else if (isNearWallLeft || isNearWallRight)
            {
                // Wall jump
                jumpVector = new Vector2(-Mathf.Sign(transform.localScale.x) * movementSpeed * (1 - hardnessInstant), 0); // Move away from the wall
                jumpVector.y = wallJumpForce * (1 - hardnessInstant); // Apply a vertical force for wall jump
            }
        }

    }

    void HandleInput()
    {
        // Horizontal movement input
        float horizontal = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right
        inputVector = new Vector2(horizontal, 0) * movementSpeed * (1 - hardnessInstant);

        // Vertical input for modifying gravity (W/S or Up/Down)
        float vertical = Input.GetAxisRaw("Vertical") * 0.5f;
        gravityVector = new Vector2(0, gravityFactor * (1 - vertical) * (1 + hardnessInstant));
        //Debug.Log(gravityVector);
    }

    void ApplyForces()
    {
        // Reset targetVelocity to zero before adding contributions
        targetVelocity = Vector2.zero;

        // Add inputs
        targetVelocity += inputVector;

        float velocityDifference = targetVelocity.x - rb.linearVelocityX;

        float accelRate = (Mathf.Abs(velocityDifference) > 0.01f) ? 10 : -10;
        //Debug.Log(targetVelocity);
        // Apply the resultant targetVelocity to the Rigidbody
        
        float movement = Mathf.Pow(Mathf.Abs(velocityDifference) * accelRate, 1) * Mathf.Sign(velocityDifference);

        if(isGrounded)
        {
            rb.AddForce(movement * Vector2.right, ForceMode2D.Force);
        }

        /* Code for ability movement */
        targetVelocity = Vector2.zero;

        // Add dash impulse if dashing
        if (isDashing)
        {
            if(dashTimeRemaining == dashDuration)
            {
                rb.AddForce(dashVector, ForceMode2D.Impulse);
            }
            dashTimeRemaining -= Time.fixedDeltaTime;
            if (dashTimeRemaining <= 0)
            {
                isDashing = false; // Stop dashing after the dash duration
            }
            
        }
        // Add jump impulse if dashing
        else if (isJumping)
        {
            targetVelocity += jumpVector;
            isJumping = false; // Stop jumping after the jump duration
        }
        else
        {
            targetVelocity += gravityVector;
        }
        
        rb.AddForce(targetVelocity, ForceMode2D.Force);
    }

    void CheckGrounded()
    {
        // Cast a small raycast downward to check if the slime is grounded
        //RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.1f);
        //isGrounded = hit.collider != null; // If there’s a collider below the slime, it’s grounded
        //Debug.Log(isGrounded);
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    void CheckWall()
    {
        // Cast rays to check if the slime is near a wall
        //RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, 0.1f);
        //RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, 0.1f);

        //isNearWall = hitLeft.collider != null || hitRight.collider != null; // If there's a wall nearby
        isNearWallLeft = Physics2D.OverlapCircle(wallCheckLeft.position, wallCheckRadius, wallLayer);
        isNearWallRight = Physics2D.OverlapCircle(wallCheckRight.position, wallCheckRadius, wallLayer);
    }

    void StartDash()
    {
        // Dash in the direction of the mouse
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        dashVector = (mousePosition - (Vector2)transform.position).normalized * dashForce * (1 - hardnessInstant);
        dashTimeRemaining = dashDuration;
        isDashing = true;
    }

    void CheckBounds()
    {
        // Define boundaries (you can adjust these values)
        float boundX = 20f; // Horizontal limit
        float boundY = 10f; // Vertical limit

        Vector2 pos = transform.position;
        if (Mathf.Abs(pos.x) > boundX || Mathf.Abs(pos.y) > boundY)
        {
            // Teleport back to (0,0)
            transform.position = Vector2.zero;
            // Optionally, reset velocity:
            rb.linearVelocity = Vector2.zero;
        }
    }

    public void GetHurt(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Player took damage, current health: " + currentHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Handle player death (respawn, game over, etc.)
        Debug.Log("Player died");
        Destroy(gameObject);
    }

}

