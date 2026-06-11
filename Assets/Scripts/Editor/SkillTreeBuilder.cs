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
            
            string[] tabNames = { "HEALTH", "FOCUS", "STRENGTH", "TECHNIQUE" };
            Button[] tabButtons = new Button[4];
            GameObject[] tabContainers = new GameObject[4];

            float tabWidth = 170f;
            float tabSpacing = 10f;
            float startTabX = -((tabWidth * 4) + (tabSpacing * 3)) / 2f + (tabWidth / 2f);

            for (int i = 0; i < 4; i++)
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

            SkillNodeUI nodeAwaken = CreateNode(healthContainer.transform, "HealthAwaken", new Vector2(-380f, 0f), false, 1, 1, "2", Color.green);
            SkillNodeUI nodeH1_Mid = CreateNode(healthContainer.transform, "Health_C1_Mid", new Vector2(-240f, 0f), true, 0, 3, "3", Color.green);
            SkillNodeUI nodeH1_Top = CreateNode(healthContainer.transform, "Health_C1_Top", new Vector2(-240f, 120f), true, 0, 3, "3", Color.green);
            SkillNodeUI nodeH1_Bot = CreateNode(healthContainer.transform, "Health_C1_Bot", new Vector2(-240f, -120f), true, 0, 3, "3", Color.green);
            
            SkillNodeUI nodeH2_Mid = CreateNode(healthContainer.transform, "Health_C2_Mid", new Vector2(-100f, 0f), true, 0, 3, "4", Color.green);
            SkillNodeUI nodeH2_Top = CreateNode(healthContainer.transform, "Health_C2_Top", new Vector2(-100f, 120f), true, 0, 3, "3", Color.green);
            SkillNodeUI nodeH2_Bot = CreateNode(healthContainer.transform, "Health_C2_Bot", new Vector2(-100f, -120f), true, 0, 3, "3", Color.green);
            SkillNodeUI nodeH2_TopR = CreateNode(healthContainer.transform, "Health_C2_Top_Right", new Vector2(20f, 200f), true, 0, 3, "3", Color.green);

            SkillNodeUI nodeH3_Mid = CreateNode(healthContainer.transform, "Health_C3_Mid", new Vector2(140f, 0f), true, 0, 1, "30", Color.blue);

            // Cluster de Bloqueados Derecho (Candados)
            SkillNodeUI nodeHLockCLeft  = CreateNode(healthContainer.transform, "LockCenterLeft",  new Vector2(260f, 0f), true, 0, 1, "5", Color.magenta);
            SkillNodeUI nodeHLockCRight = CreateNode(healthContainer.transform, "LockCenterRight", new Vector2(380f, 0f),  true, 0, 1, "6", Color.magenta);
            SkillNodeUI nodeHLockFarR   = CreateNode(healthContainer.transform, "LockFarRight",    new Vector2(500f, 0f), true, 0, 1, "60", Color.red);
            
            SkillNodeUI nodeHLockTLeft  = CreateNode(healthContainer.transform, "LockTopLeft",     new Vector2(320f, 100f),  true, 0, 1, "15", Color.yellow);
            SkillNodeUI nodeHLockTL_UL  = CreateNode(healthContainer.transform, "LockTL_UpLeft",   new Vector2(260f, 200f),true, 0, 1, "10", Color.yellow);
            SkillNodeUI nodeHLockTL_UR  = CreateNode(healthContainer.transform, "LockTL_UpRight",  new Vector2(380f, 200f), true, 0, 1, "25", Color.yellow);
            
            SkillNodeUI nodeHLockBLeft  = CreateNode(healthContainer.transform, "LockBottomLeft",  new Vector2(320f, -100f), true, 0, 1, "15", Color.yellow);
            SkillNodeUI nodeHLockTRight = CreateNode(healthContainer.transform, "LockTopRight",    new Vector2(440f, 100f),true, 0, 1, "25", Color.red);
            SkillNodeUI nodeHLockBRight = CreateNode(healthContainer.transform, "LockBottomRight", new Vector2(440f, -100f),true, 0, 1, "10", Color.red);

            // --- CONFIGURAR DEPENDENCIAS HEALTH ---
            nodeAwaken.childNodes = new[] { nodeH1_Mid };
            
            nodeH1_Mid.parentNodes = new[] { nodeAwaken };
            nodeH1_Mid.childNodes = new[] { nodeH1_Top, nodeH1_Bot, nodeH2_Mid };
            
            nodeH1_Top.parentNodes = new[] { nodeH1_Mid };
            nodeH1_Bot.parentNodes = new[] { nodeH1_Mid };

            nodeH2_Mid.parentNodes = new[] { nodeH1_Mid };
            nodeH2_Mid.childNodes = new[] { nodeH2_Top, nodeH2_Bot, nodeH3_Mid };

            nodeH2_Top.parentNodes = new[] { nodeH2_Mid };
            nodeH2_Top.childNodes = new[] { nodeH2_TopR };

            nodeH2_TopR.parentNodes = new[] { nodeH2_Top };
            nodeH2_Bot.parentNodes = new[] { nodeH2_Mid };

            nodeH3_Mid.parentNodes = new[] { nodeH2_Mid };
            nodeH3_Mid.childNodes = new[] { nodeHLockCLeft };

            // Conexiones del cluster
            nodeHLockCLeft.parentNodes = new[] { nodeH3_Mid };
            nodeHLockCLeft.childNodes = new[] { nodeHLockTLeft, nodeHLockBLeft, nodeHLockCRight };

            nodeHLockTLeft.parentNodes = new[] { nodeHLockCLeft };
            nodeHLockTLeft.childNodes = new[] { nodeHLockTL_UL, nodeHLockTL_UR };

            nodeHLockTL_UL.parentNodes = new[] { nodeHLockTLeft };
            nodeHLockTL_UR.parentNodes = new[] { nodeHLockTLeft };

            nodeHLockBLeft.parentNodes = new[] { nodeHLockCLeft };

            nodeHLockCRight.parentNodes = new[] { nodeHLockCLeft };
            nodeHLockCRight.childNodes = new[] { nodeHLockTRight, nodeHLockBRight, nodeHLockFarR };

            nodeHLockTRight.parentNodes = new[] { nodeHLockCRight };
            nodeHLockBRight.parentNodes = new[] { nodeHLockCRight };
            nodeHLockFarR.parentNodes = new[] { nodeHLockCRight };


            // --- 9b. CONSTRUIR LOS NODOS DE LA PESTAÑA "FOCUS" ---
            GameObject focusContainer = tabContainers[1];

            SkillNodeUI focusAwaken = CreateNode(focusContainer.transform, "FocusAwaken", new Vector2(-380f, 0f), false, 1, 1, "2", Color.cyan);
            
            SkillNodeUI nodeF1_Mid = CreateNode(focusContainer.transform, "Focus_C1_Mid", new Vector2(-240f, 0f), true, 0, 3, "3", Color.cyan);
            SkillNodeUI nodeF1_Top = CreateNode(focusContainer.transform, "Focus_C1_Top", new Vector2(-240f, 120f), true, 0, 3, "15", Color.cyan);
            SkillNodeUI nodeF1_TopT = CreateNode(focusContainer.transform, "Focus_C1_Top_Top", new Vector2(-240f, 240f), true, 0, 3, "15", Color.cyan);
            SkillNodeUI nodeF1_Bot = CreateNode(focusContainer.transform, "Focus_C1_Bot", new Vector2(-240f, -120f), true, 0, 3, "3", Color.cyan);
            SkillNodeUI nodeF1_BotB = CreateNode(focusContainer.transform, "Focus_C1_Bot_Bot", new Vector2(-240f, -240f), true, 0, 3, "4", Color.cyan);

            SkillNodeUI nodeF2_Mid = CreateNode(focusContainer.transform, "Focus_C2_Mid", new Vector2(-100f, 0f), true, 0, 3, "4", Color.cyan);
            SkillNodeUI nodeF2_Top = CreateNode(focusContainer.transform, "Focus_C2_Top", new Vector2(-100f, 120f), true, 0, 3, "15", Color.cyan);
            SkillNodeUI nodeF2_TopT = CreateNode(focusContainer.transform, "Focus_C2_Top_Top", new Vector2(-100f, 240f), true, 0, 3, "15", Color.cyan);
            SkillNodeUI nodeF2_Bot = CreateNode(focusContainer.transform, "Focus_C2_Bot", new Vector2(-100f, -120f), true, 0, 3, "3", Color.cyan);
            SkillNodeUI nodeF2_BotB = CreateNode(focusContainer.transform, "Focus_C2_Bot_Bot", new Vector2(-100f, -240f), true, 0, 3, "5", Color.cyan);

            SkillNodeUI nodeF3_Mid = CreateNode(focusContainer.transform, "Focus_C3_Mid", new Vector2(140f, 0f), true, 0, 1, "40", Color.blue);
            SkillNodeUI nodeF3_TopL = CreateNode(focusContainer.transform, "Focus_C3_Top_Left", new Vector2(20f, 120f), true, 0, 3, "3", Color.cyan);
            SkillNodeUI nodeF3_TopT = CreateNode(focusContainer.transform, "Focus_C3_Top_Top", new Vector2(140f, 240f), true, 0, 3, "3", Color.cyan);
            SkillNodeUI nodeF3_BotL = CreateNode(focusContainer.transform, "Focus_C3_Bot_Left", new Vector2(20f, -120f), true, 0, 3, "3", Color.cyan);

            SkillNodeUI nodeFLockCLeft  = CreateNode(focusContainer.transform, "FocusLockCenterLeft",  new Vector2(260f, 0f), true, 0, 1, "5", Color.magenta);
            SkillNodeUI nodeFLockBLeft  = CreateNode(focusContainer.transform, "FocusLockBottomLeft",  new Vector2(260f, -120f), true, 0, 1, "15", Color.yellow);
            SkillNodeUI nodeFLockCRight = CreateNode(focusContainer.transform, "FocusLockCenterRight", new Vector2(380f, 0f), true, 0, 1, "6", Color.magenta);
            SkillNodeUI nodeFLockBRight = CreateNode(focusContainer.transform, "FocusLockBottomRight", new Vector2(380f, -120f), true, 0, 1, "15", Color.yellow);
            SkillNodeUI nodeFLockFarR   = CreateNode(focusContainer.transform, "FocusLockFarRight",    new Vector2(500f, 0f), true, 0, 1, "60", Color.red);

            // --- CONFIGURAR DEPENDENCIAS FOCUS ---
            focusAwaken.childNodes = new[] { nodeF1_Mid };
            
            nodeF1_Mid.parentNodes = new[] { focusAwaken };
            nodeF1_Mid.childNodes = new[] { nodeF1_Top, nodeF1_Bot, nodeF2_Mid };

            nodeF1_Top.parentNodes = new[] { nodeF1_Mid };
            nodeF1_Top.childNodes = new[] { nodeF1_TopT };
            nodeF1_TopT.parentNodes = new[] { nodeF1_Top };

            nodeF1_Bot.parentNodes = new[] { nodeF1_Mid };
            nodeF1_Bot.childNodes = new[] { nodeF1_BotB };
            nodeF1_BotB.parentNodes = new[] { nodeF1_Bot };
            nodeF1_BotB.childNodes = new[] { nodeF2_BotB }; // Conexión horizontal en la base

            nodeF2_Mid.parentNodes = new[] { nodeF1_Mid };
            nodeF2_Mid.childNodes = new[] { nodeF2_Top, nodeF2_Bot, nodeF3_Mid };

            nodeF2_Top.parentNodes = new[] { nodeF2_Mid };
            nodeF2_Top.childNodes = new[] { nodeF2_TopT };
            nodeF2_TopT.parentNodes = new[] { nodeF2_Top };

            nodeF2_Bot.parentNodes = new[] { nodeF2_Mid };
            nodeF2_Bot.childNodes = new[] { nodeF2_BotB };
            nodeF2_BotB.parentNodes = new[] { nodeF2_Bot, nodeF1_BotB };

            nodeF3_Mid.parentNodes = new[] { nodeF2_Mid };
            nodeF3_Mid.childNodes = new[] { nodeF3_TopL, nodeF3_BotL, nodeFLockCLeft };

            nodeF3_TopL.parentNodes = new[] { nodeF3_Mid };
            nodeF3_TopL.childNodes = new[] { nodeF3_TopT };
            nodeF3_TopT.parentNodes = new[] { nodeF3_TopL };

            nodeF3_BotL.parentNodes = new[] { nodeF3_Mid };

            nodeFLockCLeft.parentNodes = new[] { nodeF3_Mid };
            nodeFLockCLeft.childNodes = new[] { nodeFLockBLeft, nodeFLockCRight };

            nodeFLockBLeft.parentNodes = new[] { nodeFLockCLeft };

            nodeFLockCRight.parentNodes = new[] { nodeFLockCLeft };
            nodeFLockCRight.childNodes = new[] { nodeFLockBRight, nodeFLockFarR };

            nodeFLockBRight.parentNodes = new[] { nodeFLockCRight };
            nodeFLockFarR.parentNodes = new[] { nodeFLockCRight };


            // --- 9c. CONSTRUIR LOS NODOS DE LA PESTAÑA "STRENGTH" ---
            GameObject strengthContainer = tabContainers[2];

            SkillNodeUI strengthAwaken = CreateNode(strengthContainer.transform, "StrengthAwaken", new Vector2(-380f, 0f), false, 1, 1, "2", Color.red);
            
            SkillNodeUI nodeS1_Mid = CreateNode(strengthContainer.transform, "Strength_C1_Mid", new Vector2(-240f, 0f), true, 0, 3, "3", Color.red);
            SkillNodeUI nodeS1_Top = CreateNode(strengthContainer.transform, "Strength_C1_Top", new Vector2(-240f, 120f), true, 0, 3, "10", Color.red);
            SkillNodeUI nodeS1_TopT = CreateNode(strengthContainer.transform, "Strength_C1_Top_Top", new Vector2(-240f, 240f), true, 0, 3, "10", Color.red);
            SkillNodeUI nodeS1_Bot = CreateNode(strengthContainer.transform, "Strength_C1_Bot", new Vector2(-240f, -120f), true, 0, 3, "10", Color.red);
            SkillNodeUI nodeS1_BotB = CreateNode(strengthContainer.transform, "Strength_C1_Bot_Bot", new Vector2(-240f, -240f), true, 0, 3, "10", Color.red);

            SkillNodeUI nodeS2_Mid = CreateNode(strengthContainer.transform, "Strength_C2_Mid", new Vector2(-100f, 0f), true, 0, 3, "4", Color.red);
            SkillNodeUI nodeS2_Top = CreateNode(strengthContainer.transform, "Strength_C2_Top", new Vector2(-100f, 120f), true, 0, 3, "10", Color.red);
            SkillNodeUI nodeS2_Bot = CreateNode(strengthContainer.transform, "Strength_C2_Bot", new Vector2(-100f, -120f), true, 0, 3, "10", Color.red);
            SkillNodeUI nodeS2_BotB = CreateNode(strengthContainer.transform, "Strength_C2_Bot_Bot", new Vector2(-20f, -200f), true, 0, 3, "10", Color.red);

            SkillNodeUI nodeS3_Mid = CreateNode(strengthContainer.transform, "Strength_C3_Mid", new Vector2(140f, 0f), true, 0, 1, "30", Color.blue);

            SkillNodeUI nodeSLockCLeft  = CreateNode(strengthContainer.transform, "StrengthLockCenterLeft",  new Vector2(260f, 0f), true, 0, 1, "5", Color.magenta);
            SkillNodeUI nodeSLockCRight = CreateNode(strengthContainer.transform, "StrengthLockCenterRight", new Vector2(380f, 0f), true, 0, 1, "6", Color.magenta);
            SkillNodeUI nodeSLockFarR   = CreateNode(strengthContainer.transform, "StrengthLockFarRight",    new Vector2(500f, 0f), true, 0, 1, "60", Color.red);
            
            SkillNodeUI nodeSLockTLeft  = CreateNode(strengthContainer.transform, "StrengthLockTopLeft",     new Vector2(260f, 120f), true, 0, 1, "15", Color.yellow);
            SkillNodeUI nodeSLockBLeft  = CreateNode(strengthContainer.transform, "StrengthLockBottomLeft",  new Vector2(260f, -120f), true, 0, 1, "10", Color.yellow);
            SkillNodeUI nodeSLockTRight = CreateNode(strengthContainer.transform, "StrengthLockTopRight",    new Vector2(380f, 120f), true, 0, 1, "3", Color.yellow);

            // --- CONFIGURAR DEPENDENCIAS STRENGTH ---
            strengthAwaken.childNodes = new[] { nodeS1_Mid };
            
            nodeS1_Mid.parentNodes = new[] { strengthAwaken };
            nodeS1_Mid.childNodes = new[] { nodeS1_Top, nodeS1_Bot, nodeS2_Mid };

            nodeS1_Top.parentNodes = new[] { nodeS1_Mid };
            nodeS1_Top.childNodes = new[] { nodeS1_TopT };
            nodeS1_TopT.parentNodes = new[] { nodeS1_Top };

            nodeS1_Bot.parentNodes = new[] { nodeS1_Mid };
            nodeS1_Bot.childNodes = new[] { nodeS1_BotB };
            nodeS1_BotB.parentNodes = new[] { nodeS1_Bot };

            nodeS2_Mid.parentNodes = new[] { nodeS1_Mid };
            nodeS2_Mid.childNodes = new[] { nodeS2_Top, nodeS2_Bot, nodeS3_Mid };

            nodeS2_Top.parentNodes = new[] { nodeS2_Mid };

            nodeS2_Bot.parentNodes = new[] { nodeS2_Mid };
            nodeS2_Bot.childNodes = new[] { nodeS2_BotB };
            nodeS2_BotB.parentNodes = new[] { nodeS2_Bot };

            nodeS3_Mid.parentNodes = new[] { nodeS2_Mid };
            nodeS3_Mid.childNodes = new[] { nodeSLockCLeft };

            nodeSLockCLeft.parentNodes = new[] { nodeS3_Mid };
            nodeSLockCLeft.childNodes = new[] { nodeSLockTLeft, nodeSLockBLeft, nodeSLockCRight };

            nodeSLockTLeft.parentNodes = new[] { nodeSLockCLeft };
            nodeSLockBLeft.parentNodes = new[] { nodeSLockCLeft };

            nodeSLockCRight.parentNodes = new[] { nodeSLockCLeft };
            nodeSLockCRight.childNodes = new[] { nodeSLockTRight, nodeSLockFarR };

            nodeSLockTRight.parentNodes = new[] { nodeSLockCRight };
            nodeSLockFarR.parentNodes = new[] { nodeSLockCRight };


            // --- 9d. CONSTRUIR LA LÍNEA DE MAESTRÍA DE "SOUL KING" (TECHNIQUE) ---
            GameObject techContainer = tabContainers[3];
            CreateStyleHeaderPanel(techContainer.transform, "Soul King", "585/1000", new Color(0.85f, 0.15f, 0.15f));

            SkillNodeUI techM1   = CreateMasteryNode(techContainer.transform, "TechMastery1",   new Vector2(-180f, 0f), false, 1, 1, "(Mastery 1)",   "", new Color(0.85f, 0.15f, 0.15f));
            SkillNodeUI techM50  = CreateMasteryNode(techContainer.transform, "TechMastery50",  new Vector2(-90f, 0f),  false, 1, 1, "(Mastery 50)",  "", new Color(0.85f, 0.15f, 0.15f));
            SkillNodeUI techM100 = CreateMasteryNode(techContainer.transform, "TechMastery100", new Vector2(0f, 0f),    false, 1, 1, "(Mastery 100)", "", new Color(0.85f, 0.15f, 0.15f));
            SkillNodeUI techM150 = CreateMasteryNode(techContainer.transform, "TechMastery150", new Vector2(90f, 0f),   false, 1, 1, "(Mastery 150)", "", new Color(0.85f, 0.15f, 0.15f));
            SkillNodeUI techM200 = CreateMasteryNode(techContainer.transform, "TechMastery200", new Vector2(180f, 0f),  false, 1, 1, "(Mastery 200)", "", new Color(0.85f, 0.15f, 0.15f));
            SkillNodeUI techM250 = CreateMasteryNode(techContainer.transform, "TechMastery250", new Vector2(270f, 0f),  true,  0, 1, "(Mastery 250)", "", new Color(0.85f, 0.15f, 0.15f));
            SkillNodeUI techM300 = CreateMasteryNode(techContainer.transform, "TechMastery300", new Vector2(360f, 0f),  true,  0, 1, "(Mastery 300)", "", new Color(0.85f, 0.15f, 0.15f));

            // Conexiones
            techM1.childNodes = new[] { techM50 };
            techM50.parentNodes = new[] { techM1 };
            techM50.childNodes = new[] { techM100 };
            techM100.parentNodes = new[] { techM50 };
            techM100.childNodes = new[] { techM150 };
            techM150.parentNodes = new[] { techM100 };
            techM150.childNodes = new[] { techM200 };
            techM200.parentNodes = new[] { techM150 };
            techM200.childNodes = new[] { techM250 };
            techM250.parentNodes = new[] { techM200 };
            techM250.childNodes = new[] { techM300 };
            techM300.parentNodes = new[] { techM250 };

            // --- 11. ASIGNAR REFERENCIAS EN EL MANAGER ---
            SerializedObject so = new SerializedObject(skillTreeUI);
            
            // Pestañas
            SerializedProperty propButtons = so.FindProperty("tabButtons");
            propButtons.arraySize = 4;
            for (int i = 0; i < 4; i++)
            {
                propButtons.GetArrayElementAtIndex(i).objectReferenceValue = tabButtons[i];
            }

            SerializedProperty propContainers = so.FindProperty("tabContainers");
            propContainers.arraySize = 4;
            for (int i = 0; i < 4; i++)
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

        private static void CreateStyleHeaderPanel(Transform parent, string styleName, string expStr, Color themeColor)
        {
            // Create a panel on the left: X = -380f, Y = 0f, Size = 240f, 150f
            GameObject panel = CreateUIObject(styleName + "_HeaderPanel", parent, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-360f, 0f), new Vector2(210f, 140f));
            CreateImage("Bg", panel.transform, new Color(0.04f, 0.04f, 0.08f, 0.95f));
            CreateOutline("Outline", panel.transform, themeColor, 1.5f);

            // Icon placeholder
            GameObject iconObj = CreateUIObject("Icon", panel.transform, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(35f, 0f), new Vector2(50f, 50f));
            CreateImage("IconBg", iconObj.transform, new Color(0.1f, 0.1f, 0.15f));
            CreateOutline("IconOutline", iconObj.transform, themeColor, 1f);
            
            // Name Text
            TextMeshProUGUI nameTxt = CreateText("NameText", panel.transform, styleName, 18, themeColor, TextAlignmentOptions.Left);
            nameTxt.rectTransform.anchoredPosition = new Vector2(125f, 35f);
            nameTxt.fontStyle = FontStyles.Bold;

            // Mastery text
            TextMeshProUGUI masteryTxt = CreateText("MasteryText", panel.transform, "(Mastery 500)", 10, new Color(0.25f, 0.75f, 0.75f), TextAlignmentOptions.Left);
            masteryTxt.rectTransform.anchoredPosition = new Vector2(125f, 12f);

            // Experience Text
            TextMeshProUGUI expTxt = CreateText("ExpText", panel.transform, expStr, 10, Color.white, TextAlignmentOptions.Left);
            expTxt.rectTransform.anchoredPosition = new Vector2(125f, -10f);

            // Simple experience bar container
            GameObject barContainer = CreateUIObject("ExpBar", panel.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(20f, -38f), new Vector2(150f, 8f));
            CreateImage("Bg", barContainer.transform, new Color(0.05f, 0.05f, 0.08f));
            
            GameObject fill = CreateUIObject("Fill", barContainer.transform, Vector2.zero, Vector2.one, new Vector2(0f, 0.5f), Vector2.zero, Vector2.zero);
            Image fillImg = fill.AddComponent<Image>();
            fillImg.color = themeColor;
            
            // Parse progress from expStr (e.g. "585/1000")
            float fillAmount = 0f;
            if (expStr.Contains("/"))
            {
                string[] parts = expStr.Split('/');
                if (float.TryParse(parts[0], out float current) && float.TryParse(parts[1], out float total) && total > 0f)
                {
                    fillAmount = current / total;
                }
            }
            RectTransform fillRect = fill.GetComponent<RectTransform>();
            fillRect.anchorMax = new Vector2(fillAmount, 1f);
        }

        private static SkillNodeUI CreateMasteryNode(Transform parent, string name, Vector2 pos, bool isLocked, int level, int maxLevel, string masteryStr, string bottomLabelStr, Color activeColor)
        {
            // Contenedor principal del nodo
            GameObject nodeContainer = CreateUIObject(name, parent, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), pos, new Vector2(75f, 85f));

            // Botón del nodo (Rombo rotado a 45 grados)
            GameObject diamondButtonObj = CreateUIObject("DiamondButton", nodeContainer.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 0f), new Vector2(50f, 50f));
            diamondButtonObj.transform.localRotation = Quaternion.Euler(0f, 0f, 45f);
            
            Image outlineImg = diamondButtonObj.AddComponent<Image>();
            outlineImg.color = activeColor;
            Button button = diamondButtonObj.AddComponent<Button>();

            // Imagen de Relleno Interno
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
            lockTxt.text = "🔒";
            lockTxt.fontSize = 20;
            lockTxt.alignment = TextAlignmentOptions.Center;
            lockTxt.color = Color.white;
            lockTxt.raycastTarget = false;
            
            // Resalte de selección
            GameObject highlightObj = CreateUIObject("Highlight", diamondButtonObj.transform, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(10f, 10f));
            Image highImg = highlightObj.AddComponent<Image>();
            highImg.color = Color.clear;
            Outline highOutline = highlightObj.AddComponent<Outline>();
            highOutline.effectColor = Color.white;
            highOutline.effectDistance = new Vector2(3f, 3f);
            highlightObj.SetActive(false);

            // Texto superior de maestría
            GameObject topLabelObj = CreateUIObject("MasteryText", nodeContainer.transform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 0.5f), new Vector2(0f, 35f), new Vector2(120f, 20f));
            TextMeshProUGUI topText = topLabelObj.AddComponent<TextMeshProUGUI>();
            topText.text = masteryStr;
            topText.fontSize = 10;
            topText.alignment = TextAlignmentOptions.Center;
            topText.color = new Color(0.7f, 0.7f, 0.7f);
            topText.fontStyle = FontStyles.Italic;
            topText.raycastTarget = false;

            // Texto inferior (coste o etiqueta especial)
            string displayCost = !string.IsNullOrEmpty(bottomLabelStr) ? bottomLabelStr : "1";
            GameObject bottomLabelObj = CreateUIObject("LevelText", nodeContainer.transform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0.5f), new Vector2(0f, -38f), new Vector2(120f, 30f));
            TextMeshProUGUI costText = bottomLabelObj.AddComponent<TextMeshProUGUI>();
            costText.text = displayCost;
            costText.fontSize = 9;
            costText.alignment = TextAlignmentOptions.Center;
            costText.color = activeColor;
            costText.fontStyle = FontStyles.Bold;
            costText.raycastTarget = false;

            // Registrar script
            SkillNodeUI skillNode = nodeContainer.AddComponent<SkillNodeUI>();
            SerializedObject soNode = new SerializedObject(skillNode);
            
            soNode.FindProperty("nodeName").stringValue = name;
            soNode.FindProperty("isLocked").boolValue = isLocked;
            soNode.FindProperty("currentLevel").intValue = level;
            soNode.FindProperty("maxLevel").intValue = maxLevel;
            soNode.FindProperty("costValueString").stringValue = displayCost;
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
