using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using VeinsOfMalice.UI;

namespace VeinsOfMalice.Editor
{
    public class HUDSetupTool : EditorWindow
    {
        [MenuItem("Tools/Veins of Malice/Setup HUD and Feedback (The Robot)")]
        public static void SetupProjectUI()
        {
            // 0. Clean up existing HUD if present
            GameObject existingHUD = GameObject.Find("HUD_Canvas");
            if (existingHUD != null) DestroyImmediate(existingHUD);
            GameObject existingDmg = GameObject.Find("DamageNumberManager");
            if (existingDmg != null) DestroyImmediate(existingDmg);

            // 1. Create Canvas
            GameObject canvasObj = new GameObject("HUD_Canvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            
            // 2. Create EventSystem
            if (FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem), typeof(UnityEngine.EventSystems.StandaloneInputModule));
            }

            // 3. Create Container
            GameObject hudPanel = new GameObject("HUD_Panel", typeof(RectTransform));
            hudPanel.transform.SetParent(canvasObj.transform, false);
            RectTransform panelRect = hudPanel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0, 1);
            panelRect.anchorMax = new Vector2(0, 1);
            panelRect.pivot = new Vector2(0, 1);
            panelRect.anchoredPosition = new Vector2(20, -20);
            panelRect.sizeDelta = new Vector2(300, 100);

            // 4. Create Health Bar
            Slider healthSlider = CreateBar(hudPanel.transform, "HealthBar", Color.red, new Vector2(0, 0));
            Slider ghostSlider = CreateBar(hudPanel.transform, "HealthGhost", new Color(1, 0.5f, 0.5f), new Vector2(0, 0));
            ghostSlider.transform.SetSiblingIndex(0); // Put ghost behind

            // 5. Create Energy Bar
            Slider energySlider = CreateBar(hudPanel.transform, "EnergyBar", Color.blue, new Vector2(0, -40));

            // 6. Create Essence Counter (Top-Right)
            GameObject essenceContainer = new GameObject("Essence_Counter", typeof(RectTransform));
            essenceContainer.transform.SetParent(canvasObj.transform, false);
            RectTransform essenceRect = essenceContainer.GetComponent<RectTransform>();
            essenceRect.anchorMin = new Vector2(1, 1);
            essenceRect.anchorMax = new Vector2(1, 1);
            essenceRect.pivot = new Vector2(1, 1);
            essenceRect.anchoredPosition = new Vector2(-20, -20);
            essenceRect.sizeDelta = new Vector2(160, 50);

            // Background for Essence
            GameObject essBgObj = new GameObject("Background", typeof(RectTransform), typeof(Image));
            essBgObj.transform.SetParent(essenceContainer.transform, false);
            RectTransform essBgRect = essBgObj.GetComponent<RectTransform>();
            essBgRect.anchorMin = Vector2.zero;
            essBgRect.anchorMax = Vector2.one;
            essBgRect.sizeDelta = Vector2.zero;
            Image essBgImg = essBgObj.GetComponent<Image>();
            essBgImg.color = new Color(0, 0, 0, 0.4f); // Semi-transparent black

            // Icon
            GameObject iconObj = new GameObject("Icon", typeof(RectTransform), typeof(Image));
            iconObj.transform.SetParent(essenceContainer.transform, false);
            RectTransform iconRect = iconObj.GetComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(1, 0.5f);
            iconRect.anchorMax = new Vector2(1, 0.5f);
            iconRect.pivot = new Vector2(1, 0.5f);
            iconRect.anchoredPosition = Vector2.zero;
            iconRect.sizeDelta = new Vector2(40, 40);
            Image iconImg = iconObj.GetComponent<Image>();
            iconImg.color = new Color(0.8f, 0.2f, 1f); // Fallback purple

            // Text
            GameObject textObj = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            textObj.transform.SetParent(essenceContainer.transform, false);
            TextMeshProUGUI essenceText = textObj.GetComponent<TextMeshProUGUI>();
            essenceText.text = "0";
            essenceText.fontSize = 24;
            essenceText.alignment = TextAlignmentOptions.Right;
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0, 0);
            textRect.anchorMax = new Vector2(1, 1);
            textRect.offsetMax = new Vector2(-50, 0); // Leave space for icon
            textRect.offsetMin = Vector2.zero;

            // 7. Setup HUDManager
            HUDManager hudManager = canvasObj.AddComponent<HUDManager>();
            SerializedObject so = new SerializedObject(hudManager);
            so.FindProperty("healthSlider").objectReferenceValue = healthSlider;
            so.FindProperty("healthGhostSlider").objectReferenceValue = ghostSlider;
            so.FindProperty("energySlider").objectReferenceValue = energySlider;
            so.FindProperty("essenceText").objectReferenceValue = essenceText;
            so.ApplyModifiedProperties();

            // 8. Setup DamageNumberManager
            GameObject dmgObj = new GameObject("DamageNumberManager");
            DamageNumberManager dmgManager = dmgObj.AddComponent<DamageNumberManager>();
            
            // Create a simple TMP prefab/template
            GameObject tmpTemplate = new GameObject("DamageText_Template", typeof(TextMeshPro));
            tmpTemplate.transform.SetParent(dmgObj.transform);
            tmpTemplate.SetActive(false);
            TextMeshPro text = tmpTemplate.GetComponent<TextMeshPro>();
            text.alignment = TextAlignmentOptions.Center;
            text.fontSize = 6;
            // text.outlineWidth = 0.2f; // Removed to avoid NullReferenceException in some environments

            SerializedObject soDmg = new SerializedObject(dmgManager);
            soDmg.FindProperty("damageTextPrefab").objectReferenceValue = tmpTemplate;
            soDmg.ApplyModifiedProperties();

            // 9. Create Inventory UI
            CreateInventoryPanel(canvasObj.transform);

            Debug.Log("<color=green>[The Robot]</color> HUD and Inventory setup complete!");
        }

        private static void CreateInventoryPanel(Transform parent)
        {
            GameObject invObj = new GameObject("Inventory_Canvas", typeof(RectTransform));
            invObj.transform.SetParent(parent, false);
            RectTransform invRect = invObj.GetComponent<RectTransform>();
            invRect.anchorMin = Vector2.zero;
            invRect.anchorMax = Vector2.one;
            invRect.sizeDelta = Vector2.zero;

            // Content Container (Everything that shows/hides)
            GameObject contentObj = new GameObject("Content", typeof(RectTransform));
            contentObj.transform.SetParent(invObj.transform, false);
            RectTransform contentRect = contentObj.GetComponent<RectTransform>();
            contentRect.anchorMin = Vector2.zero;
            contentRect.anchorMax = Vector2.one;
            contentRect.sizeDelta = Vector2.zero;

            // Background / Dim
            GameObject bgObj = new GameObject("Background_Dim", typeof(RectTransform), typeof(Image));
            bgObj.transform.SetParent(contentObj.transform, false);
            bgObj.GetComponent<Image>().color = new Color(0, 0, 0, 0.75f);
            bgObj.GetComponent<RectTransform>().sizeDelta = new Vector2(3000, 3000); // Massive overlay

            // Main Panel
            GameObject panelObj = new GameObject("Panel", typeof(RectTransform), typeof(Image));
            panelObj.transform.SetParent(contentObj.transform, false);
            RectTransform panelRect = panelObj.GetComponent<RectTransform>();
            panelRect.sizeDelta = new Vector2(600, 400);
            panelObj.GetComponent<Image>().color = new Color(0.1f, 0.08f, 0.15f, 1f); // Solid dark purple

            // Title
            GameObject titleObj = new GameObject("Title", typeof(RectTransform), typeof(TextMeshProUGUI));
            titleObj.transform.SetParent(panelObj.transform, false);
            TextMeshProUGUI titleText = titleObj.GetComponent<TextMeshProUGUI>();
            titleText.text = "INVENTARIO";
            titleText.fontSize = 28;
            titleText.alignment = TextAlignmentOptions.Center;
            titleObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 165);

            // Removing the bottom Essence counter as requested by user - it should be inside slots
            /*
            GameObject essObj = new GameObject("EssenceCount", typeof(RectTransform), typeof(TextMeshProUGUI));
            ...
            */

            // Grid Container
            GameObject gridObj = new GameObject("Grid", typeof(RectTransform), typeof(GridLayoutGroup));
            gridObj.transform.SetParent(panelObj.transform, false);
            RectTransform gridRect = gridObj.GetComponent<RectTransform>();
            gridRect.sizeDelta = new Vector2(500, 250);
            gridRect.anchoredPosition = new Vector2(0, 0);

            GridLayoutGroup grid = gridObj.GetComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(70, 70);
            grid.spacing = new Vector2(10, 10);
            grid.startAxis = GridLayoutGroup.Axis.Horizontal;
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 6;
            grid.childAlignment = TextAnchor.MiddleCenter;

            // Create 24 Slots (4 rows of 6)
            for (int i = 0; i < 24; i++)
            {
                GameObject slotObj = new GameObject($"Slot_{i}", typeof(RectTransform), typeof(Image));
                slotObj.transform.SetParent(gridObj.transform, false);
                slotObj.GetComponent<Image>().color = new Color(0.15f, 0.1f, 0.25f, 1f); // More visible slot background

                // Empty Graphic
                GameObject emptyObj = new GameObject("EmptyGraphic", typeof(RectTransform), typeof(Image));
                emptyObj.transform.SetParent(slotObj.transform, false);
                RectTransform emptyRect = emptyObj.GetComponent<RectTransform>();
                emptyRect.anchorMin = Vector2.zero;
                emptyRect.anchorMax = Vector2.one;
                emptyRect.sizeDelta = new Vector2(-20, -20);
                Image emptyImg = emptyObj.GetComponent<Image>();
                emptyImg.color = new Color(1, 1, 1, 0.15f); // Increased visibility

                // Icon Object
                GameObject iconObj = new GameObject("Icon", typeof(RectTransform), typeof(Image));
                iconObj.transform.SetParent(slotObj.transform, false);
                RectTransform iconRect = iconObj.GetComponent<RectTransform>();
                iconRect.anchorMin = Vector2.zero;
                iconRect.anchorMax = Vector2.one;
                iconRect.sizeDelta = new Vector2(-15, -15);
                Image iconImg = iconObj.GetComponent<Image>();
                iconImg.preserveAspect = true;
                iconImg.enabled = false; // Starts hidden

                // Name Text (Inside Slot)
                GameObject nameObj = new GameObject("ItemName", typeof(RectTransform), typeof(TextMeshProUGUI));
                nameObj.transform.SetParent(slotObj.transform, false);
                TextMeshProUGUI nameTxt = nameObj.GetComponent<TextMeshProUGUI>();
                nameTxt.fontSize = 8;
                nameTxt.alignment = TextAlignmentOptions.Center;
                nameTxt.color = Color.white;
                nameTxt.enableWordWrapping = false;
                nameTxt.overflowMode = TextOverflowModes.Ellipsis;
                RectTransform nameRect = nameObj.GetComponent<RectTransform>();
                nameRect.anchorMin = new Vector2(0, 0.05f);
                nameRect.anchorMax = new Vector2(1, 0.35f);
                nameRect.sizeDelta = Vector2.zero;

                // Amount Text (Inside Slot)
                GameObject amtObj = new GameObject("ItemAmount", typeof(RectTransform), typeof(TextMeshProUGUI));
                amtObj.transform.SetParent(slotObj.transform, false);
                TextMeshProUGUI amtTxt = amtObj.GetComponent<TextMeshProUGUI>();
                amtTxt.fontSize = 10;
                amtTxt.alignment = TextAlignmentOptions.Right;
                amtTxt.color = Color.yellow;
                RectTransform amtRect = amtObj.GetComponent<RectTransform>();
                amtRect.anchorMin = new Vector2(0.5f, 0.6f);
                amtRect.anchorMax = new Vector2(0.95f, 0.95f);
                amtRect.sizeDelta = Vector2.zero;

                InventorySlotUI slotUI = slotObj.AddComponent<InventorySlotUI>();
                
                SerializedObject soSlot = new SerializedObject(slotUI);
                soSlot.FindProperty("iconImage").objectReferenceValue = iconImg;
                soSlot.FindProperty("emptyGraphic").objectReferenceValue = emptyObj;
                soSlot.FindProperty("itemNameText").objectReferenceValue = nameTxt;
                soSlot.FindProperty("itemAmountText").objectReferenceValue = amtTxt;
                soSlot.ApplyModifiedProperties();
            }
            
            // Logic
            InventoryUI invUI = invObj.AddComponent<InventoryUI>();
            SerializedObject so = new SerializedObject(invUI);
            so.FindProperty("inventoryPanel").objectReferenceValue = contentObj; 
            so.FindProperty("essenceText").objectReferenceValue = null; 
            
            // Assign InputReader automatically
            InputReader ir = AssetDatabase.LoadAssetAtPath<InputReader>("Assets/Input/InputReader.asset");
            if (ir != null)
            {
                so.FindProperty("inputReader").objectReferenceValue = ir;
            }
            else
            {
                Debug.LogWarning("[HUDSetupTool] InputReader asset NOT found at 'Assets/Input/InputReader.asset'. Manual assignment required.");
            }
            
            so.ApplyModifiedProperties();

            // Hide the entire content container at start
            contentObj.SetActive(false);
            
            invObj.SetActive(true); 
        }

        private static Slider CreateBar(Transform parent, string name, Color color, Vector2 anchoredPos)
        {
            GameObject barObj = new GameObject(name, typeof(RectTransform), typeof(Slider));
            barObj.transform.SetParent(parent, false);
            Slider slider = barObj.GetComponent<Slider>();
            
            RectTransform rect = barObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = anchoredPos;
            rect.sizeDelta = new Vector2(250, 25);

            // Background
            GameObject bgObj = new GameObject("Background", typeof(Image));
            bgObj.transform.SetParent(barObj.transform, false);
            Image bgImg = bgObj.GetComponent<Image>();
            bgImg.color = new Color(0,0,0,0.5f);
            RectTransform bgRect = bgObj.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;

            // Fill Area
            GameObject fillArea = new GameObject("Fill Area", typeof(RectTransform));
            fillArea.transform.SetParent(barObj.transform, false);
            RectTransform fillAreaRect = fillArea.GetComponent<RectTransform>();
            fillAreaRect.anchorMin = Vector2.zero;
            fillAreaRect.anchorMax = Vector2.one;
            fillAreaRect.sizeDelta = new Vector2(-10, -10);

            // Fill
            GameObject fillObj = new GameObject("Fill", typeof(Image));
            fillObj.transform.SetParent(fillArea.transform, false);
            Image fillImg = fillObj.GetComponent<Image>();
            fillImg.color = color;
            RectTransform fillRect = fillObj.GetComponent<RectTransform>();
            fillRect.sizeDelta = Vector2.zero;

            slider.targetGraphic = fillImg;
            slider.fillRect = fillRect;
            slider.interactable = false;
            slider.transition = Selectable.Transition.None;
            slider.navigation = new Navigation { mode = Navigation.Mode.None };

            return slider;
        }
    }
}
