using System;
using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float invulnerabilityDuration = 0.5f;
    [SerializeField] private float blockDamageReduction = 0.7f; // 70% reducción

    [Header("Regen Settings")]
    [SerializeField] private float regenDelay = 15f; // Tiempo sin daño para empezar a curar
    [SerializeField] private float regenRate = 5f;   // Cuánta vida se recupera por segundo


    public float MaxHealth => maxHealth;
    public float CurrentHealth { get; private set; }
    public bool IsInvulnerable { get; private set; }

    public event Action<float, float> OnHealthChanged; // current, max
    public event Action OnDeath;

    private float invulnerabilityTimer;
    private PlayerCombat playerCombat;
    private VeinsOfMalice.Player.PlayerEnergy playerEnergy;
    private PlayerController playerController;
    private CameraController camController;
    private Renderer rend;
    private Color originalColor;
    
    private float lastDamageTime;


    private void Awake()
    {
        CurrentHealth = maxHealth;
        playerCombat = GetComponent<PlayerCombat>();
        playerEnergy = GetComponent<VeinsOfMalice.Player.PlayerEnergy>();
        playerController = GetComponent<PlayerController>();
        camController = Camera.main != null ? Camera.main.GetComponent<CameraController>() : null;
        rend = GetComponentInChildren<Renderer>();
        if (rend) originalColor = rend.material.color;
    }

    private void Update()
    {
        if (IsInvulnerable)
        {
            invulnerabilityTimer -= Time.deltaTime;
            if (invulnerabilityTimer <= 0f)
                IsInvulnerable = false;
        }

        // Regeneración automática (fuera de combate)
        if (CurrentHealth > 0f && CurrentHealth < maxHealth)
        {
            if (Time.time - lastDamageTime >= regenDelay)
            {
                Heal(regenRate * Time.deltaTime);
            }
        }
    }

    public void TakeDamage(float amount, Vector3 hitDirection, Color? overrideColor = null, bool isHeavy = false, bool isCursed = false)
    {
        if (IsInvulnerable || amount <= 0f) return;

        bool isPerfectBlock = false;
        bool shieldBroken = false;

        // Reducir daño si bloquea
        if (playerCombat != null && playerCombat.IsBlocking)
        {
            if (isHeavy)
            {
                // Escudo Roto por golpe pesado
                playerCombat.BreakShield();
                shieldBroken = true;
                amount = 0f; // El golpe que rompe el escudo no hace daño a la vida (según propuesta aceptada)
                
                if (isCursed && playerController != null)
                {
                    playerController.Freeze(2.0f);
                    Debug.Log("<color=blue><b>[FROZEN]</b></color> Target frozen for 2.0s!");
                }
            }
            else if (playerEnergy != null && playerEnergy.IsEnergyModeActive)
            {
                // Bloqueo Perfecto con Energía Maldita
                amount = 0f;
                playerEnergy.UseEnergy(playerEnergy.MaxEnergy * 0.15f);
                isPerfectBlock = true;
                Debug.Log("<color=black><b>[PERFECT BLOCK]</b></color> Damage nullified by Cursed Energy!");
            }
            else
            {
                amount *= (1f - blockDamageReduction);
                Debug.Log($"[PlayerHealth] Damage blocked! Reduced to {amount}");
            }
        }

        CurrentHealth = Mathf.Max(0f, CurrentHealth - amount);
        lastDamageTime = Time.time; // Reiniciar el temporizador de regeneración
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);

        // Feedback de daño
        if (!shieldBroken)
            Debug.Log($"<color=red><b>[PLAYER HIT]</b></color> Damage: {amount} | Health: {CurrentHealth}");
        
        if (camController != null) camController.Shake();

        // Spawn Damage Number
        if (VeinsOfMalice.UI.DamageNumberManager.Instance != null)
        {
            if (shieldBroken)
            {
                VeinsOfMalice.UI.DamageNumberManager.Instance.SpawnDamageNumber(transform.position, isCursed ? "FROZEN!" : "SHIELD BREAK!", Color.yellow);
            }
            else if (isPerfectBlock)
            {
                VeinsOfMalice.UI.DamageNumberManager.Instance.SpawnDamageNumber(transform.position, "PERFECT BLOCK", Color.black);
            }
            else
            {
                Color damageColor;
                if (overrideColor.HasValue) damageColor = overrideColor.Value;
                else damageColor = (playerCombat != null && playerCombat.IsBlocking) ? new Color(0.4f, 0.9f, 1f) : Color.red; // Celeste si bloquea, Rojo si no
                
                VeinsOfMalice.UI.DamageNumberManager.Instance.SpawnDamageNumber(transform.position, amount, damageColor);
            }
        }
        
        Color flashColor = (playerCombat != null && playerCombat.IsBlocking) ? new Color(1f, 0.5f, 0f) : Color.red; // Naranja si bloquea, Rojo si no
        if (rend) StartCoroutine(DamageColorFlash(flashColor));

        // Trigger invulnerability frames
        IsInvulnerable = true;
        invulnerabilityTimer = invulnerabilityDuration;

        if (CurrentHealth <= 0f)
        {
            Die();
        }
    }




    public void Heal(float amount)
    {
        if (amount <= 0f) return;
        CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }

    private IEnumerator DamageColorFlash(Color color)
    {
        if (!rend) yield break;
        rend.material.color = color;
        yield return new WaitForSeconds(invulnerabilityDuration);
        
        // Al terminar el flash, volvemos al color que corresponda según el estado actual
        if (playerCombat != null && playerCombat.IsBlocking)
            rend.material.color = new Color(0.2f, 0.5f, 1f); // Azul de bloqueo
        else
            rend.material.color = originalColor;
    }

    private void Die()
    {
        Debug.Log("Player died.");
        OnDeath?.Invoke();

        // Notify GameManager
        if (GameManager.Instance != null)
            GameManager.Instance.ChangeState(GameManager.GameState.GameOver);
    }

    // ── Upgrades ──────────────────────────────────────────────────────────────
    public void UpgradeMaxHealth(float extraHealth)
    {
        maxHealth += extraHealth;
        CurrentHealth += extraHealth;
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
        Debug.Log($"<color=green>[PlayerHealth]</color> Max health upgraded to {maxHealth}!");
    }
}
