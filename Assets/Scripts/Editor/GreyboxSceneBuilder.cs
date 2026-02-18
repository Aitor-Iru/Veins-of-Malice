#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

/// <summary>
/// Editor utility that builds the Greybox test scene programmatically.
/// Run via: Tools > Veins of Malice > Create Greybox Scene
/// </summary>
public static class GreyboxSceneBuilder
{
    [MenuItem("Tools/Veins of Malice/Create Greybox Scene")]
    public static void CreateGreyboxScene()
    {
        // ── 1. Create & open new scene ────────────────────────────────────
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // ── 2. Lighting ───────────────────────────────────────────────────
        GameObject dirLight = new GameObject("Directional Light");
        Light light = dirLight.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.2f;
        light.color = new Color(1f, 0.95f, 0.84f);
        dirLight.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

        // ── 3. Ground Layer setup ─────────────────────────────────────────
        // Ensure "Ground" layer exists (layer 6 by default in Unity)
        int groundLayer = LayerMask.NameToLayer("Ground");
        if (groundLayer == -1)
        {
            Debug.LogWarning("[GreyboxBuilder] 'Ground' layer not found. Please add it in Project Settings > Tags and Layers. Using Default layer for now.");
            groundLayer = 0;
        }

        // ── 4. Geometry ───────────────────────────────────────────────────
        // Main floor
        CreatePlatform("Floor", new Vector3(0, -1f, 0), new Vector3(30f, 0.5f, 1f), groundLayer, new Color(0.35f, 0.35f, 0.35f));

        // Floating platforms
        CreatePlatform("Platform_A", new Vector3(-5f, 1.5f, 0), new Vector3(4f, 0.4f, 1f), groundLayer, new Color(0.4f, 0.4f, 0.5f));
        CreatePlatform("Platform_B", new Vector3(2f, 3.5f, 0), new Vector3(5f, 0.4f, 1f), groundLayer, new Color(0.4f, 0.4f, 0.5f));
        CreatePlatform("Platform_C", new Vector3(9f, 5.5f, 0), new Vector3(4f, 0.4f, 1f), groundLayer, new Color(0.4f, 0.4f, 0.5f));

        // Left wall
        CreatePlatform("Wall_Left", new Vector3(-15f, 4f, 0), new Vector3(0.5f, 10f, 1f), groundLayer, new Color(0.3f, 0.3f, 0.3f));
        // Right wall
        CreatePlatform("Wall_Right", new Vector3(15f, 4f, 0), new Vector3(0.5f, 10f, 1f), groundLayer, new Color(0.3f, 0.3f, 0.3f));

        // ── 5. Player ─────────────────────────────────────────────────────
        GameObject player = CreatePlayer(groundLayer);

        // ── 6. Camera ─────────────────────────────────────────────────────
        GameObject camGO = new GameObject("Main Camera");
        camGO.tag = "MainCamera";
        Camera cam = camGO.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.08f, 0.08f, 0.12f);
        cam.orthographic = false;
        cam.fieldOfView = 60f;
        camGO.transform.position = new Vector3(0f, 2f, -10f);

        CameraController camController = camGO.AddComponent<CameraController>();
        // Set target via SerializedObject so it persists in the scene
        SerializedObject so = new SerializedObject(camController);
        so.FindProperty("target").objectReferenceValue = player.transform;
        so.ApplyModifiedProperties();

        // ── 7. GameManager ────────────────────────────────────────────────
        GameObject gmGO = new GameObject("GameManager");
        gmGO.AddComponent<GameManager>();

        // ── 8. Save scene ─────────────────────────────────────────────────
        string scenePath = "Assets/Scenes/GreyboxTest.unity";
        EditorSceneManager.SaveScene(scene, scenePath);
        AssetDatabase.Refresh();

        Debug.Log($"[GreyboxBuilder] Scene created and saved at: {scenePath}");
        EditorUtility.DisplayDialog(
            "Greybox Scene Created!",
            $"Scene saved to:\n{scenePath}\n\nPress Play to test the prototype.",
            "OK"
        );
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static GameObject CreatePlatform(string name, Vector3 position, Vector3 scale, int layer, Color color)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        go.transform.position = position;
        go.transform.localScale = scale;
        go.layer = layer;

        // Apply a simple grey material with tint
        Renderer rend = go.GetComponent<Renderer>();
        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        if (mat.shader.name == "Hidden/InternalErrorShader")
        {
            // Fallback to Standard if URP not found
            mat = new Material(Shader.Find("Standard"));
        }
        mat.color = color;
        rend.sharedMaterial = mat;

        return go;
    }

    private static GameObject CreatePlayer(int groundLayer)
    {
        // Root
        GameObject player = new GameObject("Player");
        player.transform.position = new Vector3(0f, 1f, 0f);

        // Rigidbody
        Rigidbody rb = player.AddComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
        rb.useGravity = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Collider
        CapsuleCollider col = player.AddComponent<CapsuleCollider>();
        col.height = 1.8f;
        col.radius = 0.35f;
        col.center = new Vector3(0f, 0.9f, 0f);

        // Visual placeholder (child capsule)
        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        visual.name = "Model";
        Object.DestroyImmediate(visual.GetComponent<CapsuleCollider>());
        visual.transform.SetParent(player.transform);
        visual.transform.localPosition = new Vector3(0f, 0.9f, 0f);
        visual.transform.localScale = Vector3.one;

        // Color the player placeholder
        Renderer rend = visual.GetComponent<Renderer>();
        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        if (mat.shader.name == "Hidden/InternalErrorShader")
            mat = new Material(Shader.Find("Standard"));
        mat.color = new Color(0.2f, 0.6f, 1f); // Blue placeholder
        rend.sharedMaterial = mat;

        // GroundCheck child
        GameObject groundCheck = new GameObject("GroundCheck");
        groundCheck.transform.SetParent(player.transform);
        groundCheck.transform.localPosition = new Vector3(0f, 0.05f, 0f);

        // ── InputReader asset (create if missing) ─────────────────────────
        const string inputReaderPath = "Assets/Input/InputReader.asset";
        InputReader inputReader = AssetDatabase.LoadAssetAtPath<InputReader>(inputReaderPath);
        if (inputReader == null)
        {
            // Ensure folder exists
            if (!AssetDatabase.IsValidFolder("Assets/Input"))
                AssetDatabase.CreateFolder("Assets", "Input");

            inputReader = ScriptableObject.CreateInstance<InputReader>();

            // Assign the .inputactions asset to the InputReader
            var inputActionsAsset = AssetDatabase.LoadAssetAtPath<InputActionAsset>(
                "Assets/InputSystem_Actions.inputactions");
            if (inputActionsAsset != null)
            {
                SerializedObject soReader = new SerializedObject(inputReader);
                soReader.FindProperty("inputActions").objectReferenceValue = inputActionsAsset;
                soReader.ApplyModifiedProperties();
            }
            else
            {
                Debug.LogWarning("[GreyboxBuilder] InputSystem_Actions.inputactions not found. Assign it manually in the InputReader asset.");
            }

            AssetDatabase.CreateAsset(inputReader, inputReaderPath);
            AssetDatabase.SaveAssets();
            Debug.Log($"[GreyboxBuilder] InputReader asset created at: {inputReaderPath}");
        }
        else
        {
            Debug.Log($"[GreyboxBuilder] InputReader asset loaded from: {inputReaderPath}");
        }

        // ── PlayerController ──────────────────────────────────────────────
        PlayerController pc = player.AddComponent<PlayerController>();
        SerializedObject soPc = new SerializedObject(pc);
        soPc.FindProperty("inputReader").objectReferenceValue = inputReader;
        soPc.FindProperty("groundCheck").objectReferenceValue = groundCheck.transform;
        soPc.FindProperty("groundLayer").intValue = 1 << groundLayer;
        soPc.ApplyModifiedProperties();

        // PlayerHealth
        player.AddComponent<PlayerHealth>();

        return player;
    }
}
#endif
