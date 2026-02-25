using System.Collections;
using UnityEngine;

/// <summary>
/// PlayerCombat — Gestiona el sistema de ataque, combos y bloqueo del jugador.
/// </summary>
public class PlayerCombat : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private VeinsOfMalice.Player.PlayerEnergy playerEnergy;

    [Header("Combo Settings")]
    [SerializeField] private float comboResetTime = 1f;
    [SerializeField] private int maxComboSteps = 3;

    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 0.2f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackDelay = 0.1f;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float comboStep1Damage = 10f;
    [SerializeField] private float comboStep2Damage = 15f;
    [SerializeField] private float comboStep3Damage = 25f;

    [Header("Energy Costs")]
    [SerializeField] private float attackEnergyCost = 8f; // Coste adicional por cada swing

    // ── State ─────────────────────────────────────────────────────────────────
    private int currentComboStep = 0;
    private float lastAttackTime;
    private bool isBlocking;
    private Renderer rend;
    private Color originalColor;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rend = GetComponentInChildren<Renderer>();
        if (rend) originalColor = rend.material.color;

        if (playerEnergy == null)
            playerEnergy = GetComponent<VeinsOfMalice.Player.PlayerEnergy>();
    }

    private void OnEnable()
    {
        if (inputReader == null) return;

        inputReader.OnAttackStarted += HandleAttack;
        inputReader.OnBlockStarted  += HandleBlockStarted;
        inputReader.OnBlockCanceled += HandleBlockCanceled;
        inputReader.OnHeavyAttackStarted += HandleHeavyAttack;
    }

    private void OnDisable()
    {
        if (inputReader == null) return;

        inputReader.OnAttackStarted -= HandleAttack;
        inputReader.OnBlockStarted  -= HandleBlockStarted;
        inputReader.OnBlockCanceled -= HandleBlockCanceled;
        inputReader.OnHeavyAttackStarted -= HandleHeavyAttack;
    }

    private void Update()
    {
        // Resetear combo si pasa demasiado tiempo
        if (currentComboStep > 0 && Time.time - lastAttackTime > comboResetTime)
        {
            ResetCombo();
        }
    }

    // ── Input Handlers ────────────────────────────────────────────────────────

    private void HandleAttack()
    {
        if (isBlocking) return; // No se puede atacar mientras bloqueas
        if (Time.time - lastAttackTime < attackCooldown) return;

        Attack();
    }

    private void HandleHeavyAttack()
    {
        if (isBlocking) return;
        Debug.Log("<color=magenta><b>[HEAVY ATTACK]</b></color> Cursed Energy Slash! (Prototype)");
        // Por ahora lanzamos un ataque normal para que haga algo
        Attack(); 
    }

    private void HandleBlockStarted()
    {
        isBlocking = true;
        if (animator) animator.SetBool("IsBlocking", true);
        if (rend) rend.material.color = new Color(0.2f, 0.5f, 1f); // Azul claro para bloqueo
        Debug.Log("<color=cyan><b>[BLOCK]</b></color> Defensive Stance Active");
    }

    private void HandleBlockCanceled()
    {
        isBlocking = false;
        if (animator) animator.SetBool("IsBlocking", false);
        if (rend) rend.material.color = originalColor;
        Debug.Log("[PlayerCombat] Blocking Ended");
    }

    // ── Combat Logic ──────────────────────────────────────────────────────────

    private void Attack()
    {
        currentComboStep++;
        if (currentComboStep > maxComboSteps) currentComboStep = 1;

        lastAttackTime = Time.time;

        // Feedback
        Debug.Log($"[PlayerCombat] Attack {currentComboStep}!");
        
        if (animator)
        {
            animator.SetTrigger("Attack");
            animator.SetInteger("ComboStep", currentComboStep);
        }

        if (playerEnergy != null && playerEnergy.IsEnergyModeActive)
        {
            playerEnergy.UseEnergy(attackEnergyCost);
        }

        StartCoroutine(PerformAttackDetection());
    }

    private IEnumerator PerformAttackDetection()
    {
        // Esperar un pequeño delay para que coincida con el "impacto" visual
        yield return new WaitForSeconds(attackDelay);

        Vector3 pos = attackPoint ? attackPoint.position : transform.position + transform.forward;
        Collider[] hits = Physics.OverlapSphere(pos, attackRange, enemyLayer);

        float damage = currentComboStep switch
        {
            1 => comboStep1Damage,
            2 => comboStep2Damage,
            3 => comboStep3Damage,
            _ => 10f
        };

        // Aplicar multiplicador si el modo Energía Maldita está activo
        if (playerEnergy != null && playerEnergy.IsEnergyModeActive)
        {
            damage *= 1.25f;
            Debug.Log($"<color=cyan>[ENERGY BOOST]</color> Damage boosted to {damage}!");
        }

        foreach (var hit in hits)
        {
            // Evitar que el jugador se golpee a sí mismo si está en la misma capa
            if (hit.gameObject == gameObject) continue;

            if (hit.TryGetComponent<IDamageable>(out var damageable))
            {
                Vector3 dir = (hit.transform.position - transform.position).normalized;
                damageable.TakeDamage(damage, dir);
            }
        }
    }

    private void ResetCombo()
    {
        currentComboStep = 0;
        if (animator) animator.SetInteger("ComboStep", 0);
    }

    // ── Public API ────────────────────────────────────────────────────────────

    public bool IsBlocking => isBlocking;

    private void OnDrawGizmosSelected()
    {
        Vector3 pos = attackPoint ? attackPoint.position : transform.position + transform.forward;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(pos, attackRange);
    }
}
