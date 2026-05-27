using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

namespace VeinsOfMalice.UI
{
    public class SkillTreeUI : MonoBehaviour
    {
        [Header("Tab Navigation")]
        [SerializeField] private Button[] tabButtons;
        [SerializeField] private GameObject[] tabContainers;
        [SerializeField] private Color activeTabColor = Color.white;
        [SerializeField] private Color inactiveTabColor = Color.gray;

        [Header("Stats Panel (Right)")]
        [SerializeField] private RectTransform strBarFill;
        [SerializeField] private RectTransform focBarFill;
        [SerializeField] private RectTransform tecBarFill;
        [SerializeField] private RectTransform hpBarFill;
        [SerializeField] private TextMeshProUGUI strValueText;
        [SerializeField] private TextMeshProUGUI focValueText;
        [SerializeField] private TextMeshProUGUI tecValueText;
        [SerializeField] private TextMeshProUGUI hpValueText;


        [Header("Control Panel (Bottom)")]
        [SerializeField] private TextMeshProUGUI statPointsText;
        [SerializeField] private Button refundButton;
        [SerializeField] private Button minusButton;
        [SerializeField] private Button plusButton;

        [Header("Detail Panel (Left)")]
        [SerializeField] private GameObject detailPanel;
        [SerializeField] private TextMeshProUGUI detailTitleText;
        [SerializeField] private TextMeshProUGUI detailLevelText;

        [Header("Line Connection Settings")]
        [SerializeField] private RectTransform linesContainer;
        [SerializeField] private GameObject linePrefab;
        [SerializeField] private float lineThickness = 6f;
        [SerializeField] private Color connectionLockedColor = new Color(0.15f, 0.15f, 0.15f, 0.8f);

        // Sistema de Rangos de Estadísticas (F -> SSS)
        private readonly string[] grades = { "F", "D", "C", "B", "A", "S", "SS", "SSS" };
        private int strGradeIndex = 0; // F al principio (vacío)
        private int focGradeIndex = 0; // F al principio (vacío)
        private int tecGradeIndex = 0; // F al principio (vacío)
        private int hpGradeIndex = 0;  // F al principio (vacío)

        private int statPoints = 0;

        private int activeTabIndex = 0;
        private SkillNodeUI selectedNode;
        private List<GameObject> activeLines = new List<GameObject>();

        private void Start()
        {
            // Inicializar las pestañas
            for (int i = 0; i < tabButtons.Length; i++)
            {
                int index = i;
                tabButtons[i].onClick.AddListener(() => SwitchTab(index));
            }

            // Inicializar botones de control
            if (refundButton != null) refundButton.onClick.AddListener(OnRefundClicked);
            if (minusButton != null) minusButton.onClick.AddListener(OnMinusClicked);
            if (plusButton != null) plusButton.onClick.AddListener(OnPlusClicked);

            SwitchTab(0);
            UpdateStatsUI();
            
            if (detailPanel != null)
            {
                detailPanel.SetActive(false);
            }
        }

        private void Update()
        {
            // Tecla Tab otorga un punto de mejora (Temporal)
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                statPoints++;
                UpdateStatsUI();
                Debug.Log($"<color=cyan>[SkillTree]</color> ¡Te has otorgado 1 Stat Point! Total disponible: {statPoints}");
            }

            // Para asegurar que las líneas se redibujen si cambia el layout
            if (activeLines.Count == 0 && tabContainers.Length > 0 && tabContainers[activeTabIndex] != null)
            {
                GenerateConnectionLines();
            }
        }

        public void SwitchTab(int tabIndex)
        {
            if (tabIndex < 0 || tabIndex >= tabContainers.Length) return;

            activeTabIndex = tabIndex;

            for (int i = 0; i < tabContainers.Length; i++)
            {
                if (tabContainers[i] != null)
                {
                    tabContainers[i].SetActive(i == tabIndex);
                }

                if (i < tabButtons.Length && tabButtons[i] != null)
                {
                    var text = tabButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                    if (text != null)
                    {
                        text.color = (i == tabIndex) ? activeTabColor : inactiveTabColor;
                    }
                    
                    var outline = tabButtons[i].GetComponent<Outline>();
                    if (outline != null) outline.enabled = (i == tabIndex);
                }
            }

            if (selectedNode != null)
            {
                selectedNode.SetSelected(false);
                selectedNode = null;
            }
            if (detailPanel != null) detailPanel.SetActive(false);

            GenerateConnectionLines();
        }

        public void SelectNode(SkillNodeUI node)
        {
            if (selectedNode != null)
            {
                selectedNode.SetSelected(false);
            }

            selectedNode = node;
            selectedNode.SetSelected(true);

            if (detailPanel != null)
            {
                detailPanel.SetActive(true);
                
                if (detailTitleText != null)
                {
                    detailTitleText.text = $"NODE_{node.NodeName.ToUpper()}";
                }
                
                if (detailLevelText != null)
                {
                    detailLevelText.text = $"LEVEL {node.CurrentLevel} / {node.MaxLevel}";
                }
            }
        }

        private void GenerateConnectionLines()
        {
            foreach (var line in activeLines)
            {
                if (line != null) Destroy(line);
            }
            activeLines.Clear();

            if (linesContainer == null) return;
            if (activeTabIndex >= tabContainers.Length || tabContainers[activeTabIndex] == null) return;

            SkillNodeUI[] nodes = tabContainers[activeTabIndex].GetComponentsInChildren<SkillNodeUI>(true);
            HashSet<(SkillNodeUI, SkillNodeUI)> drawnConnections = new HashSet<(SkillNodeUI, SkillNodeUI)>();

            foreach (var node in nodes)
            {
                if (node.childNodes == null) continue;

                foreach (var child in node.childNodes)
                {
                    if (child == null) continue;

                    var connection = node.GetInstanceID() < child.GetInstanceID() ? (node, child) : (child, node);
                    if (drawnConnections.Contains(connection)) continue;

                    drawnConnections.Add(connection);
                    DrawLineBetweenNodes(node, child);
                }
            }
        }

        private void DrawLineBetweenNodes(SkillNodeUI startNode, SkillNodeUI endNode)
        {
            RectTransform startRect = startNode.GetComponent<RectTransform>();
            RectTransform endRect = endNode.GetComponent<RectTransform>();

            if (startRect == null || endRect == null) return;

            GameObject lineObj;
            if (linePrefab != null)
            {
                lineObj = Instantiate(linePrefab, linesContainer);
            }
            else
            {
                lineObj = new GameObject("ConnectionLine", typeof(RectTransform), typeof(Image));
                lineObj.transform.SetParent(linesContainer, false);
            }
            activeLines.Add(lineObj);

            RectTransform lineRect = lineObj.GetComponent<RectTransform>();
            Image lineImg = lineObj.GetComponent<Image>();

            Vector2 startPos = startRect.anchoredPosition;
            Vector2 endPos = endRect.anchoredPosition;
            Vector2 direction = endPos - startPos;
            float distance = direction.magnitude;

            lineRect.anchorMin = new Vector2(0.5f, 0.5f);
            lineRect.anchorMax = new Vector2(0.5f, 0.5f);
            lineRect.pivot = new Vector2(0f, 0.5f);
            lineRect.anchoredPosition = startPos;
            lineRect.sizeDelta = new Vector2(distance, lineThickness);

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            lineRect.localRotation = Quaternion.Euler(0f, 0f, angle);

            if (!startNode.IsLocked && !endNode.IsLocked)
            {
                lineImg.color = startNode.ActiveColor;
            }
            else
            {
                lineImg.color = connectionLockedColor;
            }
        }

        private void UpdateStatsUI()
        {
            // Fills proporcionales al índice del rango actual (0/7 a 7/7)
            // Controlamos la altura de la barra cambiando anchorMax.y del RectTransform
            float maxIndex = grades.Length - 1; // 7

            if (strBarFill != null) strBarFill.anchorMax = new Vector2(1f, (float)strGradeIndex / maxIndex);
            if (focBarFill != null) focBarFill.anchorMax = new Vector2(1f, (float)focGradeIndex / maxIndex);
            if (tecBarFill != null) tecBarFill.anchorMax = new Vector2(1f, (float)tecGradeIndex / maxIndex);
            if (hpBarFill != null) hpBarFill.anchorMax = new Vector2(1f, (float)hpGradeIndex / maxIndex);

            // Mostrar el valor en letra correspondiente al rango actual
            if (strValueText != null) strValueText.text = grades[strGradeIndex];
            if (focValueText != null) focValueText.text = grades[focGradeIndex];
            if (tecValueText != null) tecValueText.text = grades[tecGradeIndex];
            if (hpValueText != null) hpValueText.text = grades[hpGradeIndex];



            if (statPointsText != null) statPointsText.text = $"STAT POINTS: {statPoints}";
        }

        private void OnRefundClicked()
        {
            Debug.Log("[SkillTreeUI] Refund Stats clicked!");
            
            // 1. Calcular puntos gastados para devolverlos
            int refundedPoints = 0;
            foreach (var container in tabContainers)
            {
                if (container == null) continue;
                SkillNodeUI[] nodes = container.GetComponentsInChildren<SkillNodeUI>(true);
                foreach (var node in nodes)
                {
                    // Saltar los nodos iniciales "Awaken" o "Start" que están desbloqueados gratis
                    if (node.gameObject.name.Contains("Awaken") || node.gameObject.name.Contains("Start"))
                    {
                        continue;
                    }
                    refundedPoints += node.CurrentLevel;
                }
            }

            // Devolver los puntos gastados
            statPoints += refundedPoints;

            // 2. Restablecer los nodos a su estado inicial
            foreach (var container in tabContainers)
            {
                if (container == null) continue;
                SkillNodeUI[] nodes = container.GetComponentsInChildren<SkillNodeUI>(true);
                foreach (var node in nodes)
                {
                    if (node.gameObject.name.Contains("Awaken") || node.gameObject.name.Contains("Start"))
                    {
                        node.SetState(false, 1);
                    }
                    else
                    {
                        node.SetState(true, 0);
                    }
                }
            }

            // 3. Reiniciar el rango de los atributos a F (0)
            strGradeIndex = 0;
            focGradeIndex = 0;
            tecGradeIndex = 0;
            hpGradeIndex = 0;

            GenerateConnectionLines();
            UpdateStatsUI();

            if (selectedNode != null && detailLevelText != null)
            {
                detailLevelText.text = $"LEVEL {selectedNode.CurrentLevel} / {selectedNode.MaxLevel}";
            }
        }

        private void OnMinusClicked()
        {
            Debug.Log("[SkillTreeUI] Minus button clicked!");
            
            // Quitar mejora si hay un nodo seleccionado y tiene niveles comprados
            if (selectedNode != null && selectedNode.CurrentLevel > 0)
            {
                // Saltar nodos iniciales
                if (selectedNode.gameObject.name.Contains("Awaken") || selectedNode.gameObject.name.Contains("Start"))
                {
                    return;
                }

                // Decrementar nivel del nodo
                int newLevel = selectedNode.CurrentLevel - 1;
                bool shouldLock = (newLevel == 0);
                selectedNode.SetState(shouldLock, newLevel);

                // Devolver punto
                statPoints++;

                // Reducir la estadística de la categoría correspondiente
                DecreaseStatForTab(activeTabIndex);

                GenerateConnectionLines();
                UpdateStatsUI();

                if (detailLevelText != null)
                {
                    detailLevelText.text = $"LEVEL {selectedNode.CurrentLevel} / {selectedNode.MaxLevel}";
                }
            }
        }

        private void OnPlusClicked()
        {
            Debug.Log("[SkillTreeUI] Plus button clicked!");

            if (selectedNode == null) return;
            if (statPoints <= 0)
            {
                Debug.LogWarning("[SkillTree] ¡No tienes Stat Points suficientes!");
                return;
            }

            // Comprobar si el nodo ya está al nivel máximo
            if (selectedNode.CurrentLevel >= selectedNode.MaxLevel)
            {
                Debug.LogWarning("[SkillTree] Este nodo ya está al nivel máximo.");
                return;
            }

            // Comprobar si el nodo padre está desbloqueado (para mantener la lógica del árbol)
            bool isParentUnlocked = false;
            if (selectedNode.parentNodes == null || selectedNode.parentNodes.Length == 0)
            {
                isParentUnlocked = true; // Nodo raíz
            }
            else
            {
                foreach (var parent in selectedNode.parentNodes)
                {
                    if (parent != null && !parent.IsLocked)
                    {
                        isParentUnlocked = true;
                        break;
                    }
                }
            }

            if (!isParentUnlocked)
            {
                Debug.LogWarning("[SkillTree] Debes desbloquear el nodo previo primero.");
                return;
            }

            // Realizar la mejora
            statPoints--;
            selectedNode.SetState(false, selectedNode.CurrentLevel + 1);

            // Aumentar la estadística de la categoría correspondiente
            IncreaseStatForTab(activeTabIndex);

            GenerateConnectionLines();
            UpdateStatsUI();

            if (detailLevelText != null)
            {
                detailLevelText.text = $"LEVEL {selectedNode.CurrentLevel} / {selectedNode.MaxLevel}";
            }
        }

        private void IncreaseStatForTab(int tabIndex)
        {
            switch (tabIndex)
            {
                case 0: // HEALTH
                    hpGradeIndex = Mathf.Clamp(hpGradeIndex + 1, 0, grades.Length - 1);
                    break;
                case 1: // FOCUS
                    focGradeIndex = Mathf.Clamp(focGradeIndex + 1, 0, grades.Length - 1);
                    break;
                case 2: // TECHNIQUE
                    tecGradeIndex = Mathf.Clamp(tecGradeIndex + 1, 0, grades.Length - 1);
                    break;
                case 3: // STRENGTH
                case 4: // INNATES (Mapeado a STR por simplicidad)
                    strGradeIndex = Mathf.Clamp(strGradeIndex + 1, 0, grades.Length - 1);
                    break;
            }
        }

        private void DecreaseStatForTab(int tabIndex)
        {
            switch (tabIndex)
            {
                case 0: // HEALTH
                    hpGradeIndex = Mathf.Clamp(hpGradeIndex - 1, 0, grades.Length - 1);
                    break;
                case 1: // FOCUS
                    focGradeIndex = Mathf.Clamp(focGradeIndex - 1, 0, grades.Length - 1);
                    break;
                case 2: // TECHNIQUE
                    tecGradeIndex = Mathf.Clamp(tecGradeIndex - 1, 0, grades.Length - 1);
                    break;
                case 3: // STRENGTH
                case 4: // INNATES
                    strGradeIndex = Mathf.Clamp(strGradeIndex - 1, 0, grades.Length - 1);
                    break;
            }
        }
    }
}
