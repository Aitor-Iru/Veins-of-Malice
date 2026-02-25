using UnityEngine;
using System.Collections;

namespace VeinsOfMalice.AI
{
    /// <summary>
    /// Handles combat logic for AI entities.
    /// </summary>
    public class EnemyCombat : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float attackDamage = 10f;
        [SerializeField] private float attackRange = 1.5f;
        [SerializeField] private float attackCooldown = 2f;
        
        [Header("Visual Feedback")]
        [SerializeField] private Color attackColor = Color.white;
        [SerializeField] private float flashDuration = 0.1f;

        private Renderer rend;
        private Color originalColor;
        private float lastAttackTime;

        private void Awake()
        {
            rend = GetComponentInChildren<Renderer>();
            if (rend) originalColor = rend.material.color;
        }

        public bool CanAttack(Transform target)
        {
            if (target == null) return false;
            float distance = Vector3.Distance(transform.position, target.position);
            return distance <= attackRange && Time.time >= lastAttackTime + attackCooldown;
        }

        public void PerformAttack(Transform target)
        {
            lastAttackTime = Time.time;
            Debug.Log($"[AI] Attacking {target.name}");
            
            // Basic attack logic
            if (target.TryGetComponent<IDamageable>(out var damageable))
            {
                Vector3 dir = (target.position - transform.position).normalized;
                damageable.TakeDamage(attackDamage, dir);
            }
            
            if (rend) StartCoroutine(FlashAttack());
        }

        private IEnumerator FlashAttack()
        {
            rend.material.color = attackColor;
            yield return new WaitForSeconds(flashDuration);
            rend.material.color = originalColor;
        }

        public float GetAttackRange() => attackRange;
    }
}
