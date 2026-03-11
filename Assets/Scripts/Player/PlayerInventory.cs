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

        [Header("Inventory")]
        [SerializeField] private int maxSlots = 24;
        [SerializeField] private System.Collections.Generic.List<World.ItemData> items = new System.Collections.Generic.List<World.ItemData>();

        // ── Events ────────────────────────────────────────────────────────────────
        public event Action<int> OnEssenceChanged;
        public event Action OnInventoryChanged;

        // ── Public API ────────────────────────────────────────────────────────────
        public int CursedEssenceCount => cursedEssenceCount;
        public System.Collections.Generic.List<World.ItemData> Items => items;
        public int MaxSlots => maxSlots;

        /// <summary>
        /// Añade un ítem al inventario. Retorna true si hubo espacio.
        /// </summary>
        public bool AddItem(World.ItemData item)
        {
            if (items.Count >= maxSlots)
            {
                Debug.LogWarning("<color=orange>[Inventory]</color> No more space!");
                return false;
            }

            items.Add(item);
            Debug.Log($"<color=cyan>[Inventory]</color> Added item: {item.itemName}");
            OnInventoryChanged?.Invoke();
            return true;
        }

        /// <summary>
        /// Elimina un ítem del inventario.
        /// </summary>
        public void RemoveItem(World.ItemData item)
        {
            if (items.Remove(item))
            {
                OnInventoryChanged?.Invoke();
            }
        }

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
