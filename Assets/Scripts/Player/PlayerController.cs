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
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float dashForce = 20f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1.5f;

    [Header("Sprint Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private bool startsRunning = true;

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
    private bool isRunning;
    private float currentSpeed;
    private bool isDashing;
    private bool isFrozen;
    private float dashTimer;
    private float dashCooldownTimer;
    private float coyoteTimer;
    private float jumpBufferTimer;
    private int jumpsRemaining;

    // ── References ────────────────────────────────────────────────────────────
    private Rigidbody rb;
    private Animator animator;
    private Renderer rend;
    private Color originalColor;

    // ── Unity Lifecycle ───────────────────────────────────────────────────────

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        rend = GetComponentInChildren<Renderer>();
        if (rend) originalColor = rend.material.color;

        // Ensure groundLayer includes "Ground" layer if not set
        if (groundLayer == 0)
        {
            int gl = LayerMask.NameToLayer("Ground");
            if (gl != -1)
                groundLayer = 1 << gl;
        }

        // Ensure groundCheck is assigned; try to find a child named "GroundCheck"
        if (groundCheck == null)
        {
            Transform gc = transform.Find("GroundCheck");
            if (gc != null)
                groundCheck = gc;
            else
            {
                // Create a new child for ground checking
                GameObject go = new GameObject("GroundCheck");
                go.transform.SetParent(transform);
                go.transform.localPosition = new Vector3(0, 0.1f, 0);
                groundCheck = go.transform;
            }
        }

        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
        rb.useGravity = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        jumpsRemaining = maxJumps;
        isRunning = startsRunning;
        currentSpeed = isRunning ? runSpeed : walkSpeed;
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
        inputReader.OnSprintStarted += HandleSprintToggle;
    }

    private void OnDisable()
    {
        if (inputReader == null) return;

        inputReader.OnMoveEvent    -= HandleMove;
        inputReader.OnJumpStarted  -= HandleJumpStarted;
        inputReader.OnDashStarted  -= HandleDashStarted;
        inputReader.OnSprintStarted -= HandleSprintToggle;
    }

    // ── Input Handlers ────────────────────────────────────────────────────────

    private void HandleMove(Vector2 input)
    {
        if (isFrozen)
        {
            moveInput = Vector2.zero;
            return;
        }

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
        if (isFrozen) return;
        if (VeinsOfMalice.UI.DialogueUI.Instance != null && VeinsOfMalice.UI.DialogueUI.Instance.IsDisplaying) return;
        
        // Siempre activar el buffer; TryJump decide si se puede saltar
        jumpBufferTimer = jumpBufferTime;
    }

    private void HandleDashStarted()
    {
        if (isFrozen) return;
        if (VeinsOfMalice.UI.DialogueUI.Instance != null && VeinsOfMalice.UI.DialogueUI.Instance.IsDisplaying) return;

        if (!isDashing && dashCooldownTimer <= 0f && moveInput.magnitude > 0)
            StartDash();
    }

    private void HandleSprintToggle()
    {
        if (isFrozen) return;
        isRunning = !isRunning;
        currentSpeed = isRunning ? runSpeed : walkSpeed;
        Debug.Log(isRunning ? "<color=green>[Movement]</color> Running Mode" : "<color=yellow>[Movement]</color> Walking Mode");
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

        if (!isDashing && !isFrozen)
            Move();

        HandleJumpBuffer();
    }

    // ── Movement ──────────────────────────────────────────────────────────────

    private void Move()
    {
        Vector3 targetVelocity = new Vector3(moveInput.x * currentSpeed, rb.linearVelocity.y, 0);
        rb.linearVelocity = targetVelocity;
    }

    public void Freeze(float duration)
    {
        if (gameObject.activeInHierarchy)
            StartCoroutine(FreezeRoutine(duration));
    }

    private System.Collections.IEnumerator FreezeRoutine(float duration)
    {
        if (isFrozen) yield break;

        isFrozen = true;
        moveInput = Vector2.zero;
        
        // Visual feedback
        if (rend) rend.material.color = new Color(0.5f, 0.8f, 1f); // Celeste/Hielo
        
        // Stop movement
        rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);

        yield return new WaitForSeconds(duration);

        if (rend) rend.material.color = originalColor;
        isFrozen = false;
    }

    public bool IsFrozen => isFrozen;
    public bool IsGrounded => isGrounded;

    public void InitiateDownslam(float force)
    {
        if (isFrozen) return;
        rb.linearVelocity = new Vector3(0, -force, 0);
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
            rb.linearVelocity = new Vector3(moveInput.x * currentSpeed, rb.linearVelocity.y, 0);
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

        bool hitCenter = Physics.Raycast(origin, Vector3.down, rayDistance);
bool hitLeft   = Physics.Raycast(origin + Vector3.left * sideOffset, Vector3.down, rayDistance);
bool hitRight  = Physics.Raycast(origin + Vector3.right * sideOffset, Vector3.down, rayDistance);

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
