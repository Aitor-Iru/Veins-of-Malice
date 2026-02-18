using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float invulnerabilityDuration = 0.5f;

    public float MaxHealth => maxHealth;
    public float CurrentHealth { get; private set; }
    public bool IsInvulnerable { get; private set; }

    public event Action<float, float> OnHealthChanged; // current, max
    public event Action OnDeath;

    private float invulnerabilityTimer;

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    private void Update()
    {
        if (IsInvulnerable)
        {
            invulnerabilityTimer -= Time.deltaTime;
            if (invulnerabilityTimer <= 0f)
                IsInvulnerable = false;
        }
    }

    public void TakeDamage(float amount)
    {
        if (IsInvulnerable || amount <= 0f) return;

        CurrentHealth = Mathf.Max(0f, CurrentHealth - amount);
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);

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

    private void Die()
    {
        Debug.Log("Player died.");
        OnDeath?.Invoke();

        // Notify GameManager
        if (GameManager.Instance != null)
            GameManager.Instance.ChangeState(GameManager.GameState.GameOver);
    }
}
