using UnityEngine;
using System.Collections;

/// <summary>
/// BlockingDummy — Un dummy que siempre bloquea para probar mecánicas de Shield Break y Congelación.
/// </summary>
public class BlockingDummy : MonoBehaviour, IDamageable
{
    [Header("Settings")]
    [SerializeField] private float health = 1000f;
    [SerializeField] private Color blockColor = new Color(0.2f, 0.5f, 1f);
    [SerializeField] private Color shieldBreakColor = Color.yellow;
    [SerializeField] private Color frozenColor = Color.cyan;
    
    private Renderer rend;
    private Color originalColor;
    private bool isBlocking = true;
    private bool isFrozen = false;
    private bool shieldBreakRoutineActive = false;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        if (rend) 
        {
            originalColor = rend.material.color;
            rend.material.color = blockColor;
        }
    }

    public void TakeDamage(float amount, Vector3 hitDirection, Color? overrideColor = null, bool isHeavy = false, bool isCursed = false)
    {

        if (isBlocking)
        {
            if (isHeavy)
            {
                // Shield Break
                isBlocking = false;
                if (isCursed)
                {
                    StopAllCoroutines();
                    StartCoroutine(FreezeRoutine());
                }
                else
                {
                    StopAllCoroutines();
                    StartCoroutine(ShieldBreakRoutine());
                }
                
                if (VeinsOfMalice.UI.DamageNumberManager.Instance != null)
                {
                    VeinsOfMalice.UI.DamageNumberManager.Instance.SpawnDamageNumber(transform.position, isCursed ? "FROZEN!" : "SHIELD BREAK!", Color.yellow);
                }
                return; // 0 damage on shield break hit
            }
            else
            {
                // Normal damage reduction when blocking
                amount *= 0.3f; // 70% reduction
                if (VeinsOfMalice.UI.DamageNumberManager.Instance != null)
                {
                    VeinsOfMalice.UI.DamageNumberManager.Instance.SpawnDamageNumber(transform.position, amount, new Color(0.4f, 0.9f, 1f));
                }
            }
        }
        else
        {
            // Dummy is vulnerable
            if (VeinsOfMalice.UI.DamageNumberManager.Instance != null)
            {
                Color damageColor = overrideColor ?? Color.red;
                VeinsOfMalice.UI.DamageNumberManager.Instance.SpawnDamageNumber(transform.position, amount, damageColor);
            }
        }

        health -= amount;
        Debug.Log($"[BlockingDummy] Hit! Damage: {amount}. Health remaining: {health}");
        
        if (rend && !isFrozen && !shieldBreakRoutineActive) 
            StartCoroutine(FlashHit());
    }

    private IEnumerator ShieldBreakRoutine()
    {
        shieldBreakRoutineActive = true;
        rend.material.color = shieldBreakColor;
        yield return new WaitForSeconds(2.0f); // Se queda vulnerable por 2 segundos
        rend.material.color = blockColor;
        isBlocking = true;
        shieldBreakRoutineActive = false;
    }

    private IEnumerator FreezeRoutine()
    {
        isFrozen = true;
        rend.material.color = frozenColor;
        yield return new WaitForSeconds(2.0f); // Tiempo solicitado
        rend.material.color = blockColor;
        isBlocking = true;
        isFrozen = false;
    }

    private IEnumerator FlashHit()
    {
        Color current = rend.material.color;
        rend.material.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        rend.material.color = current;
    }
}
