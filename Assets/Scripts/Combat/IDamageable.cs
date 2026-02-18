using UnityEngine;

/// <summary>
/// Interfaz para cualquier entidad que pueda recibir daño.
/// </summary>
public interface IDamageable
{
    /// <summary>
    /// Aplica daño a la entidad.
    /// </summary>
    /// <param name="amount">Cantidad de daño.</param>
    /// <param name="hitDirection">Dirección del impacto (para knockback).</param>
    void TakeDamage(float amount, Vector3 hitDirection);
}
