using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor.Events;
using TMPro; // TextMeshPro para textos modernos
using VeinsOfMalice.UI; // Namespace de nuestros managers

namespace VeinsOfMalice.EditorTools
{
    /// <summary>
    /// Robot que autogenera y conecta la interfaz de los menús en Unity.
    /// Aparecerá un menú arriba: Tools > Veins of Malice > UI Generators
    /// </summary>
    public class GameMenuBuilder : EditorWindow
    {
        [MenuItem("Tools/Veins of Malice/UI Generators/1. Construir Menú Principal")]
        public static void BuildMainMenu()
        {
            Canvas canvas = SetupCanvasAndEventSystem();

            // 1. Crear o buscar MainMenuManager
            MainMenuManager manager = Object.FindAnyObjectByType<MainMenuManager>();
            if (manager == null)
            {
                GameObject managerObj = new GameObject("MainMenuManager_Auto");
                managerObj.transform.SetParent(canvas.transform);
                manager = managerObj.AddComponent<MainMenuManager>();
            }

            // 2. Fondo (Panel Principal)
            GameObject mainPanel = CreatePanel("MainPanel_BG", canvas.transform, new Color(0.05f, 0.05f, 0.05f, 1f));

            // 3. Título
            CreateText("GameTitle", mainPanel.transform, "VEINS OF MALICE\n<size=50%>Menú Principal</size>", 60, new Vector2(0, 150), Color.white);

            // 4. Botones
            Button btnPlay = CreateButton("Btn_Jugar", mainPanel.transform, "JUGAR", new Vector2(0, -20));
            Button btnOptions = CreateButton("Btn_Opciones", mainPanel.transform, "OPCIONES", new Vector2(0, -100));
            Button btnQuit = CreateButton("Btn_Salir", mainPanel.transform, "SALIR", new Vector2(0, -180));

            // 5. Conectar eventos a los botones automáticamente (persistente en el editor)
            UnityEventTools.AddPersistentListener(btnPlay.onClick, manager.OnPlayClicked);
            UnityEventTools.AddPersistentListener(btnOptions.onClick, manager.OnOptionsClicked);
            UnityEventTools.AddPersistentListener(btnQuit.onClick, manager.OnQuitClicked);

            // Limpiar selección para ver el resultado
            Selection.activeGameObject = canvas.gameObject;
            Debug.Log("<color=green>[GameMenuBuilder]</color> ¡Menú Principal generado y conectado con éxito!");
        }

        [MenuItem("Tools/Veins of Malice/UI Generators/2. Construir Pausa y Game Over")]
        public static void BuildGameplayUI()
        {
            Canvas canvas = SetupCanvasAndEventSystem();

            // 1. Crear el GameUIManager en el propio Canvas
            GameUIManager manager = canvas.gameObject.GetComponent<GameUIManager>();
            if (manager == null)
            {
                manager = canvas.gameObject.AddComponent<GameUIManager>();
            }

            // 2. Construir el Panel de PAUSA
            GameObject pausePanel = CreatePanel("PausePanel", canvas.transform, new Color(0f, 0f, 0f, 0.85f));
            CreateText("PauseTitle", pausePanel.transform, "JUEGO EN PAUSA", 70, new Vector2(0, 150), Color.white);

            Button btnResume = CreateButton("Btn_Continuar", pausePanel.transform, "CONTINUAR", new Vector2(0, 20));
            Button btnGoUpgrades = CreateButton("Btn_AbrirMejoras", pausePanel.transform, "MEJORAS", new Vector2(0, -60));
            Button btnRestartFromPause = CreateButton("Btn_Reiniciar", pausePanel.transform, "REINICIAR", new Vector2(0, -140));
            Button btnMenuFromPause = CreateButton("Btn_MenuPrincipal", pausePanel.transform, "MENÚ PRINCIPAL", new Vector2(0, -220));

            // Conectar eventos de Pausa
            UnityEventTools.AddPersistentListener(btnResume.onClick, manager.ResumeGame);
            UnityEventTools.AddPersistentListener(btnGoUpgrades.onClick, manager.OpenUpgradesMenu);
            UnityEventTools.AddPersistentListener(btnRestartFromPause.onClick, manager.RestartGame);
            UnityEventTools.AddPersistentListener(btnMenuFromPause.onClick, manager.GoToMainMenu);

            // 2.5 Construir el Panel de MEJORAS (oculto por defecto)
            GameObject upgradesPanel = CreatePanel("UpgradesPanel", canvas.transform, new Color(0.1f, 0f, 0.1f, 0.95f));
            CreateText("UpgradesTitle", upgradesPanel.transform, "SANTUARIO DE MEJORAS\n<size=50%>(Requiere Esencia Maldita)</size>", 60, new Vector2(0, 150), Color.magenta);

            Button btnUpgHealth = CreateButton("Btn_UpgHealth", upgradesPanel.transform, "Mejorar Vida (+5)\nCoste: 10", new Vector2(-200, -20));
            Button btnUpgDamage = CreateButton("Btn_UpgDamage", upgradesPanel.transform, "Mejorar Daño (+2)\nCoste: 10", new Vector2(200, -20));
            Button btnBackUpg = CreateButton("Btn_Volver", upgradesPanel.transform, "VOLVER", new Vector2(0, -200));

            UpgradeManager upgManager = canvas.gameObject.GetComponent<UpgradeManager>();
            if (upgManager == null) upgManager = canvas.gameObject.AddComponent<UpgradeManager>();

            UnityEventTools.AddPersistentListener(btnUpgHealth.onClick, upgManager.BuyHealth);
            UnityEventTools.AddPersistentListener(btnUpgDamage.onClick, upgManager.BuyDamage);
            UnityEventTools.AddPersistentListener(btnBackUpg.onClick, manager.CloseUpgradesMenu);

            SerializedObject serializedUpg = new SerializedObject(upgManager);
            serializedUpg.FindProperty("healthButtonText").objectReferenceValue = btnUpgHealth.GetComponentInChildren<TextMeshProUGUI>();
            serializedUpg.FindProperty("damageButtonText").objectReferenceValue = btnUpgDamage.GetComponentInChildren<TextMeshProUGUI>();
            serializedUpg.ApplyModifiedProperties();

            // 3. Construir el Panel de GAME OVER
            GameObject gameOverPanel = CreatePanel("GameOverPanel", canvas.transform, new Color(0.3f, 0f, 0f, 0.9f));
            CreateText("GameOverTitle", gameOverPanel.transform, "HAS MUERTO", 90, new Vector2(0, 150), Color.red);

            Button btnRetry = CreateButton("Btn_Reintentar", gameOverPanel.transform, "REINTENTAR", new Vector2(0, 20));
            Button btnMenuFromGO = CreateButton("Btn_MenuPrincipal", gameOverPanel.transform, "MENÚ PRINCIPAL", new Vector2(0, -60));

            // Conectar eventos de Game Over
            UnityEventTools.AddPersistentListener(btnRetry.onClick, manager.RestartGame);
            UnityEventTools.AddPersistentListener(btnMenuFromGO.onClick, manager.GoToMainMenu);

            // 4. Asignar las referencias autogeneradas en el script del Manager
            SerializedObject serializedManager = new SerializedObject(manager);
            serializedManager.FindProperty("pausePanel").objectReferenceValue = pausePanel;
            serializedManager.FindProperty("gameOverPanel").objectReferenceValue = gameOverPanel;
            serializedManager.FindProperty("upgradesPanel").objectReferenceValue = upgradesPanel;
            serializedManager.ApplyModifiedProperties();

            // Para no molestar la vista, los desactivamos
            pausePanel.SetActive(false);
            upgradesPanel.SetActive(false);
            gameOverPanel.SetActive(false);

            Selection.activeGameObject = canvas.gameObject;
            Debug.Log("<color=green>[GameMenuBuilder]</color> ¡Paneles de Pausa y Game Over generados y conectados con éxito!");
        }

        // ─── Herramientas de Construcción de UI (Robots Internos) ─────────────────────────

        private static Canvas SetupCanvasAndEventSystem()
        {
            // Busca o crea un Canvas
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

            // Busca o crea el EventSystem para que los clicks funcionen
            if (Object.FindAnyObjectByType<EventSystem>() == null)
            {
                GameObject esObj = new GameObject("EventSystem");
                esObj.AddComponent<EventSystem>();
                esObj.AddComponent<StandaloneInputModule>(); // Módulo estándar
            }

            return canvas;
        }

        private static GameObject CreatePanel(string name, Transform parent, Color color)
        {
            GameObject panelObj = new GameObject(name);
            panelObj.transform.SetParent(parent, false);

            RectTransform rect = panelObj.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero; // Full stretch
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            Image img = panelObj.AddComponent<Image>();
            img.color = color;

            return panelObj;
        }

        private static Button CreateButton(string name, Transform parent, string textStr, Vector2 pos)
        {
            GameObject btnObj = new GameObject(name);
            btnObj.transform.SetParent(parent, false);

            RectTransform rect = btnObj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(300, 60);
            rect.anchoredPosition = pos;

            Image img = btnObj.AddComponent<Image>();
            img.color = new Color(0.15f, 0.15f, 0.15f, 1f); // Botón oscuro
            
            Button btn = btnObj.AddComponent<Button>();

            TextMeshProUGUI txtTmp = CreateText("Text", btnObj.transform, textStr, 28, Vector2.zero, Color.white);
            // Igualar tamaño del texto al del botón para no tener cajas de texto gigantes
            txtTmp.GetComponent<RectTransform>().sizeDelta = rect.sizeDelta;

            return btn;
        }

        private static TextMeshProUGUI CreateText(string name, Transform parent, string textStr, float fontSize, Vector2 pos, Color color)
        {
            GameObject txtObj = new GameObject(name);
            txtObj.transform.SetParent(parent, false);

            RectTransform rect = txtObj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(800, 100);
            rect.anchoredPosition = pos;

            TextMeshProUGUI tmp = txtObj.AddComponent<TextMeshProUGUI>();
            tmp.text = textStr;
            tmp.fontSize = fontSize;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = color;
            tmp.enableWordWrapping = false;
            tmp.raycastTarget = false; // <-- CRUCIAL: Impide que el texto invisible reciba clics

            return tmp;
        }
    }
}
