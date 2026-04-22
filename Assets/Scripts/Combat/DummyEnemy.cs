using UnityEngine;

/// <summary>
/// DummyEnemy — Un enemigo simple que recibe daño y registra los impactos en la consola.
/// </summary>
public class DummyEnemy : MonoBehaviour, IDamageable
{
    [SerializeField] private float health = 1000f;
    [SerializeField] private Color hitColor = Color.red;
    [SerializeField] private float colorDuration = 0.1f;

    private Renderer rend;
    private Color originalColor;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        if (rend) originalColor = rend.material.color;
    }

    public void TakeDamage(float amount, Vector3 hitDirection)
    {
        health -= amount;
        Debug.Log($"[DummyEnemy] Hit! Damage: {amount}. Health remaining: {health}");
        
        if (VeinsOfMalice.UI.DamageNumberManager.Instance != null)
        {
            VeinsOfMalice.UI.DamageNumberManager.Instance.SpawnDamageNumber(transform.position, amount, Color.red);
        }

        if (rend) StartCoroutine(FlashColor());
    }

    private System.Collections.IEnumerator FlashColor()
    {
        rend.material.color = hitColor;
        yield return new WaitForSeconds(colorDuration);
        rend.material.color = originalColor;
    }
}
