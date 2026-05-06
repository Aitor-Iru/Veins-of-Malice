using UnityEngine;
using TMPro;
using VeinsOfMalice.Player;

namespace VeinsOfMalice.UI
{
    public class GradeUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI gradeText;
        [SerializeField] private PlayerExperience playerXP;

        private void OnEnable()
        {
            if (playerXP == null)
                playerXP = FindFirstObjectByType<PlayerExperience>();

            if (playerXP != null)
            {
                playerXP.OnGradeChanged += UpdateGradeDisplay;
                UpdateGradeDisplay(playerXP.CurrentGrade);
            }
        }

        private void OnDisable()
        {
            if (playerXP != null)
            {
                playerXP.OnGradeChanged -= UpdateGradeDisplay;
            }
        }

        private void UpdateGradeDisplay(PlayerGrade grade)
        {
            if (gradeText != null && playerXP != null)
            {
                gradeText.text = playerXP.GetGradeName();
                
                // Color optional based on grade
                switch(grade)
                {
                    case PlayerGrade.SpecialGrade:
                        gradeText.color = new Color(1f, 0.2f, 0.2f); // Rojo brillante
                        break;
                    case PlayerGrade.Grade1:
                    case PlayerGrade.SemiGrade1:
                        gradeText.color = new Color(1f, 0.8f, 0.2f); // Dorado
                        break;
                    default:
                        gradeText.color = Color.white;
                        break;
                }
            }
        }
    }
}
