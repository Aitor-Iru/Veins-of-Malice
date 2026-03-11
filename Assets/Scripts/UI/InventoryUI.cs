using UnityEngine;
using TMPro;
using VeinsOfMalice.Player;

namespace VeinsOfMalice.UI
{
    /// <summary>
    /// InventoryUI — Controla la visibilidad del panel de inventario y muestra los recursos.
    /// Se activa/desactiva con la tecla configurada en InputReader (ej. 'B').
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private InputReader inputReader;
        [SerializeField] private PlayerInventory playerInventory;
        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] private TextMeshProUGUI essenceText;
        [SerializeField] private InventorySlotUI[] slots;

        private bool isOpen = false;

        private void Start()
        {
            Debug.Log("<color=green>[InventoryUI]</color> Script Started on: " + gameObject.name);
            
            if (playerInventory == null)
                playerInventory = FindFirstObjectByType<PlayerInventory>();

            if (inputReader == null)
            {
                Debug.LogWarning("<color=orange>[InventoryUI]</color> InputReader not assigned! Seeking in project or Resources...");
                inputReader = Resources.Load<InputReader>("InputReader");
            }

            // Find slots if not assigned
            if (slots == null || slots.Length == 0)
            {
                slots = GetComponentsInChildren<InventorySlotUI>(true);
                Debug.Log($"<color=green>[InventoryUI]</color> Found {slots.Length} slots in children.");
            }

            // Ensure panel starts hidden if assigned
            if (inventoryPanel != null)
                inventoryPanel.SetActive(false);
            
            UpdateUI();
        }

        private void OnEnable()
        {
            if (inputReader != null)
            {
                inputReader.OnInventoryToggleStarted += ToggleInventory;
                Debug.Log("<color=green>[InventoryUI]</color> Subscribed to OnInventoryToggleStarted");
            }
            else
            {
                Debug.LogError("<color=red>[InventoryUI]</color> InputReader is still NULL. Can't toggle inventory!");
            }
            
            if (playerInventory != null)
            {
                playerInventory.OnEssenceChanged += HandleEssenceChanged;
                playerInventory.OnInventoryChanged += HandleInventoryChanged;
            }
        }

        private void OnDisable()
        {
            if (inputReader != null)
                inputReader.OnInventoryToggleStarted -= ToggleInventory;
            
            if (playerInventory != null)
            {
                playerInventory.OnEssenceChanged -= HandleEssenceChanged;
                playerInventory.OnInventoryChanged -= HandleInventoryChanged;
            }
        }

        public void ToggleInventory()
        {
            isOpen = !isOpen;
            if (inventoryPanel != null)
                inventoryPanel.SetActive(isOpen);
            
            if (isOpen)
            {
                UpdateUI();
                Debug.Log("<color=cyan>[Inventory]</color> Panel Opened");
            }
            else
            {
                Debug.Log("<color=cyan>[Inventory]</color> Panel Closed");
            }
        }

        private void HandleEssenceChanged(int count)
        {
            UpdateUI();
        }

        private void HandleInventoryChanged()
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (essenceText != null && playerInventory != null)
            {
                essenceText.text = "CURSED ESSENCE: " + playerInventory.CursedEssenceCount.ToString();
            }

            if (playerInventory != null && slots != null)
            {
                var items = playerInventory.Items;
                for (int i = 0; i < slots.Length; i++)
                {
                    if (i < items.Count)
                    {
                        slots[i].SetItem(items[i]);
                    }
                    else
                    {
                        slots[i].ClearSlot();
                    }
                }
            }
        }
    }
}
