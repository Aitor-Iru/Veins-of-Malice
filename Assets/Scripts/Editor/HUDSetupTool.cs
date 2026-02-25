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

            // 6. Setup HUDManager
            HUDManager hudManager = canvasObj.AddComponent<HUDManager>();
            // Using Reflection or manual assignment if the fields were public. 
            // In my implementation they are serialized, so we'd usually use SerializedObject.
            SerializedObject so = new SerializedObject(hudManager);
            so.FindProperty("healthSlider").objectReferenceValue = healthSlider;
            so.FindProperty("healthGhostSlider").objectReferenceValue = ghostSlider;
            so.FindProperty("energySlider").objectReferenceValue = energySlider;
            so.ApplyModifiedProperties();

            // 7. Setup DamageNumberManager
            GameObject dmgObj = new GameObject("DamageNumberManager");
            DamageNumberManager dmgManager = dmgObj.AddComponent<DamageNumberManager>();
            
            // Create a simple TMP prefab/template
            GameObject tmpTemplate = new GameObject("DamageText_Template", typeof(TextMeshPro));
            tmpTemplate.transform.SetParent(dmgObj.transform);
            tmpTemplate.SetActive(false);
            TextMeshPro text = tmpTemplate.GetComponent<TextMeshPro>();
            text.alignment = TextAlignmentOptions.Center;
            text.fontSize = 6;
            text.outlineWidth = 0.2f;

            SerializedObject soDmg = new SerializedObject(dmgManager);
            soDmg.FindProperty("damageTextPrefab").objectReferenceValue = tmpTemplate;
            soDmg.ApplyModifiedProperties();

            Debug.Log("<color=green>[The Robot]</color> HUD and Feedback setup complete! Check the new 'HUD_Canvas' and 'DamageNumberManager' in your scene.");
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
