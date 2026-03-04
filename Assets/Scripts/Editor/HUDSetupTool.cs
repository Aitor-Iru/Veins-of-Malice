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
            essenceRect.sizeDelta = new Vector2(150, 50);

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

            // Background / Dim
            GameObject bgObj = new GameObject("Background_Dim", typeof(RectTransform), typeof(Image));
            bgObj.transform.SetParent(invObj.transform, false);
            bgObj.GetComponent<Image>().color = new Color(0, 0, 0, 0.6f);
            bgObj.GetComponent<RectTransform>().sizeDelta = new Vector2(2000, 2000); // Overlay

            // Main Panel
            GameObject panelObj = new GameObject("Panel", typeof(RectTransform), typeof(Image));
            panelObj.transform.SetParent(invObj.transform, false);
            RectTransform panelRect = panelObj.GetComponent<RectTransform>();
            panelRect.sizeDelta = new Vector2(600, 400);
            panelObj.GetComponent<Image>().color = new Color(0.15f, 0.1f, 0.2f); // Dark purple

            // Title
            GameObject titleObj = new GameObject("Title", typeof(RectTransform), typeof(TextMeshProUGUI));
            titleObj.transform.SetParent(panelObj.transform, false);
            TextMeshProUGUI titleText = titleObj.GetComponent<TextMeshProUGUI>();
            titleText.text = "INVENTARIO";
            titleText.fontSize = 32;
            titleText.alignment = TextAlignmentOptions.Center;
            titleObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 150);

            // Essence Count in Inventory
            GameObject essObj = new GameObject("EssenceCount", typeof(RectTransform), typeof(TextMeshProUGUI));
            essObj.transform.SetParent(panelObj.transform, false);
            TextMeshProUGUI essText = essObj.GetComponent<TextMeshProUGUI>();
            essText.text = "Esencia Maldita: 0";
            essText.fontSize = 24;
            essText.alignment = TextAlignmentOptions.Center;
            
            // Logic
            InventoryUI invUI = invObj.AddComponent<InventoryUI>();
            SerializedObject so = new SerializedObject(invUI);
            so.FindProperty("inventoryPanel").objectReferenceValue = invObj;
            so.FindProperty("essenceText").objectReferenceValue = essText;
            
            // Try assign InputReader manually if possible, usually better via Inspector
            so.ApplyModifiedProperties();

            invObj.SetActive(false);
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
