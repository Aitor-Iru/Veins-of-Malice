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
                if (playerExperience != null && playerExperience.CurrentLevel >= PlayerExperience.AbsoluteMaxLevel)
                {
                    xpNumbersText.text = "<color=#FFFFFF>GRADE UP</color>";
                }
                else if (playerExperience != null && playerExperience.CurrentLevel >= playerExperience.MaxLevel)
                {
                    xpNumbersText.text = "<color=#FFFFFF>MAX LVL</color>";
                }
                else
                {
                    xpNumbersText.text = $"<color=#FFFFFF>{currentXP} / {xpToNext}</color>";
                }
            }
        }

        private void UpdateLevel(int newLevel)
        {
            if (levelText != null)
            {
                levelText.text = $"<color=#FFFFFF>Level: {newLevel}</color>"; // Todo el texto en blanco
            }

            if (rebirthButton != null && playerExperience != null)
            {
                // Aparece en el nivel máximo de cada tramo (100, 200...)
                bool canRebirth = newLevel >= playerExperience.MaxLevel && playerExperience.MaxLevel < PlayerExperience.AbsoluteMaxLevel;
                bool canGradeUp = newLevel >= PlayerExperience.AbsoluteMaxLevel && playerExperience.CurrentGrade < PlayerGrade.SpecialGrade;
                
                rebirthButton.gameObject.SetActive(canRebirth || canGradeUp);
                
                var btnText = rebirthButton.GetComponentInChildren<TextMeshProUGUI>();
                if (btnText != null)
                {
                    btnText.text = canGradeUp ? "GRADE UP" : "REBIRTH";
                }
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
