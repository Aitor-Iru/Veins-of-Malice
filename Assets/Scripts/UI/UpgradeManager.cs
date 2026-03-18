using UnityEngine;
using TMPro;
using VeinsOfMalice.Player;

namespace VeinsOfMalice.UI
{
    public class UpgradeManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerInventory playerInventory;
        [SerializeField] private PlayerHealth playerHealth;
        [SerializeField] private PlayerCombat playerCombat;

        [Header("UI Text Updates")]
        public TextMeshProUGUI healthButtonText;
        public TextMeshProUGUI damageButtonText;

        [Header("Upgrade Values")]
        [SerializeField] private float healthUpgradeAmount = 5f;
        [SerializeField] private float damageUpgradeAmount = 2f;

        [Header("Cost Scaling")]
        public int currentHealthCost = 10;
        public int currentDamageCost = 10;
        public int costIncrement = 5; // Escala sumando +5 cada vez

        private void Start()
        {
            // Auto asignar si faltan
            if (playerInventory == null) playerInventory = FindFirstObjectByType<PlayerInventory>();
            if (playerHealth == null) playerHealth = FindFirstObjectByType<PlayerHealth>();
            if (playerCombat == null) playerCombat = FindFirstObjectByType<PlayerCombat>();

            UpdateUI();
        }

        public void BuyHealth()
        {
            if (playerInventory != null && playerInventory.TrySpendEssence(currentHealthCost))
            {
                playerHealth.UpgradeMaxHealth(healthUpgradeAmount);
                currentHealthCost += costIncrement;
                UpdateUI();
                
                Debug.Log($"<color=green>[Upgrades]</color> Vida mejorada! Siguiente coste: {currentHealthCost}");
            }
            else
            {
                Debug.LogWarning("[Upgrades] No hay esencia suficiente para la Vida.");
            }
        }

        public void BuyDamage()
        {
            if (playerInventory != null && playerInventory.TrySpendEssence(currentDamageCost))
            {
                playerCombat.UpgradeBaseDamage(damageUpgradeAmount);
                currentDamageCost += costIncrement;
                UpdateUI();

                Debug.Log($"<color=green>[Upgrades]</color> Daño mejorado! Siguiente coste: {currentDamageCost}");
            }
            else
            {
                Debug.LogWarning("[Upgrades] No hay esencia suficiente para el Daño.");
            }
        }

        private void UpdateUI()
        {
            if (healthButtonText != null)
                healthButtonText.text = $"MEJORAR VIDA (+{healthUpgradeAmount})\nCoste: {currentHealthCost}";

            if (damageButtonText != null)
                damageButtonText.text = $"MEJORAR DAÑO (+{damageUpgradeAmount})\nCoste: {currentDamageCost}";
        }
    }
}
