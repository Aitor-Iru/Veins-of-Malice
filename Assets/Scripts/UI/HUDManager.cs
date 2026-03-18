using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using VeinsOfMalice.Player;

namespace VeinsOfMalice.UI
{
    /// <summary>
    /// Manages the main HUD elements: Health and Cursed Energy bars.
    /// </summary>
    public class HUDManager : MonoBehaviour
    {
        [Header("Player Settings")]
        [SerializeField] private PlayerHealth playerHealth;
        [SerializeField] private VeinsOfMalice.Player.PlayerEnergy playerEnergy;

        [Header("Health Bar")]
        [SerializeField] private Slider healthSlider;
        [SerializeField] private Slider healthGhostSlider; // For that "delayed" damage effect
        [SerializeField] private float ghostLerpSpeed = 2f;
        [SerializeField] private TMPro.TextMeshProUGUI healthText;

        [Header("Energy Bar")]
        [SerializeField] private Slider energySlider;
        [SerializeField] private TMPro.TextMeshProUGUI energyText;

        [Header("Essence Counter")]
        [SerializeField] private TMPro.TextMeshProUGUI essenceText;
        [SerializeField] private PlayerInventory playerInventory;

        private void Start()
        {
            if (playerHealth == null)
                playerHealth = FindFirstObjectByType<PlayerHealth>();
            
            if (playerEnergy == null)
                playerEnergy = FindFirstObjectByType<VeinsOfMalice.Player.PlayerEnergy>();

            if (playerInventory == null)
                playerInventory = FindFirstObjectByType<PlayerInventory>();

            if (playerHealth != null)
            {
                playerHealth.OnHealthChanged += UpdateHealthHUD;
                UpdateHealthHUD(playerHealth.CurrentHealth, playerHealth.MaxHealth);
            }

            if (playerEnergy != null)
            {
                playerEnergy.OnEnergyChanged += UpdateEnergyHUD;
                UpdateEnergyHUD(playerEnergy.CurrentEnergyNormalized * 100f, 100f);
            }
            else
            {
                // Fallback initial values if no energy script found yet
                if (energySlider != null)
                {
                    energySlider.maxValue = 100f;
                    energySlider.value = 100f;
                }
            }

            if (playerInventory != null)
            {
                playerInventory.OnEssenceChanged += UpdateEssenceHUD;
                UpdateEssenceHUD(playerInventory.CursedEssenceCount);
            }
        }

        private void OnDestroy()
        {
            if (playerHealth != null)
                playerHealth.OnHealthChanged -= UpdateHealthHUD;
            
            if (playerEnergy != null)
                playerEnergy.OnEnergyChanged -= UpdateEnergyHUD;

            if (playerInventory != null)
                playerInventory.OnEssenceChanged -= UpdateEssenceHUD;
        }

        private void UpdateHealthHUD(float current, float max)
        {
            if (healthSlider != null)
            {
                healthSlider.maxValue = max;
                healthSlider.value = current;
                
                if (healthGhostSlider != null)
                {
                    healthGhostSlider.maxValue = max;
                    // Ghost slider catches up in Update
                }
            }

            if (healthText != null)
            {
                healthText.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
            }
        }

        private void Update()
        {
            // Smoothly lerp the ghost bar to catch up with actual health
            if (healthGhostSlider != null && healthSlider != null)
            {
                if (healthGhostSlider.value > healthSlider.value)
                {
                    healthGhostSlider.value = Mathf.MoveTowards(healthGhostSlider.value, healthSlider.value, ghostLerpSpeed * Time.deltaTime * max(10f, (healthGhostSlider.value - healthSlider.value) * 5f));
                }
                else
                {
                    healthGhostSlider.value = healthSlider.value;
                }
            }
        }

        private float max(float a, float b) => a > b ? a : b;

        private void UpdateEnergyHUD(float current, float max)
        {
            if (energySlider != null)
            {
                energySlider.maxValue = max;
                energySlider.value = current;
            }

            if (energyText != null)
            {
                energyText.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
            }
        }

        private void UpdateEssenceHUD(int count)
        {
            if (essenceText != null)
            {
                essenceText.text = count.ToString();
            }
        }
    }
}
