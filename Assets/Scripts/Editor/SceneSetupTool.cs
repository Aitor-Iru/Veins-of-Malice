using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class SceneSetupTool : EditorWindow
{
    [MenuItem("Tools/Veins of Malice/Setup Base Scene")]
    public static void SetupBaseScene()
    {
        // Check if user wants to save current scene
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            // Create new scene
            var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            
            // Create standard folders/parents
            GameObject systems = new GameObject("--- SYSTEMS ---");
            GameObject env = new GameObject("--- ENVIRONMENT ---");
            GameObject ui = new GameObject("--- UI ---");
            GameObject lighting = new GameObject("--- LIGHTING ---");

            // Setup Systems
            GameObject gameManager = new GameObject("GameManager");
            gameManager.transform.SetParent(systems.transform);
            // Add GameManager component
            if (System.Type.GetType("GameManager") != null) gameManager.AddComponent(System.Type.GetType("GameManager"));

            // Setup Camera
            GameObject mainCamera = new GameObject("Main Camera");
            mainCamera.tag = "MainCamera";
            Camera cam = mainCamera.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 5f;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.1f, 0.1f, 0.1f); // Dark background
            mainCamera.transform.position = new Vector3(0, 0, -10);
            mainCamera.AddComponent<AudioListener>();

            // Setup Lighting (2D default)
            GameObject globalLight = new GameObject("Global Light 2D");
            globalLight.transform.SetParent(lighting.transform);
            // In URP 2D, we would add Light2D, but for now just placeholder
            
            // Setup Environment placeholder
            GameObject ground = new GameObject("Ground_Placeholder");
            ground.transform.SetParent(env.transform);
            ground.transform.position = new Vector3(0, -2, 0);
            ground.transform.localScale = new Vector3(10, 1, 1);
            var sr = ground.AddComponent<SpriteRenderer>();
            // Create a white sprite texture on the fly if needed, or just leave it empty component
            
            // Save Scene
            string scenePath = "Assets/Scenes/BaseScene.unity";
            EditorSceneManager.SaveScene(newScene, scenePath);
            Debug.Log($"Base Scene created at {scenePath}");
        }
    }
}
