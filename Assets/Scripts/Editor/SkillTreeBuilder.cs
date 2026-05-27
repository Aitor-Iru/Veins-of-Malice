using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using VeinsOfMalice.UI;
using UnityEditor.SceneManagement;

namespace VeinsOfMalice.EditorTools
{
    public class SkillTreeBuilder : EditorWindow
    {
        [MenuItem("Tools/Veins of Malice/UI Generators/3. Construir Árbol de Habilidades (Visual)")]
        public static void BuildSkillTree()
        {
            // 1. Obtener o crear Canvas
            Canvas canvas = Object.FindAnyObjectByType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObj = new GameObject("MainCanvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                
                CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                
                canvasObj.AddComponent<GraphicRaycaster>();
            }

            // 2. Limpiar panel anterior si existe
            GameObject oldPanel = GameObject.Find("SkillTreePanel");
            if (oldPanel != null) DestroyImmediate(oldPanel);

            // 3. Crear el panel principal del Árbol de Habilidades
            GameObject panelObj = new GameObject("SkillTreePanel", typeof(RectTransform), typeof(Image));
            panelObj.transform.SetParent(canvas.transform, false);
            RectTransform panelRect = panelObj.GetComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero; // Estirado completo
            panelRect.anchorMax = Vector2.one;
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            // Fondo oscuro premium con tinte azulado
            Image panelBg = panelObj.GetComponent<Image>();
            panelBg.color = new Color(0.015f, 0.015f, 0.03f, 0.98f);

            // Agregar el componente manager
            SkillTreeUI skillTreeUI = panelObj.AddComponent<SkillTreeUI>();

            // --- DISEÑAR LAYOUT EN TRES COLUMNAS ---
            
            // 4. Contenedor Izquierdo: Detalle del Nodo
            GameObject leftPanel = CreateUIObject("Left_DetailPanel", panelObj.transform, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(50f, 0f), new Vector2(350f, 600f));
            CreateImage("Background", leftPanel.transform, new Color(0.05f, 0.05f, 0.08f, 0.8f));
            CreateOutline("Outline", leftPanel.transform, new Color(0.15f, 0.15f, 0.25f, 0.5f), 2f);
            
            TextMeshProUGUI detailTitle = CreateText("TitleText", leftPanel.transform, "SELECCIONA UN NODO", 22, Color.white, TextAlignmentOptions.Center);
            detailTitle.rectTransform.anchoredPosition = new Vector2(0f, 250f);
            detailTitle.fontStyle = FontStyles.Bold;

            TextMeshProUGUI detailLevel = CreateText("LevelText", leftPanel.transform, "", 16, new Color(0.6f, 0.6f, 0.8f), TextAlignmentOptions.Center);
            detailLevel.rectTransform.anchoredPosition = new Vector2(0f, 200f);

            // Cuadro vacío en el centro del detalle para simular el look
            GameObject detailArtMock = CreateUIObject("ArtMock", leftPanel.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 0f), new Vector2(180f, 180f));
            CreateImage("Bg", detailArtMock.transform, new Color(0.08f, 0.08f, 0.12f, 1f));
            CreateOutline("Outline", detailArtMock.transform, new Color(0.2f, 0.2f, 0.3f, 0.5f), 1f);
            CreateText("CrossText", detailArtMock.transform, "+", 60, new Color(1f, 1f, 1f, 0.15f), TextAlignmentOptions.Center);

            // 5. Contenedor Derecho: Estadísticas (STR, FOC, TEC, HP)
            GameObject rightPanel = CreateUIObject("Right_StatsPanel", panelObj.transform, new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(-50f, 0f), new Vector2(300f, 600f));
            CreateImage("Background", rightPanel.transform, new Color(0.05f, 0.05f, 0.08f, 0.8f));
            CreateOutline("Outline", rightPanel.transform, new Color(0.15f, 0.15f, 0.25f, 0.5f), 2f);

            // Contenedor de barras
            GameObject barsContainer = CreateUIObject("BarsContainer", rightPanel.transform, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), new Vector2(15f, 15f), new Vector2(-30f, -30f));
            
            // Crear las 4 barras verticales vacías (F)
            RectTransform strBar = CreateVerticalBar(barsContainer.transform, "STR", new Color(0.85f, 0.35f, 0.25f), new Vector2(-90f, 0f), 0f, "F", out TextMeshProUGUI strVal);
            RectTransform focBar = CreateVerticalBar(barsContainer.transform, "FOC", new Color(0.25f, 0.75f, 0.75f), new Vector2(-30f, 0f), 0f, "F", out TextMeshProUGUI focVal);
            RectTransform tecBar = CreateVerticalBar(barsContainer.transform, "TEC", new Color(0.6f, 0.35f, 0.85f), new Vector2(30f, 0f), 0f, "F", out TextMeshProUGUI tecVal);
            RectTransform hpBar  = CreateVerticalBar(barsContainer.transform, "HP",  new Color(0.35f, 0.85f, 0.35f), new Vector2(90f, 0f), 0f, "F", out TextMeshProUGUI hpVal);

            // 6. Contenedor Central/Superior: Nodos y Conexiones
            GameObject centerPanel = CreateUIObject("Center_TreePanel", panelObj.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 50f), new Vector2(1000f, 600f));
            
            // Contenedor de líneas de conexión
            GameObject linesContainer = CreateUIObject("Lines_Container", centerPanel.transform, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            
            // 7. Pestañas Superiores (Tab Buttons)
            GameObject tabsPanel = CreateUIObject("Tabs_Panel", panelObj.transform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 40f), new Vector2(900f, 50f));
            
            string[] tabNames = { "HEALTH", "FOCUS", "TECHNIQUE", "STRENGTH", "INNATES" };
            Button[] tabButtons = new Button[5];
            GameObject[] tabContainers = new GameObject[5];

            float tabWidth = 170f;
            float tabSpacing = 10f;
            float startTabX = -((tabWidth * 5) + (tabSpacing * 4)) / 2f + (tabWidth / 2f);

            for (int i = 0; i < 5; i++)
            {
                int index = i;
                GameObject tabBtnObj = CreateUIObject($"Tab_{tabNames[i]}", tabsPanel.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(startTabX + i * (tabWidth + tabSpacing), 0f), new Vector2(tabWidth, 40f));
                
                // Fondo del botón
                CreateImage("Bg", tabBtnObj.transform, new Color(0.08f, 0.08f, 0.15f, 1f));
                CreateOutline("Outline", tabBtnObj.transform, new Color(0.25f, 0.25f, 0.4f, 0.5f), 1f);
                
                // Texto de la pestaña
                CreateText("Text", tabBtnObj.transform, tabNames[i], 18, Color.gray, TextAlignmentOptions.Center);
                
                tabButtons[i] = tabBtnObj.AddComponent<Button>();

                // Crear contenedor correspondiente
                GameObject tabContent = CreateUIObject($"Container_{tabNames[i]}", centerPanel.transform, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
                tabContainers[i] = tabContent;
                tabContent.SetActive(false);
            }

            // 8. Contenedor Central Inferior (Stat Points y Refund)
            GameObject bottomPanel = CreateUIObject("Bottom_ControlPanel", panelObj.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -320f), new Vector2(400f, 80f));
            CreateImage("Bg", bottomPanel.transform, new Color(0.04f, 0.04f, 0.08f, 0.9f));
            CreateOutline("Outline", bottomPanel.transform, new Color(0.2f, 0.2f, 0.35f, 0.5f), 1.5f);

            // Botones Menos (-) y Mas (+)
            GameObject btnMinusObj = CreateUIObject("Btn_Minus", bottomPanel.transform, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(-40f, 0f), new Vector2(50f, 50f));
            CreateImage("Bg", btnMinusObj.transform, new Color(0.08f, 0.08f, 0.15f, 1f));
            CreateOutline("Outline", btnMinusObj.transform, new Color(0.3f, 0.3f, 0.5f, 0.6f), 1f);
            CreateText("Text", btnMinusObj.transform, "-", 28, Color.white, TextAlignmentOptions.Center);
            Button minusBtn = btnMinusObj.AddComponent<Button>();

            GameObject btnPlusObj = CreateUIObject("Btn_Plus", bottomPanel.transform, new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(40f, 0f), new Vector2(50f, 50f));
            CreateImage("Bg", btnPlusObj.transform, new Color(0.08f, 0.08f, 0.15f, 1f));
            CreateOutline("Outline", btnPlusObj.transform, new Color(0.3f, 0.3f, 0.5f, 0.6f), 1f);
            CreateText("Text", btnPlusObj.transform, "+", 28, Color.white, TextAlignmentOptions.Center);
            Button plusBtn = btnPlusObj.AddComponent<Button>();

            // Texto de Stat Points
            TextMeshProUGUI statPointsText = CreateText("StatPointsText", bottomPanel.transform, "STAT POINTS: 0", 18, Color.white, TextAlignmentOptions.Center);
            statPointsText.rectTransform.anchoredPosition = new Vector2(0f, 15f);

            // Botón de Refund (Rombo) - QUITADO CASH
            GameObject btnRefundObj = CreateUIObject("Btn_Refund", bottomPanel.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -22f), new Vector2(110f, 25f));
            CreateImage("Bg", btnRefundObj.transform, new Color(0.08f, 0.08f, 0.15f, 1f));
            CreateOutline("Outline", btnRefundObj.transform, new Color(0.4f, 0.4f, 0.6f, 0.7f), 1f);
            CreateText("Text", btnRefundObj.transform, "REFUND STATS", 10, new Color(0.8f, 0.8f, 1f), TextAlignmentOptions.Center);
            Button refundBtn = btnRefundObj.AddComponent<Button>();

            // --- 9. CONSTRUIR LOS NODOS DE LA PESTAÑA "HEALTH" ---
            GameObject healthContainer = tabContainers[0];

            // Nodos principales y posiciones
            // Fila Central
            SkillNodeUI nodeAwaken = CreateNode(healthContainer.transform, "HealthAwaken", new Vector2(-380f, 0f), false, 1, 1, "Awaken", Color.green);
            SkillNodeUI nodeHealth1 = CreateNode(healthContainer.transform, "HealthIncrease1", new Vector2(-220f, 0f), true, 0, 3, "3", Color.green);
            SkillNodeUI nodeHealth2 = CreateNode(healthContainer.transform, "HealthIncrease2", new Vector2(-60f, 0f), true, 0, 3, "3", Color.green);
            SkillNodeUI nodeSlowRCT  = CreateNode(healthContainer.transform, "SlowRCT", new Vector2(100f, 0f), true, 0, 1, "30", Color.blue);

            // Ramas Verticales
            SkillNodeUI nodeLockAboveH1 = CreateNode(healthContainer.transform, "LockAboveH1", new Vector2(-220f, 150f), true, 0, 1, "3", Color.red);
            
            SkillNodeUI nodeVampFocus1 = CreateNode(healthContainer.transform, "VampiricFocus1", new Vector2(-60f, 120f), true, 0, 3, "3", new Color(0.5f, 0.2f, 1f)); // Púrpura
            SkillNodeUI nodeVampFocus2 = CreateNode(healthContainer.transform, "VampiricFocus2", new Vector2(60f, 200f), true, 0, 1, "25", new Color(0.5f, 0.2f, 1f)); // Púrpura/Bloqueado
            
            SkillNodeUI nodeDecayResist = CreateNode(healthContainer.transform, "DecayResistance", new Vector2(-60f, -120f), true, 0, 3, "3", Color.grey);

            // Cluster de Bloqueados Derecho (Candados)
            Vector2 clusterCenter = new Vector2(280f, 0f);
            SkillNodeUI nodeLockCLeft  = CreateNode(healthContainer.transform, "LockCenterLeft",  clusterCenter + new Vector2(-80f, 0f), true, 0, 1, "5", Color.magenta);
            SkillNodeUI nodeLockCRight = CreateNode(healthContainer.transform, "LockCenterRight", clusterCenter + new Vector2(80f, 0f),  true, 0, 1, "6", Color.magenta);
            SkillNodeUI nodeLockFarR   = CreateNode(healthContainer.transform, "LockFarRight",    clusterCenter + new Vector2(200f, 0f), true, 0, 1, "60", Color.red);
            
            SkillNodeUI nodeLockTLeft  = CreateNode(healthContainer.transform, "LockTopLeft",     clusterCenter + new Vector2(0f, 100f),  true, 0, 1, "15", Color.yellow);
            SkillNodeUI nodeLockTL_UL  = CreateNode(healthContainer.transform, "LockTL_UpLeft",   clusterCenter + new Vector2(-80f, 200f),true, 0, 1, "10", Color.yellow);
            SkillNodeUI nodeLockTL_UR  = CreateNode(healthContainer.transform, "LockTL_UpRight",  clusterCenter + new Vector2(80f, 200f), true, 0, 1, "25", Color.yellow);
            
            SkillNodeUI nodeLockBLeft  = CreateNode(healthContainer.transform, "LockBottomLeft",  clusterCenter + new Vector2(0f, -100f), true, 0, 1, "15", Color.yellow);
            SkillNodeUI nodeLockTRight = CreateNode(healthContainer.transform, "LockTopRight",    clusterCenter + new Vector2(140f, 100f),true, 0, 1, "25", Color.red);
            SkillNodeUI nodeLockBRight = CreateNode(healthContainer.transform, "LockBottomRight", clusterCenter + new Vector2(140f, -100f),true, 0, 1, "10", Color.red);

            // --- CONFIGURAR DEPENDENCIAS ---
            nodeAwaken.childNodes = new[] { nodeHealth1 };
            
            nodeHealth1.parentNodes = new[] { nodeAwaken };
            nodeHealth1.childNodes = new[] { nodeHealth2, nodeLockAboveH1 };
            
            nodeLockAboveH1.parentNodes = new[] { nodeHealth1 };

            nodeHealth2.parentNodes = new[] { nodeHealth1 };
            nodeHealth2.childNodes = new[] { nodeVampFocus1, nodeDecayResist, nodeSlowRCT };

            nodeVampFocus1.parentNodes = new[] { nodeHealth2 };
            nodeVampFocus1.childNodes = new[] { nodeVampFocus2 };

            nodeVampFocus2.parentNodes = new[] { nodeVampFocus1 };
            
            nodeDecayResist.parentNodes = new[] { nodeHealth2 };

            nodeSlowRCT.parentNodes = new[] { nodeHealth2 };
            nodeSlowRCT.childNodes = new[] { nodeLockCLeft };

            // Conexiones del cluster
            nodeLockCLeft.parentNodes = new[] { nodeSlowRCT };
            nodeLockCLeft.childNodes = new[] { nodeLockTLeft, nodeLockBLeft, nodeLockCRight };

            nodeLockTLeft.parentNodes = new[] { nodeLockCLeft };
            nodeLockTLeft.childNodes = new[] { nodeLockTL_UL, nodeLockTL_UR };

            nodeLockTL_UL.parentNodes = new[] { nodeLockTLeft };
            nodeLockTL_UR.parentNodes = new[] { nodeLockTLeft };

            nodeLockBLeft.parentNodes = new[] { nodeLockCLeft };

            nodeLockCRight.parentNodes = new[] { nodeLockCLeft };
            nodeLockCRight.childNodes = new[] { nodeLockTRight, nodeLockBRight, nodeLockFarR };

            nodeLockTRight.parentNodes = new[] { nodeLockCRight };
            nodeLockBRight.parentNodes = new[] { nodeLockCRight };
            nodeLockFarR.parentNodes = new[] { nodeLockCRight };

            // --- 10. AÑADIR UN PAR DE NODOS DE MUESTRA PARA OTRAS PESTAÑAS ---
            for (int t = 1; t < 5; t++)
            {
                CreateNode(tabContainers[t].transform, "StartNode", new Vector2(-150f, 0f), false, 1, 1, "Start", Color.cyan);
                CreateNode(tabContainers[t].transform, "UpgradeNode", new Vector2(150f, 0f), true, 0, 3, "15", Color.cyan);
            }

            // --- 11. ASIGNAR REFERENCIAS EN EL MANAGER ---
            SerializedObject so = new SerializedObject(skillTreeUI);
            
            // Pestañas
            SerializedProperty propButtons = so.FindProperty("tabButtons");
            propButtons.arraySize = 5;
            for (int i = 0; i < 5; i++)
            {
                propButtons.GetArrayElementAtIndex(i).objectReferenceValue = tabButtons[i];
            }

            SerializedProperty propContainers = so.FindProperty("tabContainers");
            propContainers.arraySize = 5;
            for (int i = 0; i < 5; i++)
            {
                propContainers.GetArrayElementAtIndex(i).objectReferenceValue = tabContainers[i];
            }

            // Stats
            so.FindProperty("strBarFill").objectReferenceValue = strBar;
            so.FindProperty("focBarFill").objectReferenceValue = focBar;
            so.FindProperty("tecBarFill").objectReferenceValue = tecBar;
            so.FindProperty("hpBarFill").objectReferenceValue = hpBar;

            so.FindProperty("strValueText").objectReferenceValue = strVal;
            so.FindProperty("focValueText").objectReferenceValue = focVal;
            so.FindProperty("tecValueText").objectReferenceValue = tecVal;
            so.FindProperty("hpValueText").objectReferenceValue = hpVal;



            // Controles
            so.FindProperty("statPointsText").objectReferenceValue = statPointsText;
            so.FindProperty("refundButton").objectReferenceValue = refundBtn;
            so.FindProperty("minusButton").objectReferenceValue = minusBtn;
            so.FindProperty("plusButton").objectReferenceValue = plusBtn;

            // Detalle
            so.FindProperty("detailPanel").objectReferenceValue = leftPanel;
            so.FindProperty("detailTitleText").objectReferenceValue = detailTitle;
            so.FindProperty("detailLevelText").objectReferenceValue = detailLevel;

            // Líneas
            so.FindProperty("linesContainer").objectReferenceValue = linesContainer.GetComponent<RectTransform>();

            so.ApplyModifiedProperties();

            // Desactivar el panel general para que empiece oculto, listo para abrir con pausa
            panelObj.SetActive(false);

            // Conectar el panel al GameUIManager en el Canvas
            GameUIManager uiManager = canvas.GetComponent<GameUIManager>();
            if (uiManager != null)
            {
                SerializedObject soUI = new SerializedObject(uiManager);
                soUI.FindProperty("upgradesPanel").objectReferenceValue = panelObj;
                soUI.ApplyModifiedProperties();
            }

            // Marcar la escena como modificada para que se guarde
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

            Selection.activeGameObject = panelObj;
            Debug.Log("<color=green>[SkillTreeBuilder]</color> ¡Árbol de Habilidades visual interactivo generado y conectado con éxito!");
        }

        // --- MÉTODOS AUXILIARES ---

        private static GameObject CreateUIObject(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 anchoredPos, Vector2 sizeDelta)
        {
            GameObject obj = new GameObject(name, typeof(RectTransform));
            obj.transform.SetParent(parent, false);
            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;
            rect.anchoredPosition = anchoredPos;
            rect.sizeDelta = sizeDelta;
            return obj;
        }

        private static Image CreateImage(string name, Transform parent, Color color)
        {
            GameObject obj = new GameObject(name, typeof(RectTransform), typeof(Image));
            obj.transform.SetParent(parent, false);
            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            Image img = obj.GetComponent<Image>();
            img.color = color;
            return img;
        }

        private static Outline CreateOutline(string name, Transform parent, Color color, float thickness)
        {
            GameObject obj = new GameObject(name, typeof(RectTransform), typeof(Image));
            obj.transform.SetParent(parent, false);
            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            Image img = obj.GetComponent<Image>();
            img.color = Color.clear;
            
            Outline outline = obj.AddComponent<Outline>();
            outline.effectColor = color;
            outline.effectDistance = new Vector2(thickness, thickness);
            return outline;
        }

        private static TextMeshProUGUI CreateText(string name, Transform parent, string text, float fontSize, Color color, TextAlignmentOptions alignment)
        {
            GameObject obj = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
            obj.transform.SetParent(parent, false);
            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(250f, 50f);
            
            TextMeshProUGUI tmp = obj.GetComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.color = color;
            tmp.alignment = alignment;
            tmp.raycastTarget = false;
            return tmp;
        }

        private static RectTransform CreateVerticalBar(Transform parent, string label, Color barColor, Vector2 xOffset, float initialFill, string initialVal, out TextMeshProUGUI valText)
        {
            // Contenedor de barra individual
            GameObject container = CreateUIObject($"Bar_{label}", parent, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), xOffset, new Vector2(40f, 400f));
            
            // Fondo de la barra
            CreateImage("Bg", container.transform, new Color(0.08f, 0.08f, 0.12f, 0.9f));
            CreateOutline("Outline", container.transform, new Color(0.2f, 0.2f, 0.3f, 0.5f), 1f);

            // Barra de relleno (Fill) — Usa anchors para controlar la altura visible
            // anchorMin=(0,0) anchorMax=(1, fillLevel) para que la barra crezca de abajo hacia arriba
            GameObject fillObj = new GameObject("Fill", typeof(RectTransform), typeof(Image));
            fillObj.transform.SetParent(container.transform, false);
            RectTransform fillRect = fillObj.GetComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = new Vector2(1f, initialFill); // 0 = vacío, 1 = lleno
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;
            Image fillImg = fillObj.GetComponent<Image>();
            fillImg.color = barColor;

            // Letra/Valor de rango en el medio (ej. SSS o A)
            valText = CreateText("ValueText", container.transform, initialVal, 18, Color.white, TextAlignmentOptions.Center);
            valText.fontStyle = FontStyles.Bold;
            valText.rectTransform.anchoredPosition = new Vector2(0f, 0f);

            // Etiqueta abajo
            TextMeshProUGUI labelText = CreateText("LabelText", container.transform, label, 12, new Color(0.5f, 0.5f, 0.6f), TextAlignmentOptions.Center);
            labelText.rectTransform.anchoredPosition = new Vector2(0f, -220f);

            return fillRect;
        }

        private static SkillNodeUI CreateNode(Transform parent, string name, Vector2 pos, bool isLocked, int level, int maxLevel, string costStr, Color activeColor)
        {
            // Contenedor principal del nodo
            GameObject nodeContainer = CreateUIObject(name, parent, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), pos, new Vector2(70f, 75f));

            // Botón del nodo (Rombo rotado a 45 grados)
            GameObject diamondButtonObj = CreateUIObject("DiamondButton", nodeContainer.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 8f), new Vector2(50f, 50f));
            diamondButtonObj.transform.localRotation = Quaternion.Euler(0f, 0f, 45f);
            
            // Añadir botón e imagen de borde
            Image outlineImg = diamondButtonObj.AddComponent<Image>();
            outlineImg.color = activeColor;
            Button button = diamondButtonObj.AddComponent<Button>();

            // Imagen de Relleno Interno (Relleno horizontal para simular barra de progreso diagonal por rotación)
            GameObject fillObj = CreateUIObject("Fill", diamondButtonObj.transform, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(-8f, -8f));
            Image fillImg = fillObj.AddComponent<Image>();
            fillImg.color = activeColor;
            fillImg.type = Image.Type.Filled;
            fillImg.fillMethod = Image.FillMethod.Horizontal;
            fillImg.fillAmount = 1f;

            // Icono de candado (rotado en sentido opuesto -45 grados para quedar recto)
            GameObject lockObj = CreateUIObject("LockIcon", diamondButtonObj.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(30f, 30f));
            lockObj.transform.localRotation = Quaternion.Euler(0f, 0f, -45f);
            TextMeshProUGUI lockTxt = lockObj.AddComponent<TextMeshProUGUI>();
            lockTxt.text = "🔒"; // Símbolo de candado unicode
            lockTxt.fontSize = 20;
            lockTxt.alignment = TextAlignmentOptions.Center;
            lockTxt.color = Color.white;
            lockTxt.raycastTarget = false;
            
            // Resalte de selección (borde exterior extra)
            GameObject highlightObj = CreateUIObject("Highlight", diamondButtonObj.transform, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(10f, 10f));
            Image highImg = highlightObj.AddComponent<Image>();
            highImg.color = Color.clear;
            Outline highOutline = highlightObj.AddComponent<Outline>();
            highOutline.effectColor = Color.white;
            highOutline.effectDistance = new Vector2(3f, 3f);
            highlightObj.SetActive(false);

            // Texto inferior de nivel/coste (fuera del rombo rotado para no distorsionar)
            GameObject labelObj = CreateUIObject("LevelText", nodeContainer.transform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0.5f), new Vector2(0f, -22f), new Vector2(60f, 20f));
            TextMeshProUGUI costText = labelObj.AddComponent<TextMeshProUGUI>();
            costText.text = costStr;
            costText.fontSize = 11;
            costText.alignment = TextAlignmentOptions.Center;
            costText.color = new Color(0.7f, 0.7f, 0.7f);
            costText.fontStyle = FontStyles.Italic;
            costText.raycastTarget = false;

            // Añadir y configurar el script SkillNodeUI
            SkillNodeUI skillNode = nodeContainer.AddComponent<SkillNodeUI>();
            SerializedObject soNode = new SerializedObject(skillNode);
            
            soNode.FindProperty("nodeName").stringValue = name;
            soNode.FindProperty("isLocked").boolValue = isLocked;
            soNode.FindProperty("currentLevel").intValue = level;
            soNode.FindProperty("maxLevel").intValue = maxLevel;
            soNode.FindProperty("costValueString").stringValue = costStr;
            soNode.FindProperty("activeColor").colorValue = activeColor;
            
            soNode.FindProperty("outlineImage").objectReferenceValue = outlineImg;
            soNode.FindProperty("fillImage").objectReferenceValue = fillImg;
            soNode.FindProperty("lockIcon").objectReferenceValue = lockObj;
            soNode.FindProperty("costText").objectReferenceValue = costText;
            soNode.FindProperty("selectionHighlight").objectReferenceValue = highlightObj;
            
            soNode.ApplyModifiedProperties();
            skillNode.UpdateVisuals();

            return skillNode;
        }
    }
}
