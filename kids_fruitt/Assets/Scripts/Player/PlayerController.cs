using UnityEngine;
using static UnityEngine.UI.ScrollRect;

public enum MovementType
{
    Rolling,      // For oval shapes like potatoes
    Tilting       // For straight shapes like carrots
}

public enum MovementAxis
{
    XAxis,        // Movement along X axis
    ZAxis         // Movement along Z axis
}

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private MovementType movementType = MovementType.Rolling;
    [SerializeField] private MovementAxis movementAxis = MovementAxis.ZAxis;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 7f;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private AudioClip jumpClip;

    [Header("Jump Settings")]
    [SerializeField] private float jumpCooldown = 0.2f;
    [SerializeField] private float maxJumpVelocity = 10f;
    [SerializeField] private bool resetYVelocityOnJump = true;
    [SerializeField] private float slopedSurfaceYVelocityThreshold = 2f;

    private Rigidbody rb;
    private bool isGrounded;
    private float currentHorizontalInput;
    private bool jumpRequested;
    private PlayerVisuals playerVisuals;
    private PlayerInputHandler inputHandler;
    private AudioSource audioSource;

    private float lastJumpTime = -1f;
    private bool wasGroundedLastFrame = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerVisuals = GetComponent<PlayerVisuals>();
        inputHandler = GetComponent<PlayerInputHandler>();
        audioSource = GetComponent<AudioSource>();

        // Subscribe to input events
        inputHandler.OnMovementInput += HandleMovementInput;
        inputHandler.OnJumpInput += HandleJumpInput;
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        if (inputHandler != null)
        {
            inputHandler.OnMovementInput -= HandleMovementInput;
            inputHandler.OnJumpInput -= HandleJumpInput;
        }
    }

    private void HandleMovementInput(Vector2 input)
    {
        currentHorizontalInput = input.x;
    }

    private void HandleJumpInput()
    {
        if (CanJump())
        {
            jumpRequested = true;
        }
    }

    private bool CanJump()
    {
        bool velocityCheck = Mathf.Abs(rb.linearVelocity.y) <= slopedSurfaceYVelocityThreshold;
        bool cooldownCheck = (Time.time - lastJumpTime > jumpCooldown);

        return isGrounded &&
               cooldownCheck &&
               !rb.isKinematic &&
               velocityCheck;
    }

    private void Update()
    {
        // Check if grounded with improved detection
        bool previousGrounded = isGrounded;
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
        if (!previousGrounded && isGrounded)
        {
            lastJumpTime = -jumpCooldown;
        }

        rb.isKinematic = MiniGamesManager.instance.GetIsMiniGameActive();

        wasGroundedLastFrame = isGrounded;

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log($"Y Velocity: {rb.linearVelocity.y}, Can Jump: {CanJump()}");
        }
    }

    private void FixedUpdate()
    {
        Move();

        if (jumpRequested)
        {
            Jump();
            jumpRequested = false;
        }
    }

    private void Move()
    {
        // Create movement direction based on selected axis
        Vector3 moveDirection;
        if (movementAxis == MovementAxis.XAxis)
        {
            moveDirection = new Vector3(currentHorizontalInput, 0, 0).normalized;
        }
        else
        {
            moveDirection = new Vector3(0, 0, currentHorizontalInput).normalized;
        }

        // Apply movement to the rigidbody
        Vector3 currentVelocity = rb.linearVelocity;
        if (movementAxis == MovementAxis.XAxis)
        {
            currentVelocity.x = moveDirection.x * moveSpeed;
        }
        else
        {
            currentVelocity.z = moveDirection.z * moveSpeed;
        }

        if (!rb.isKinematic)
            rb.linearVelocity = new Vector3(currentVelocity.x, rb.linearVelocity.y, currentVelocity.z);

        // Notify visuals about movement
        if (playerVisuals != null)
        {
            playerVisuals.HandleMovement(moveDirection, movementType, movementAxis, isGrounded);
        }
    }

    private void Jump()
    {
        if (!CanJump())
        {
            Debug.Log($"Jump blocked - Grounded: {isGrounded}, Y Velocity: {rb.linearVelocity.y}, Cooldown: {Time.time - lastJumpTime}");
            return;
        }

        lastJumpTime = Time.time;

        if (resetYVelocityOnJump && rb.linearVelocity.y < 1f)
        {
            Vector3 currentVelocity = rb.linearVelocity;
            currentVelocity.y = 0f;
            rb.linearVelocity = currentVelocity;
        }

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        if (rb.linearVelocity.y > maxJumpVelocity)
        {
            Vector3 clampedVelocity = rb.linearVelocity;
            clampedVelocity.y = maxJumpVelocity;
            rb.linearVelocity = clampedVelocity;
        }

        if (audioSource != null && jumpClip != null)
        {
            audioSource.clip = jumpClip;
            audioSource.Play();
        }

        if (playerVisuals != null)
        {
            playerVisuals.HandleJump();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    // Public setters for runtime configuration
    public void SetMovementType(MovementType newType)
    {
        movementType = newType;
        playerVisuals?.ResetRotation();
    }

    public void SetMovementAxis(MovementAxis newAxis)
    {
        movementAxis = newAxis;
        playerVisuals?.ResetRotation();
    }

    public bool IsGrounded() => isGrounded;
    public float GetCurrentYVelocity() => rb.linearVelocity.y;
    public float GetTimeSinceLastJump() => Time.time - lastJumpTime;
}