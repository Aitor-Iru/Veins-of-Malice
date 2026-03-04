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

        private bool isOpen = false;

        private void Start()
        {
            if (playerInventory == null)
                playerInventory = FindFirstObjectByType<PlayerInventory>();

            if (inventoryPanel != null)
                inventoryPanel.SetActive(false);
            
            UpdateUI();
        }

        private void OnEnable()
        {
            if (inputReader != null)
                inputReader.OnInventoryToggleStarted += ToggleInventory;
            
            if (playerInventory != null)
                playerInventory.OnEssenceChanged += HandleEssenceChanged;
        }

        private void OnDisable()
        {
            if (inputReader != null)
                inputReader.OnInventoryToggleStarted -= ToggleInventory;
            
            if (playerInventory != null)
                playerInventory.OnEssenceChanged -= HandleEssenceChanged;
        }

        public void ToggleInventory()
        {
            isOpen = !isOpen;
            if (inventoryPanel != null)
                inventoryPanel.SetActive(isOpen);
            
            if (isOpen)
            {
                UpdateUI();
                // Opcional: Pausar el juego o cambiar el modo de input
                // inputReader.EnableUIInput();
                Debug.Log("<color=cyan>[Inventory]</color> Panel Opened");
            }
            else
            {
                // inputReader.EnableGameplayInput();
                Debug.Log("<color=cyan>[Inventory]</color> Panel Closed");
            }
        }

        private void HandleEssenceChanged(int count)
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (essenceText != null && playerInventory != null)
            {
                essenceText.text = playerInventory.CursedEssenceCount.ToString();
            }
        }
    }
}
