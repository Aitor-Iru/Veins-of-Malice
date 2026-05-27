using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace VeinsOfMalice.UI
{
    /// <summary>
    /// SkillNodeUI — Controla la visualización y selección de un nodo individual en el árbol de habilidades.
    /// </summary>
    public class SkillNodeUI : MonoBehaviour
    {
        [Header("UI Component References")]
        [SerializeField] private Image outlineImage;
        [SerializeField] private Image fillImage;
        [SerializeField] private Image lockIcon;
        [SerializeField] private TextMeshProUGUI costText;
        [SerializeField] private GameObject selectionHighlight;

        [Header("Node Configuration")]
        [SerializeField] private string nodeName = "Skill";
        [SerializeField] private bool isLocked = true;
        [SerializeField] private int currentLevel = 0;
        [SerializeField] private int maxLevel = 3;
        [SerializeField] private string costValueString = "3";
        [SerializeField] private Color activeColor = Color.green;
        [SerializeField] private Color lockedColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);

        // Referencias a los nodos hijos y padres
        public SkillNodeUI[] parentNodes;
        public SkillNodeUI[] childNodes;

        private Button button;
        private SkillTreeUI skillTreeManager;

        public string NodeName => nodeName;
        public bool IsLocked => isLocked;
        public int CurrentLevel => currentLevel;
        public int MaxLevel => maxLevel;
        public Color ActiveColor => activeColor;

        private void Awake()
        {
            button = GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(OnNodeClicked);
            }
            
            skillTreeManager = GetComponentInParent<SkillTreeUI>();
        }

        private void Start()
        {
            UpdateVisuals();
        }

        /// <summary>
        /// Actualiza el estado visual del nodo según sus valores actuales.
        /// </summary>
        public void UpdateVisuals()
        {
            if (costText != null)
            {
                costText.text = costValueString;
            }

            if (isLocked)
            {
                if (lockIcon != null) lockIcon.gameObject.SetActive(true);
                if (fillImage != null) fillImage.fillAmount = 0f;
                if (outlineImage != null) outlineImage.color = lockedColor;
            }
            else
            {
                if (lockIcon != null) lockIcon.gameObject.SetActive(false);
                if (outlineImage != null) outlineImage.color = activeColor;

                if (fillImage != null)
                {
                    if (maxLevel > 0)
                    {
                        fillImage.fillAmount = (float)currentLevel / maxLevel;
                    }
                    else
                    {
                        fillImage.fillAmount = 1f; // Si no tiene niveles (ej. habilidad especial de 1 nivel)
                    }
                }
            }
        }

        /// <summary>
        /// Establece el resaltado de selección.
        /// </summary>
        public void SetSelected(bool selected)
        {
            if (selectionHighlight != null)
            {
                selectionHighlight.SetActive(selected);
            }
        }

        private void OnNodeClicked()
        {
            if (skillTreeManager != null)
            {
                skillTreeManager.SelectNode(this);
            }
        }

        /// <summary>
        /// Fuerza el estado de bloqueo/desbloqueo del nodo (útil para pruebas visuales en el editor o en runtime).
        /// </summary>
        public void SetState(bool locked, int level)
        {
            isLocked = locked;
            currentLevel = Mathf.Clamp(level, 0, maxLevel);
            UpdateVisuals();
        }
    }
}
