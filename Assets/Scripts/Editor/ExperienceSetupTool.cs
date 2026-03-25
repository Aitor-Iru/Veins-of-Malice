using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using VeinsOfMalice.Player;
using VeinsOfMalice.UI;

namespace VeinsOfMalice.EditorTools
{
    public class ExperienceSetupTool : EditorWindow
    {
        [MenuItem("Tools/Veins of Malice/UI Generators/3. Configurar Barra de Experiencia")]
        public static void SetupExperienceSystem()
        {
            // 1. Configurar Kairo (el Jugador)
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                // Si no hay tag Player, buscar por Inventory
                var inv = Object.FindFirstObjectByType<PlayerInventory>();
                if (inv != null) player = inv.gameObject;
            }

            if (player != null)
            {
                PlayerExperience exp = player.GetComponent<PlayerExperience>();
                if (exp == null)
                {
                    exp = player.AddComponent<PlayerExperience>();
                    Debug.Log("<color=green>[Robot XP]</color> Añadido PlayerExperience al Jugador.");
                }
            }
            else
            {
                Debug.LogWarning("<color=orange>[Robot XP]</color> No pude encontrar el Jugador en la escena actual.");
            }

            // 2. Buscar el Panel de Pausa
            GameUIManager uiManager = Object.FindFirstObjectByType<GameUIManager>();
            if (uiManager == null)
            {
                Debug.LogError("<color=red>[Robot XP]</color> No he encontrado el GameUIManager en la escena. ¡Abre la escena donde está o usa primero el bot de construir los Menús de Pausa!");
                return;
            }

            SerializedObject soManager = new SerializedObject(uiManager);
            GameObject pausePanel = soManager.FindProperty("pausePanel").objectReferenceValue as GameObject;

            if (pausePanel == null)
            {
                Debug.LogError("<color=red>[Robot XP]</color> GameUIManager no tiene un PausePanel asignado.");
                return;
            }

            // 3. Crear Componentes UI en el PausePanel
            // Eliminar si ya existía para no duplicar
            Transform oldXP = pausePanel.transform.Find("XP_Panel_Auto");
            if (oldXP != null) DestroyImmediate(oldXP.gameObject);

            GameObject xpPanel = new GameObject("XP_Panel_Auto", typeof(RectTransform));
            xpPanel.transform.SetParent(pausePanel.transform, false);
            RectTransform xpRect = xpPanel.GetComponent<RectTransform>();
            xpRect.anchorMin = new Vector2(0.5f, 0);
            xpRect.anchorMax = new Vector2(0.5f, 0);
            xpRect.pivot = new Vector2(0.5f, 0);
            xpRect.anchoredPosition = new Vector2(0, 50); // Abajo del menu de pausa
            xpRect.sizeDelta = new Vector2(400, 80);

            // Level Text
            GameObject lvlObj = new GameObject("LevelText", typeof(RectTransform), typeof(TextMeshProUGUI));
            lvlObj.transform.SetParent(xpPanel.transform, false);
            RectTransform lvlRect = lvlObj.GetComponent<RectTransform>();
            lvlRect.anchorMin = new Vector2(0, 0.5f);
            lvlRect.anchorMax = new Vector2(1, 1f);
            lvlRect.sizeDelta = Vector2.zero;
            TextMeshProUGUI lvlText = lvlObj.GetComponent<TextMeshProUGUI>();
            lvlText.text = "Level: 1";
            lvlText.fontSize = 28;
            lvlText.alignment = TextAlignmentOptions.Center;
            lvlText.color = new Color(0.8f, 0.3f, 0.9f); // Morado oscuro/magia
            lvlText.enableWordWrapping = false;

            // Slider Bar
            GameObject barObj = new GameObject("XP_Slider", typeof(RectTransform), typeof(Slider));
            barObj.transform.SetParent(xpPanel.transform, false);
            RectTransform barRect = barObj.GetComponent<RectTransform>();
            barRect.anchorMin = new Vector2(0, 0);
            barRect.anchorMax = new Vector2(1, 0.5f);
            barRect.sizeDelta = new Vector2(-40, -10); // Margenes
            barRect.anchoredPosition = new Vector2(0, 5);
            Slider slider = barObj.GetComponent<Slider>();
            slider.interactable = false;
            slider.transition = Selectable.Transition.None;

            // Slider Background
            GameObject bgObj = new GameObject("Background", typeof(RectTransform), typeof(Image));
            bgObj.transform.SetParent(barObj.transform, false);
            RectTransform bgRect = bgObj.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;
            bgObj.GetComponent<Image>().color = new Color(0, 0, 0, 0.5f);

            // Slider Fill Area
            GameObject fillArea = new GameObject("Fill Area", typeof(RectTransform));
            fillArea.transform.SetParent(barObj.transform, false);
            RectTransform fillAreaRect = fillArea.GetComponent<RectTransform>();
            fillAreaRect.anchorMin = Vector2.zero;
            fillAreaRect.anchorMax = Vector2.one;
            fillAreaRect.sizeDelta = new Vector2(-10, -10);

            // Slider Fill
            GameObject fillObj = new GameObject("Fill", typeof(RectTransform), typeof(Image));
            fillObj.transform.SetParent(fillArea.transform, false);
            RectTransform fillRect = fillObj.GetComponent<RectTransform>();
            fillRect.sizeDelta = Vector2.zero;
            Image fillImg = fillObj.GetComponent<Image>();
            fillImg.color = new Color(0.6f, 0.1f, 0.9f); // Morado brillante

            slider.targetGraphic = fillImg;
            slider.fillRect = fillRect;

            // XP Numbers inside Slider
            GameObject numObj = new GameObject("XPText", typeof(RectTransform), typeof(TextMeshProUGUI));
            numObj.transform.SetParent(barObj.transform, false);
            RectTransform numRect = numObj.GetComponent<RectTransform>();
            numRect.anchorMin = Vector2.zero;
            numRect.anchorMax = Vector2.one;
            numRect.sizeDelta = Vector2.zero;
            TextMeshProUGUI numText = numObj.GetComponent<TextMeshProUGUI>();
            numText.text = "0 / 100";
            numText.fontSize = 18;
            numText.alignment = TextAlignmentOptions.Center;
            numText.color = Color.white;
            numText.enableWordWrapping = false;

            // Rebirth Button
            GameObject rbBtnObj = new GameObject("Btn_Rebirth", typeof(RectTransform), typeof(Image), typeof(Button));
            rbBtnObj.transform.SetParent(xpPanel.transform, false);
            RectTransform rbRect = rbBtnObj.GetComponent<RectTransform>();
            rbRect.anchorMin = new Vector2(0.5f, 1.8f); // Por encima de la barra
            rbRect.anchorMax = new Vector2(0.5f, 1.8f);
            rbRect.pivot = new Vector2(0.5f, 0.5f);
            rbRect.sizeDelta = new Vector2(300, 40);
            rbRect.anchoredPosition = new Vector2(0, 0);

            Image rbImg = rbBtnObj.GetComponent<Image>();
            rbImg.color = new Color(0.9f, 0.1f, 0.3f, 1f); // Rojo/Naranja
            Button rbBtn = rbBtnObj.GetComponent<Button>();

            GameObject rbTxtObj = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            rbTxtObj.transform.SetParent(rbBtnObj.transform, false);
            RectTransform rbTxtRect = rbTxtObj.GetComponent<RectTransform>();
            rbTxtRect.anchorMin = Vector2.zero;
            rbTxtRect.anchorMax = Vector2.one;
            rbTxtRect.sizeDelta = Vector2.zero;
            TextMeshProUGUI rbText = rbTxtObj.GetComponent<TextMeshProUGUI>();
            rbText.text = "RENACIMIENTO (Desbloquear Nivel)";
            rbText.fontSize = 20;
            rbText.alignment = TextAlignmentOptions.Center;
            rbText.color = Color.white;
            rbText.enableWordWrapping = false;
            rbText.raycastTarget = false;
            
            rbBtnObj.SetActive(false); // Oculto por defecto

            // 4. Conectar al script ExperienceUI en el Panel de Pausa
            ExperienceUI expUI = pausePanel.GetComponent<ExperienceUI>();
            if (expUI == null) expUI = pausePanel.AddComponent<ExperienceUI>();

            // Conectar el Action persistente
            UnityEditor.Events.UnityEventTools.AddPersistentListener(rbBtn.onClick, expUI.OnRebirthClicked);

            SerializedObject soUI = new SerializedObject(expUI);
            soUI.FindProperty("xpSlider").objectReferenceValue = slider;
            soUI.FindProperty("levelText").objectReferenceValue = lvlText;
            soUI.FindProperty("xpNumbersText").objectReferenceValue = numText;
            soUI.FindProperty("rebirthButton").objectReferenceValue = rbBtn;
            
            if (player != null)
            {
                soUI.FindProperty("playerExperience").objectReferenceValue = player.GetComponent<PlayerExperience>();
            }

            soUI.ApplyModifiedProperties();

            // Guardar Cambios en la escena o prefab (forzar actualización)
            EditorUtility.SetDirty(pausePanel);

            Selection.activeGameObject = xpPanel;
            Debug.Log("<color=green>[Robot XP]</color> ¡La barra de experiencia ha sido generada y conectada automáticamente a tu Menú de Pausa!");
        }
    }
}
