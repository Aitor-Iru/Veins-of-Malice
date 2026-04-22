using UnityEngine;
using System;

namespace VeinsOfMalice.AI
{
    /// <summary>
    /// Handles health for AI entities. Triggers death events.
    /// </summary>
    public class EnemyHealth : MonoBehaviour, IDamageable
    {
        [Header("Settings")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float currentHealth;
        [SerializeField] private Color hitColor = Color.red;
        [SerializeField] private float flashDuration = 0.1f;
        [SerializeField] private int xpReward = 20;
        
        private Renderer rend;
        private Color originalColor;
        public event Action OnDeath;
        private bool isDead = false;

        private void Awake()
        {
            currentHealth = maxHealth;
            rend = GetComponentInChildren<Renderer>();
            if (rend) originalColor = rend.material.color;
        }

        public void TakeDamage(float amount, Vector3 hitDirection)
        {
            if (isDead) return;

            currentHealth -= amount;
            Debug.Log($"<color=orange>[EnemyHealth]</color> {gameObject.name} took {amount} damage. HP: {currentHealth}");

            if (rend) StopAllCoroutines();
            if (rend) StartCoroutine(FlashHit());

            // Spawn Damage Number
            if (VeinsOfMalice.UI.DamageNumberManager.Instance != null)
            {
                VeinsOfMalice.UI.DamageNumberManager.Instance.SpawnDamageNumber(transform.position, amount, Color.red);
            }

            // Camera Shake on hit
            CameraController cam = Camera.main != null ? Camera.main.GetComponent<CameraController>() : null;
            if (cam != null) cam.Shake();

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private System.Collections.IEnumerator FlashHit()
        {
            rend.material.color = hitColor;
            yield return new WaitForSeconds(flashDuration);
            rend.material.color = originalColor;
        }

        private void Die()
        {
            isDead = true;
            OnDeath?.Invoke();

            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                var xpSys = player.GetComponentInChildren<VeinsOfMalice.Player.PlayerExperience>();
                if (xpSys != null) xpSys.AddXP(xpReward);
            }
        }
    }
}
