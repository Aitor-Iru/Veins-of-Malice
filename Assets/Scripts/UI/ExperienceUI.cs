using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VeinsOfMalice.Player;

namespace VeinsOfMalice.UI
{
    /// <summary>
    /// ExperienceUI — Controla la interfaz de la barra flotante/menú de pausa para la experiencia.
    /// </summary>
    public class ExperienceUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerExperience playerExperience;
        
        [Header("UI Elements")]
        [SerializeField] private Slider xpSlider;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI xpNumbersText;
        [SerializeField] private UnityEngine.UI.Button rebirthButton;

        private void Start()
        {
            if (playerExperience == null)
            {
                Debug.LogWarning("<color=orange>[ExperienceUI]</color> No PlayerExperience component found in scene.");
            }
        }

        private void OnEnable()
        {
            if (playerExperience == null)
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null) playerExperience = player.GetComponentInChildren<PlayerExperience>();
                
                if (playerExperience == null)
                    playerExperience = FindFirstObjectByType<PlayerExperience>();
            }

            if (playerExperience != null)
            {
                SubscribeEvents();
                // Update UI visually every time the panel is opened
                UpdateXP(playerExperience.CurrentXP, playerExperience.XPPerLevel);
                UpdateLevel(playerExperience.CurrentLevel);
            }
        }

        private void OnDisable()
        {
            if (playerExperience != null) UnsubscribeEvents();
        }

        private void SubscribeEvents()
        {
            UnsubscribeEvents(); // Prevent double calls
            playerExperience.OnXPChanged += UpdateXP;
            playerExperience.OnLevelChanged += UpdateLevel;
        }

        private void UnsubscribeEvents()
        {
            playerExperience.OnXPChanged -= UpdateXP;
            playerExperience.OnLevelChanged -= UpdateLevel;
        }

        private void UpdateXP(int currentXP, int xpToNext)
        {
            if (xpSlider != null)
            {
                xpSlider.maxValue = xpToNext;
                xpSlider.value = currentXP;
            }

            if (xpNumbersText != null)
            {
                xpNumbersText.text = $"{currentXP} / {xpToNext}";
            }
        }

        private void UpdateLevel(int newLevel)
        {
            if (levelText != null)
            {
                levelText.text = $"Level: {newLevel}";
            }

            if (rebirthButton != null && playerExperience != null)
            {
                bool canRebirth = newLevel >= playerExperience.MaxLevel && playerExperience.MaxLevel < VeinsOfMalice.Player.PlayerExperience.AbsoluteMaxLevel;
                rebirthButton.gameObject.SetActive(canRebirth);
            }
        }

        public void OnRebirthClicked()
        {
            if (playerExperience != null)
            {
                if (playerExperience.TryRebirth())
                {
                    // Update visuals explicitly just in case
                    UpdateXP(playerExperience.CurrentXP, playerExperience.XPPerLevel);
                    UpdateLevel(playerExperience.CurrentLevel);
                }
            }
        }
    }
}
