using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// PlayerController — Controlador de personaje 2.5D para Veins of Malice.
/// Usa InputReader (ScriptableObject) para desacoplarse del PlayerInput.
/// Requiere que se asigne el InputReader en el Inspector.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputReader inputReader;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float dashForce = 20f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1.5f;

    [Header("Jump Feel")]
    [SerializeField] private float coyoteTime = 0.15f;
    [SerializeField] private float jumpBufferTime = 0.1f;
    [SerializeField] private int maxJumps = 2; // 1 = solo salto normal, 2 = doble salto

    [Header("Ground Detection")]
    [SerializeField] private float groundCheckRadius = 0.25f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;

    // ── State ─────────────────────────────────────────────────────────────────
    private Vector2 moveInput;
    private bool isGrounded;
    private bool isDashing;
    private float dashTimer;
    private float dashCooldownTimer;
    private float coyoteTimer;
    private float jumpBufferTimer;
    private int jumpsRemaining;

    // ── References ────────────────────────────────────────────────────────────
    private Rigidbody rb;
    private Animator animator;

    // ── Unity Lifecycle ───────────────────────────────────────────────────────

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();

        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
        rb.useGravity = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        jumpsRemaining = maxJumps;
    }

    private void OnEnable()
    {
        if (inputReader == null)
        {
            Debug.LogWarning("[PlayerController] InputReader not assigned! Input will not work.");
            return;
        }

        inputReader.OnMoveEvent    += HandleMove;
        inputReader.OnJumpStarted  += HandleJumpStarted;
        inputReader.OnDashStarted  += HandleDashStarted;
    }

    private void OnDisable()
    {
        if (inputReader == null) return;

        inputReader.OnMoveEvent    -= HandleMove;
        inputReader.OnJumpStarted  -= HandleJumpStarted;
        inputReader.OnDashStarted  -= HandleDashStarted;
    }

    // ── Input Handlers ────────────────────────────────────────────────────────

    private void HandleMove(Vector2 input)
    {
        moveInput = input;

        // Flip model based on direction
        if (moveInput.x != 0 && !isDashing && transform.childCount > 0)
        {
            Transform model = transform.GetChild(0);
            float yRot = moveInput.x > 0 ? 90f : -90f;
            model.localRotation = Quaternion.Euler(0, yRot, 0);
        }
    }

    private void HandleJumpStarted()
    {
        // Siempre activar el buffer; TryJump decide si se puede saltar
        jumpBufferTimer = jumpBufferTime;
    }

    private void HandleDashStarted()
    {
        if (!isDashing && dashCooldownTimer <= 0f && moveInput.magnitude > 0)
            StartDash();
    }

    // ── Unity Loop ────────────────────────────────────────────────────────────

    private void Update()
    {
        HandleDashTimer();
        HandleCooldowns();
        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        CheckGround();
        HandleCoyoteTime();
        ResetJumpsOnLanding();

        if (!isDashing)
            Move();

        HandleJumpBuffer();
    }

    // ── Movement ──────────────────────────────────────────────────────────────

    private void Move()
    {
        Vector3 targetVelocity = new Vector3(moveInput.x * moveSpeed, rb.linearVelocity.y, 0);
        rb.linearVelocity = targetVelocity;
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, 0);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        coyoteTimer     = 0f;
        jumpBufferTimer = 0f;
        jumpsRemaining--;
    }

    private void StartDash()
    {
        isDashing = true;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;

        Vector3 dashDir = new Vector3(moveInput.x, 0, 0).normalized;
        if (dashDir == Vector3.zero) dashDir = transform.right;
        rb.linearVelocity = dashDir * dashForce;
    }

    // ── Timers ────────────────────────────────────────────────────────────────

    private void HandleDashTimer()
    {
        if (!isDashing) return;
        dashTimer -= Time.deltaTime;
        if (dashTimer <= 0f)
        {
            isDashing = false;
            rb.linearVelocity = new Vector3(moveInput.x * moveSpeed, rb.linearVelocity.y, 0);
        }
    }

    private void HandleCooldowns()
    {
        if (dashCooldownTimer > 0f) dashCooldownTimer -= Time.deltaTime;
        if (jumpBufferTimer   > 0f) jumpBufferTimer   -= Time.deltaTime;
    }

    private void HandleCoyoteTime()
    {
        coyoteTimer = isGrounded ? coyoteTime : coyoteTimer - Time.fixedDeltaTime;
    }

    private void ResetJumpsOnLanding()
    {
        // Resetear saltos siempre que estemos en el suelo y NO estemos subiendo (para no resetear justo al saltar)
        if (isGrounded && rb.linearVelocity.y <= 0.1f)
        {
            jumpsRemaining = maxJumps;
        }
    }

    private void HandleJumpBuffer()
    {
        if (jumpBufferTimer > 0f && jumpsRemaining > 0)
        {
            Jump();
        }
    }

    // ── Ground Detection ──────────────────────────────────────────────────────

    private void CheckGround()
    {
        // Usamos Raycasts en lugar de cajas/esferas para ser 100% precisos y evitar paredes.
        // Tiramos 3 rayos: uno central y dos laterales ligeramente hacia adentro.
        
        Vector3 origin = groundCheck ? groundCheck.position : transform.position;
        origin += Vector3.up * 0.1f; // Empezamos un poco por encima de los pies

        float rayDistance = 0.2f; // El rayo baja 0.1u por debajo de los pies
        float sideOffset = groundCheckRadius * 0.5f; // Offset lateral para los rayos laterales

        bool hitCenter = Physics.Raycast(origin, Vector3.down, rayDistance, groundLayer);
        bool hitLeft   = Physics.Raycast(origin + Vector3.left * sideOffset, Vector3.down, rayDistance, groundLayer);
        bool hitRight  = Physics.Raycast(origin + Vector3.right * sideOffset, Vector3.down, rayDistance, groundLayer);

        isGrounded = hitCenter || hitLeft || hitRight;

        // Debug visual en el editor
        Debug.DrawRay(origin, Vector3.down * rayDistance, hitCenter ? Color.green : Color.red);
        Debug.DrawRay(origin + Vector3.left * sideOffset, Vector3.down * rayDistance, hitLeft ? Color.green : Color.red);
        Debug.DrawRay(origin + Vector3.right * sideOffset, Vector3.down * rayDistance, hitRight ? Color.green : Color.red);
    }

    // ── Animations ────────────────────────────────────────────────────────────

    private void UpdateAnimations()
    {
        if (animator == null) return;
        animator.SetFloat("Speed",           Mathf.Abs(moveInput.x));
        animator.SetBool("IsGrounded",       isGrounded);
        animator.SetBool("IsDashing",        isDashing);
        animator.SetFloat("VerticalVelocity", rb.linearVelocity.y);
    }

    // ── Gizmos ────────────────────────────────────────────────────────────────

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = isGrounded ? Color.green : Color.red;
        // Dibujamos una línea simple en Gizmos para representar el área de los rayos
        Vector3 origin = groundCheck.position;
        Gizmos.DrawLine(origin + Vector3.left * (groundCheckRadius * 0.5f), origin + Vector3.right * (groundCheckRadius * 0.5f));
    }
}
