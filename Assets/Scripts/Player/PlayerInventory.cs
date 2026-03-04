using UnityEngine;
using System;

namespace VeinsOfMalice.Player
{
    /// <summary>
    /// PlayerInventory — Gestiona los recursos recolectados por el jugador, como la Esencia Maldita.
    /// </summary>
    public class PlayerInventory : MonoBehaviour
    {
        [Header("Currency")]
        [SerializeField] private int cursedEssenceCount = 0;

        // ── Events ────────────────────────────────────────────────────────────────
        public event Action<int> OnEssenceChanged;

        // ── Public API ────────────────────────────────────────────────────────────
        public int CursedEssenceCount => cursedEssenceCount;

        /// <summary>
        /// Añade una cantidad de esencia al inventario.
        /// </summary>
        public void AddEssence(int amount)
        {
            cursedEssenceCount += amount;
            Debug.Log($"<color=cyan>[Inventory]</color> Collected Essence! Total: {cursedEssenceCount}");
            OnEssenceChanged?.Invoke(cursedEssenceCount);
        }

        /// <summary>
        /// Intenta gastar una cantidad de esencia. Retorna true si fue posible.
        /// </summary>
        public bool TrySpendEssence(int amount)
        {
            if (cursedEssenceCount >= amount)
            {
                cursedEssenceCount -= amount;
                OnEssenceChanged?.Invoke(cursedEssenceCount);
                return true;
            }
            return false;
        }
    }
}
