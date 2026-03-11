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
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null) playerInventory = player.GetComponentInParent<PlayerInventory>();
                
                if (playerInventory == null)
                    playerInventory = FindFirstObjectByType<PlayerInventory>();
            }

            if (playerInventory != null)
            {
                Debug.Log($"<color=green>[InventoryUI]</color> Linked to PlayerInventory on {playerInventory.gameObject.name} (ID: {playerInventory.GetInstanceID()})");
            }
            else
            {
                Debug.LogError("<color=red>[InventoryUI]</color> FAILED to find any PlayerInventory in scene!");
            }
            if (inputReader == null)
            {
                Debug.LogWarning("<color=orange>[InventoryUI]</color> InputReader not assigned! Seeking in project or Resources...");
                inputReader = Resources.Load<InputReader>("InputReader");
            }

            // Force refresh slots in Start
            slots = GetComponentsInChildren<InventorySlotUI>(true);
            Debug.Log($"<color=green>[InventoryUI]</color> Initialized with {slots.Length} slots.");
            
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
            if (playerInventory == null) return;

            Debug.Log($"<color=green>[InventoryUI]</color> Updating UI. Items in list: {playerInventory.Items.Count}");

            if (slots != null)
            {
                var invItems = playerInventory.Items;
                for (int i = 0; i < slots.Length; i++)
                {
                    if (i < invItems.Count)
                    {
                        slots[i].SetItem(invItems[i]);
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
