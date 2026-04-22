using System.Collections;
using UnityEngine;

/// <summary>
/// AttackingDummy — Un dummy que ataca al jugador periódicamente para probar el bloqueo.
/// </summary>
public class AttackingDummy : MonoBehaviour, IDamageable
{
    [Header("Health")]
    [SerializeField] private float health = 1000f;
    [SerializeField] private Color hitColor = Color.red;

    [Header("Attack Settings")]
    [SerializeField] private float attackInterval = 3f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private Color telegraphColor = Color.yellow;
    [SerializeField] private float telegraphDuration = 0.5f;

    private Renderer rend;
    private Color originalColor;
    private float nextAttackTime;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        if (rend) originalColor = rend.material.color;
        nextAttackTime = Time.time + attackInterval;
    }

    private void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            StartCoroutine(AttackRoutine());
            nextAttackTime = Time.time + attackInterval;
        }
    }

    private IEnumerator AttackRoutine()
    {
        // 1. Telégrafo (Aviso)
        Debug.Log("[AttackingDummy] Preparing attack...");
        if (rend) rend.material.color = telegraphColor;
        yield return new WaitForSeconds(telegraphDuration);

        // 2. Ataque
        Debug.Log("[AttackingDummy] ATTACK!");
        if (rend) rend.material.color = Color.white; // Flash de ataque

        // Buscar al jugador
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player") && hit.TryGetComponent<IDamageable>(out var damageable))
            {
                Vector3 dir = (hit.transform.position - transform.position).normalized;
                damageable.TakeDamage(damage, dir);
            }
        }

        yield return new WaitForSeconds(0.1f);
        if (rend) rend.material.color = originalColor;
    }

    public void TakeDamage(float amount, Vector3 hitDirection)
    {
        health -= amount;
        Debug.Log($"[AttackingDummy] Ouch! Received {amount} damage. HP: {health}");
        
        if (VeinsOfMalice.UI.DamageNumberManager.Instance != null)
        {
            VeinsOfMalice.UI.DamageNumberManager.Instance.SpawnDamageNumber(transform.position, amount, Color.red);
        }

        StartCoroutine(FlashColor(hitColor));
    }

    private IEnumerator FlashColor(Color color)
    {
        if (!rend) yield break;
        Color prev = rend.material.color;
        rend.material.color = color;
        yield return new WaitForSeconds(0.1f);
        rend.material.color = prev;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
