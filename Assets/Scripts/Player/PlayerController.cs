using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float dashForce = 20f;
    [SerializeField] private float dashDuration = 0.2f;

    [Header("Ground Detection")]
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;

    // State
    private Vector2 moveInput;
    private bool isGrounded;
    private bool isDashing;
    private bool isJumpPressed; // Buffered jump
    private float dashTimer;

    // References
    private Rigidbody rb;
    private Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        
        // Ensure Rigidbody is configured for 2.5D
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
        rb.useGravity = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        
        // Flip character model (visuals only)
        if (moveInput.x != 0 && !isDashing)
        {
            // Assuming model is child 0 or we rotate this object
            // For 2.5D, usually rotating the model container is better
            if (transform.childCount > 0)
            {
                Transform model = transform.GetChild(0);
                float yRot = moveInput.x > 0 ? 90f : -90f; // Face right or left
                model.localRotation = Quaternion.Euler(0, yRot, 0);
            }
        }
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed && isGrounded)
        {
            isJumpPressed = true;
        }
    }

    public void OnDash(InputValue value)
    {
        if (value.isPressed && !isDashing && moveInput.magnitude > 0)
        {
            StartDash();
        }
    }

    private void Update()
    {
        HandleDashTimer();
        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        CheckGround();
        
        if (!isDashing)
        {
            Move();
        }

        if (isJumpPressed)
        {
            Jump();
            isJumpPressed = false;
        }
    }

    private void Move()
    {
        // Apply velocity, keeping Y velocity for gravity
        Vector3 targetVelocity = new Vector3(moveInput.x * moveSpeed, rb.linearVelocity.y, 0);
        rb.linearVelocity = targetVelocity;
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, 0); // Reset vertical velocity for consistent jump height
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void StartDash()
    {
        isDashing = true;
        dashTimer = dashDuration;
        
        // Dash logic (impulse)
        Vector3 dashDir = new Vector3(moveInput.x, 0, 0).normalized;
        if (dashDir == Vector3.zero) dashDir = transform.right; // Default forward if no input

        rb.linearVelocity = dashDir * dashForce;
        // Could disable gravity during dash here if desired
    }

    private void HandleDashTimer()
    {
        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0)
            {
                isDashing = false;
                rb.linearVelocity = Vector3.zero; // Stop dash momentum
            }
        }
    }

    private void CheckGround()
    {
        // Simple Raycast check
        Vector3 origin = groundCheck ? groundCheck.position : transform.position;
        isGrounded = Physics.Raycast(origin + Vector3.up * 0.1f, Vector3.down, groundCheckDistance, groundLayer);
        
        // Debug
        Debug.DrawRay(origin + Vector3.up * 0.1f, Vector3.down * groundCheckDistance, isGrounded ? Color.green : Color.red);
    }

    private void UpdateAnimations()
    {
        if (animator == null) return;

        animator.SetFloat("Speed", Mathf.Abs(moveInput.x));
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("IsDashing", isDashing);
        animator.SetFloat("VerticalVelocity", rb.linearVelocity.y);
    }
}
