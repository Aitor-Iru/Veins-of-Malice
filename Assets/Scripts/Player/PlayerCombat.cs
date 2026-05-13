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
    [SerializeField] private float comboCooldown = 1.5f;

    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 0.2f;
    [SerializeField] private float heavyAttackCooldown = 5.0f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float heavyAttackRange = 2.0f;
    [SerializeField] private float attackDelay = 0.1f;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float comboStep1Damage = 10f;
    [SerializeField] private float comboStep2Damage = 15f;
    [SerializeField] private float comboStep3Damage = 25f;
    [SerializeField] private float heavyAttackDamage = 80f;
    [SerializeField] private float downslamDamage = 7f;

    [Header("Energy Costs")]
    [SerializeField] private float attackEnergyCost = 8f; // Coste adicional por cada swing
    [SerializeField] private float heavyAttackEnergyCost = 60f; // Coste mucho más alto
    [SerializeField] private float downslamEnergyCost = 15f;

    [Header("Downslam Settings")]
    [SerializeField] private float downslamCooldown = 2.0f;
    [SerializeField] private float downslamRadius = 3.5f;

    // ── State ─────────────────────────────────────────────────────────────────
    private int currentComboStep = 0;
    private float lastAttackTime;
    private float lastHeavyAttackTime;
    private float lastDownslamTime;
    private float comboCooldownEndTime;
    private bool isBlocking;
    private Renderer rend;
    private Color originalColor;
    private Animator animator;
    private PlayerController playerController;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rend = GetComponentInChildren<Renderer>();
        if (rend) originalColor = rend.material.color;

        if (playerEnergy == null)
            playerEnergy = GetComponent<VeinsOfMalice.Player.PlayerEnergy>();
        
        playerController = GetComponent<PlayerController>();
    }

    private void OnEnable()
    {
        if (inputReader == null) return;

        inputReader.OnAttackStarted += HandleAttack;
        inputReader.OnBlockStarted  += HandleBlockStarted;
        inputReader.OnBlockCanceled += HandleBlockCanceled;
        inputReader.OnHeavyAttackStarted += HandleHeavyAttack;
        inputReader.OnDownslamStarted += HandleDownslam;
    }

    private void OnDisable()
    {
        if (inputReader == null) return;

        inputReader.OnAttackStarted -= HandleAttack;
        inputReader.OnBlockStarted  -= HandleBlockStarted;
        inputReader.OnBlockCanceled -= HandleBlockCanceled;
        inputReader.OnHeavyAttackStarted -= HandleHeavyAttack;
        inputReader.OnDownslamStarted -= HandleDownslam;
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
        if (isBlocking || (playerController != null && playerController.IsFrozen)) return;
        if (VeinsOfMalice.UI.DialogueUI.Instance != null && VeinsOfMalice.UI.DialogueUI.Instance.IsDisplaying) return;
        
        if (Time.time < comboCooldownEndTime) return;
        if (Time.time - lastAttackTime < attackCooldown) return;

        Attack();
    }

    private void HandleHeavyAttack()
    {
        if (isBlocking || (playerController != null && playerController.IsFrozen)) return;
        if (VeinsOfMalice.UI.DialogueUI.Instance != null && VeinsOfMalice.UI.DialogueUI.Instance.IsDisplaying) return;

        if (Time.time - lastHeavyAttackTime < heavyAttackCooldown)
        {
            Debug.Log("<color=orange>[Heavy Attack] Cooldown active...</color>");
            return;
        }

        HeavyAttack();
    }

    private void HandleDownslam()
    {
        Debug.Log("<color=white>[Combat]</color> HandleDownslam called from Input!");
        if (isBlocking) 
        {
            Debug.Log("<color=yellow>[Combat]</color> Downslam blocked: currently blocking.");
            return;
        }
        if (playerController != null && playerController.IsFrozen)
        {
            Debug.Log("<color=yellow>[Combat]</color> Downslam blocked: player is frozen.");
            return;
        }
        if (VeinsOfMalice.UI.DialogueUI.Instance != null && VeinsOfMalice.UI.DialogueUI.Instance.IsDisplaying)
        {
            Debug.Log("<color=yellow>[Combat]</color> Downslam blocked: dialogue active.");
            return;
        }

        if (Time.time - lastDownslamTime < downslamCooldown)
        {
            Debug.Log("<color=yellow>[Combat]</color> Downslam blocked: cooldown active.");
            return;
        }

        Downslam();
    }

    private void HandleBlockStarted()
    {
        if (playerController != null && playerController.IsFrozen) return;
        if (VeinsOfMalice.UI.DialogueUI.Instance != null && VeinsOfMalice.UI.DialogueUI.Instance.IsDisplaying) return;
        
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

    public void BreakShield()
    {
        if (isBlocking)
        {
            HandleBlockCanceled();
            Debug.Log("<color=red><b>[SHIELD BROKEN]</b></color>");
        }
    }

    // ── Combat Logic ──────────────────────────────────────────────────────────

    private void Attack()
    {
        currentComboStep++;
        if (currentComboStep > maxComboSteps) currentComboStep = 1;

        lastAttackTime = Time.time;

        if (currentComboStep == maxComboSteps)
        {
            // Apply extra cooldown after finishing the full combo
            comboCooldownEndTime = Time.time + comboCooldown;
        }

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

        StartCoroutine(PerformAttackDetection(false));
    }

    private void HeavyAttack()
    {
        lastHeavyAttackTime = Time.time;
        comboCooldownEndTime = 0f; // Asegurar que el cooldown del combo se limpie al usar un golpe fuerte
        ResetCombo();

        Debug.Log("<color=magenta><b>[HEAVY ATTACK]</b></color> Massive Energy Slash!");

        if (animator)
        {
            animator.SetTrigger("HeavyAttack"); // Asumimos que existe o se añadirá
        }

        if (playerEnergy != null && playerEnergy.IsEnergyModeActive)
        {
            playerEnergy.UseEnergy(heavyAttackEnergyCost);
        }

        StartCoroutine(PerformAttackDetection(true));
    }

    private void Downslam()
    {
        if (playerController != null && playerController.IsGrounded)
        {
            Debug.Log("<color=yellow>[Combat]</color> Downslam requires being in the air!");
            return;
        }

        StartCoroutine(DownslamRoutine());
    }

    private IEnumerator DownslamRoutine()
    {
        lastDownslamTime = Time.time;
        Debug.Log("<color=orange><b>[DOWNSLAM]</b></color> Slamming down!");

        // Caída rápida
        float slamSpeed = 25f;
        while (playerController != null && !playerController.IsGrounded)
        {
            playerController.InitiateDownslam(slamSpeed);
            yield return null;
        }

        // IMPACTO AL LLEGAR AL SUELO
        Debug.Log("<color=red><b>[DOWNSLAM IMPACT]</b></color>");

        if (animator) animator.SetTrigger("Downslam");

        if (playerEnergy != null && playerEnergy.IsEnergyModeActive)
        {
            playerEnergy.UseEnergy(downslamEnergyCost);
        }

        // Camera Shake
        CameraController cam = Camera.main != null ? Camera.main.GetComponent<CameraController>() : null;
        if (cam != null) cam.Shake();

        // Detectar enemigos en un círculo alrededor del jugador
        Collider[] hits = Physics.OverlapSphere(transform.position, downslamRadius, enemyLayer);
        bool isCursed = playerEnergy != null && playerEnergy.IsEnergyModeActive;

        foreach (var hit in hits)
        {
            if (hit.transform.root == transform.root) continue;

            // Saltar dummies
            if (hit.GetComponentInParent<BlockingDummy>() != null) continue;

            if (hit.TryGetComponent<IDamageable>(out var damageable))
            {
                float damage = isCursed ? 10f : downslamDamage;
                Vector3 dir = (hit.transform.position - transform.position).normalized;
                Color? overrideColor = isCursed ? new Color(0.5f, 0.2f, 1f) : new Color(1f, 0.5f, 0f); 
                damageable.TakeDamage(damage, dir, overrideColor, false, isCursed);
                
                // Aplicar empuje (Knockback) si tiene motor de enemigo
                if (hit.TryGetComponent<VeinsOfMalice.AI.EnemyMotor>(out var motor))
                {
                    Debug.Log($"<color=cyan>[Combat]</color> Applying knockback to {hit.name}");
                    Vector3 knockbackForce = (dir + Vector3.up * 0.7f).normalized * 1000f; // Fuerza aumentada para masa 50
                    motor.ApplyKnockback(knockbackForce);
                }
                else if (hit.transform.parent != null && hit.transform.parent.TryGetComponent<VeinsOfMalice.AI.EnemyMotor>(out var parentMotor))
                {
                    Debug.Log($"<color=cyan>[Combat]</color> Applying knockback to parent {hit.transform.parent.name}");
                    Vector3 knockbackForce = (dir + Vector3.up * 0.7f).normalized * 1000f;
                    parentMotor.ApplyKnockback(knockbackForce);
                }
            }
        }
        
        // Visual feedback
        if (rend) StartCoroutine(DownslamFlash());
    }

    private IEnumerator DownslamFlash()
    {
        Color flashColor = new Color(1f, 0.8f, 0.2f);
        rend.material.color = flashColor;
        yield return new WaitForSeconds(0.15f);
        rend.material.color = isBlocking ? new Color(0.2f, 0.5f, 1f) : originalColor;
    }

    private IEnumerator PerformAttackDetection(bool isHeavy)
    {
        // Esperar un pequeño delay para que coincida con el "impacto" visual
        yield return new WaitForSeconds(attackDelay);

        float range = isHeavy ? heavyAttackRange : attackRange;
        Vector3 pos = attackPoint ? attackPoint.position : transform.position + transform.forward;
        Collider[] hits = Physics.OverlapSphere(pos, range, enemyLayer);

        float damage;
        if (isHeavy)
        {
            damage = heavyAttackDamage;
        }
        else
        {
            damage = currentComboStep switch
            {
                1 => comboStep1Damage,
                2 => comboStep2Damage,
                3 => comboStep3Damage,
                _ => 10f
            };
        }

        // Aplicar multiplicador si el modo Energía Maldita está activo
        bool isCursed = playerEnergy != null && playerEnergy.IsEnergyModeActive;
        if (isCursed)
        {
            float multiplier = isHeavy ? 1.75f : 1.25f;
            damage *= multiplier;
            Debug.Log($"<color=cyan>[ENERGY BOOST]</color> {(isHeavy ? "HEAVY " : "LIGHT ")}Damage boosted by {multiplier}x to {damage}!");
        }

        foreach (var hit in hits)
        {
            // Evitar que el jugador se golpee a sí mismo (revisando si el collider pertenece a nuestro propio objeto o hijos)
            if (hit.transform.root == transform.root) continue;

            if (hit.TryGetComponent<IDamageable>(out var damageable))
            {
                Vector3 dir = (hit.transform.position - transform.position).normalized;
                
                Color? overrideColor = isCursed ? new Color(0.5f, 0.2f, 1f) : null; // Púrpura azulado si está en modo energía
                damageable.TakeDamage(damage, dir, overrideColor, isHeavy, isCursed);
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

    // ── Upgrades ──────────────────────────────────────────────────────────────
    public void UpgradeBaseDamage(float extraDamage)
    {
        comboStep1Damage += extraDamage;
        comboStep2Damage += extraDamage;
        comboStep3Damage += extraDamage;
        heavyAttackDamage += (extraDamage * 1.5f);
        Debug.Log($"<color=green>[PlayerCombat]</color> Damage upgraded! Combo Step 1 is now {comboStep1Damage}");
    }
}
