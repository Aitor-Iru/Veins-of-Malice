using UnityEngine;
using System;

namespace VeinsOfMalice.Player
{
    [System.Serializable]
    public class InventoryItem
    {
        public World.ItemData data;
        public int quantity;

        public InventoryItem(World.ItemData data, int quantity)
        {
            this.data = data;
            this.quantity = quantity;
        }
    }

    /// <summary>
    /// PlayerInventory — Gestiona los recursos recolectados por el jugador, como la Esencia Maldita.
    /// </summary>
    public class PlayerInventory : MonoBehaviour
    {
        [Header("Currency")]
        [SerializeField] private int cursedEssenceCount = 0;

        [Header("Inventory")]
        [SerializeField] private int maxSlots = 24;
        [SerializeField] private System.Collections.Generic.List<InventoryItem> items = new System.Collections.Generic.List<InventoryItem>();

        // ── Events ────────────────────────────────────────────────────────────────
        public event Action<int> OnEssenceChanged;
        public event Action OnInventoryChanged;

        // ── Public API ────────────────────────────────────────────────────────────
        public int CursedEssenceCount => cursedEssenceCount;
        public System.Collections.Generic.List<InventoryItem> Items => items;
        public int MaxSlots => maxSlots;

        /// <summary>
        /// Añade un ítem al inventario. Retorna true si hubo espacio.
        /// </summary>
        public bool AddItem(World.ItemData item, int amount = 1)
        {
            Debug.Log($"<color=white>[Inventory]</color> (ID: {GetInstanceID()}) AddItem requested for: {(item != null ? item.itemName : "NULL ITEM")} x{amount}");

            if (item == null)
            {
                Debug.LogError("<color=red>[Inventory]</color> CANNOT ADD NULL ITEM!");
                return false;
            }

            if (item.isStackable)
            {
                var existingItem = items.Find(i => i.data == item && i.quantity < item.maxStack);
                if (existingItem != null)
                {
                    int spaceLeft = item.maxStack - existingItem.quantity;
                    int toAdd = Math.Min(spaceLeft, amount);
                    existingItem.quantity += toAdd;
                    
                    Debug.Log($"<color=cyan>[Inventory]</color> (ID: {GetInstanceID()}) SUCCESS. Stacked: {item.itemName}. New quantity: {existingItem.quantity}");
                    
                    int remaining = amount - toAdd;
                    if (remaining > 0)
                    {
                        return AddItem(item, remaining);
                    }
                    else
                    {
                        OnInventoryChanged?.Invoke();
                        return true;
                    }
                }
            }

            if (items.Count >= maxSlots)
            {
                Debug.LogWarning($"<color=orange>[Inventory]</color> No more space ({items.Count}/{maxSlots})!");
                return false;
            }

            int addQuantity = item.isStackable ? Math.Min(amount, item.maxStack) : 1;
            items.Add(new InventoryItem(item, addQuantity));
            Debug.Log($"<color=cyan>[Inventory]</color> (ID: {GetInstanceID()}) SUCCESS. Added new slot: {item.itemName} x{addQuantity}. Current slot count: {items.Count}");
            
            int leftOver = amount - addQuantity;
            if (leftOver > 0)
            {
                return AddItem(item, leftOver);
            }
            
            OnInventoryChanged?.Invoke();
            return true;
        }

        /// <summary>
        /// Elimina un ítem del inventario.
        /// </summary>
        public void RemoveItem(World.ItemData item, int amount = 1)
        {
            var existingItem = items.Find(i => i.data == item);
            if (existingItem != null)
            {
                existingItem.quantity -= amount;
                if (existingItem.quantity <= 0)
                {
                    items.Remove(existingItem);
                }
                OnInventoryChanged?.Invoke();
            }
        }

        /// <summary>
        /// Añade una cantidad de esencia al inventario. Además otorga experiencia proporcional.
        /// </summary>
        public void AddEssence(int amount)
        {
            cursedEssenceCount += amount;
            Debug.Log($"<color=cyan>[Inventory]</color> Collected Essence! Total: {cursedEssenceCount}");
            OnEssenceChanged?.Invoke(cursedEssenceCount);

            // Añadir XP (reducida, ej. 2 XP por cada punto de esencia recogido, 
            // ten en cuenta que el usuario sube de nivel con 100 XP)
            PlayerExperience playerXP = FindFirstObjectByType<PlayerExperience>();
            if (playerXP != null)
            {
                int xpReward = amount * 2; // Relación Configurable (2 de XP por esencia)
                playerXP.AddXP(xpReward);
            }
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
