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
                
                // CRITICAL: Check for duplicates
                PlayerInventory[] allInvs = FindObjectsByType<PlayerInventory>(FindObjectsSortMode.None);
                if (allInvs.Length > 1)
                {
                    Debug.LogError($"<color=red>[InventoryUI]</color> DETECTED {allInvs.Length} INVENTORIES! IDs:");
                    foreach(var inv in allInvs) Debug.LogError($" - {inv.gameObject.name} (ID: {inv.GetInstanceID()})");
                }
            }

            if (playerInventory != null)
            {
                Debug.Log($"<color=green>[InventoryUI]</color> Linked to PlayerInventory on {playerInventory.gameObject.name} (ID: {playerInventory.GetInstanceID()})");
                SubscribeToInventory();
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
                Debug.Log("<color=green>[InventoryUI]</color> Subscribed to InputReader");
            }
            
            if (playerInventory != null)
            {
                SubscribeToInventory();
            }
        }

        private void SubscribeToInventory()
        {
            // Unsubscribe first to avoid double subscription
            playerInventory.OnEssenceChanged -= HandleEssenceChanged;
            playerInventory.OnInventoryChanged -= HandleInventoryChanged;

            playerInventory.OnEssenceChanged += HandleEssenceChanged;
            playerInventory.OnInventoryChanged += HandleInventoryChanged;
            Debug.Log("<color=green>[InventoryUI]</color> Events Subscribed");
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
            
            Debug.Log($"<color=cyan>[InventoryUI]</color> (ID: {GetInstanceID()}) Toggle: {(isOpen ? "OPEN" : "CLOSED")}");

            if (isOpen)
            {
                UpdateUI();
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
            if (playerInventory == null)
            {
                Debug.LogError($"<color=red>[InventoryUI]</color> (ID: {GetInstanceID()}) CANNOT UPDATE: PlayerInventory is NULL!");
                return;
            }

            Debug.Log($"<color=green>[InventoryUI]</color> (ID: {GetInstanceID()}) Updating from Inv ID: {playerInventory.GetInstanceID()}. Items: {playerInventory.Items.Count}");

            if (slots != null)
            {
                var invItems = playerInventory.Items;
                for (int i = 0; i < slots.Length; i++)
                {
                    if (i < invItems.Count)
                    {
                        var item = invItems[i];
                        if (item != null)
                            slots[i].SetItem(item);
                        else
                            slots[i].ClearSlot();
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
