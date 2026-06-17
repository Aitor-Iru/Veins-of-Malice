using UnityEngine;
using UnityEditor;

namespace VeinsOfMalice.Editor
{
    public class LevelBuilder : EditorWindow
    {
        [MenuItem("Tools/Veins of Malice/Generate Park Prototype Level")]
        public static void GenerateParkLevel()
        {
            GameObject levelRoot = new GameObject("ParkLevel_Prototype");
            
            // 1. Scene Organization
            Transform env = new GameObject("Environment").transform;
            Transform gameplay = new GameObject("Gameplay").transform;
            Transform props = new GameObject("Props").transform;
            env.SetParent(levelRoot.transform);
            gameplay.SetParent(levelRoot.transform);
            props.SetParent(levelRoot.transform);

            // 2. Floor & Path
            CreateBox(env, "Grass_Floor", new Vector3(0, -0.5f, 0), new Vector3(60, 1, 15), new Color(0.2f, 0.5f, 0.1f), "Ground");
            CreateBox(env, "Dirt_Path", new Vector3(0, -0.48f, 0), new Vector3(60, 1, 3.5f), new Color(0.4f, 0.3f, 0.1f), "Ground");

            // 3. Background (Depth)
            CreateBox(env, "Distant_Trees_Wall", new Vector3(0, 8, 7f), new Vector3(70, 16, 0.1f), new Color(0.1f, 0.3f, 0.1f), "Wall");

            // 4. Gameplay Platforms (Stone style)
            Color stoneColor = new Color(0.4f, 0.4f, 0.45f);
            CreateBox(gameplay, "Stone_Platform_1", new Vector3(-8, 2, 0), new Vector3(6, 0.5f, 2), stoneColor, "Ground").AddComponent<VeinsOfMalice.World.OneWayPlatform>();
            CreateBox(gameplay, "Stone_Platform_2", new Vector3(0, 4, 0), new Vector3(8, 0.5f, 2), stoneColor, "Ground").AddComponent<VeinsOfMalice.World.OneWayPlatform>();
            CreateBox(gameplay, "Stone_Platform_3", new Vector3(10, 2.5f, 0), new Vector3(6, 0.5f, 2), stoneColor, "Ground").AddComponent<VeinsOfMalice.World.OneWayPlatform>();

            // 5. Trees (Randomized scattering for depth)
            for (int i = 0; i < 15; i++)
            {
                float x = Random.Range(-28f, 28f);
                float z = (i % 2 == 0) ? Random.Range(3f, 5f) : Random.Range(-3f, -5f); // Background and Foreground trees
                CreateTree(props, "ParkTree_" + i, new Vector3(x, 0, z));
            }

            // 6. Benches
            for (int i = 0; i < 4; i++)
            {
                float x = -15 + (i * 10);
                CreateBench(props, "Bench_" + i, new Vector3(x, 0.25f, 2.5f));
            }

            // 7. Boundaries
            CreateBox(gameplay, "Level_Start", new Vector3(-30.5f, 5, 0), new Vector3(1, 10, 5), Color.red, "Wall", false);
            CreateBox(gameplay, "Level_End", new Vector3(30.5f, 5, 0), new Vector3(1, 10, 5), Color.red, "Wall", false);

            Selection.activeGameObject = levelRoot;
            SceneView.FrameLastActiveSceneView();
            
            Debug.Log("<color=green>[Park Builder]</color> A beautiful (provisional) park has been generated! Ideal for 2.5D testing.");
        }

        [MenuItem("Tools/Veins of Malice/Generate Fantasy Forest Combat Level")]
        public static void GenerateFantasyForestLevel()
        {
            GameObject levelRoot = new GameObject("FantasyForest_CombatLevel");
            
            // 1. Scene Organization
            Transform env = new GameObject("Environment").transform;
            Transform gameplay = new GameObject("Gameplay").transform;
            Transform props = new GameObject("Props").transform;
            env.SetParent(levelRoot.transform);
            gameplay.SetParent(levelRoot.transform);
            props.SetParent(levelRoot.transform);

            // 2. Floor & Path using Fantasy Forest materials
            Material grassMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Fantasy Forest Environment Free Sample/Materials/grass01.mat");
            Material dirtMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Fantasy Forest Environment Free Sample/Materials/dirt01.mat");
            Material barkMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Fantasy Forest Environment Free Sample/Materials/bark01_bottom.mat");

            GameObject grassFloor = CreateBox(env, "Grass_Floor", new Vector3(0, -0.5f, 0), new Vector3(60, 1, 15), new Color(0.2f, 0.5f, 0.1f), "Ground");
            if (grassMat != null) grassFloor.GetComponent<Renderer>().sharedMaterial = grassMat;

            GameObject dirtPath = CreateBox(env, "Dirt_Path", new Vector3(0, -0.48f, 0), new Vector3(60, 1, 3.5f), new Color(0.4f, 0.3f, 0.1f), "Ground");
            if (dirtMat != null) dirtPath.GetComponent<Renderer>().sharedMaterial = dirtMat;

            // 3. Background (Depth Wall)
            GameObject backgroundWall = CreateBox(env, "Distant_Trees_Wall", new Vector3(0, 8, 7f), new Vector3(70, 16, 0.1f), new Color(0.1f, 0.3f, 0.1f), "Wall");
            if (grassMat != null) backgroundWall.GetComponent<Renderer>().sharedMaterial = grassMat;

            // 4. Gameplay Platforms (Stone/Wood style)
            Color platformColor = new Color(0.45f, 0.35f, 0.25f);
            GameObject plat1 = CreateBox(gameplay, "Platform_Left", new Vector3(-8, 2, 0), new Vector3(6, 0.5f, 2), platformColor, "Ground");
            plat1.AddComponent<VeinsOfMalice.World.OneWayPlatform>();
            if (barkMat != null) plat1.GetComponent<Renderer>().sharedMaterial = barkMat;

            GameObject plat2 = CreateBox(gameplay, "Platform_Center", new Vector3(0, 4, 0), new Vector3(8, 0.5f, 2), platformColor, "Ground");
            plat2.AddComponent<VeinsOfMalice.World.OneWayPlatform>();
            if (barkMat != null) plat2.GetComponent<Renderer>().sharedMaterial = barkMat;

            GameObject plat3 = CreateBox(gameplay, "Platform_Right", new Vector3(10, 2.5f, 0), new Vector3(6, 0.5f, 2), platformColor, "Ground");
            plat3.AddComponent<VeinsOfMalice.World.OneWayPlatform>();
            if (barkMat != null) plat3.GetComponent<Renderer>().sharedMaterial = barkMat;

            // 5. Trees (Scatter trees from the package)
            GameObject treePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Fantasy Forest Environment Free Sample/Meshes/Prefabs/tree_1.prefab");
            int treeCount = 12;
            for (int i = 0; i < treeCount; i++)
            {
                float x = -26f + (i * 5.2f) + Random.Range(-1.5f, 1.5f);
                
                // Adjust tree X positions to keep them away from player and dummy spawn spots
                if (Mathf.Abs(x - (-12f)) < 3.0f) x += (x >= -12f) ? 3.0f : -3.0f;
                if (Mathf.Abs(x - (-3f)) < 3.0f) x += (x >= -3f) ? 3.0f : -3.0f;
                if (Mathf.Abs(x - 5f) < 3.0f) x += (x >= 5f) ? 3.0f : -3.0f;
                if (Mathf.Abs(x - 12f) < 3.0f) x += (x >= 12f) ? 3.0f : -3.0f;

                float z = (i % 2 == 0) ? Random.Range(3.5f, 5.5f) : Random.Range(-3.5f, -5.5f); // Z-Depth layout for combat clear screen
                Vector3 treePos = new Vector3(x, 0, z);

                if (treePrefab != null)
                {
                    GameObject treeInstance = (GameObject)PrefabUtility.InstantiatePrefab(treePrefab);
                    treeInstance.name = "ForestTree_" + i;
                    treeInstance.transform.position = treePos;
                    treeInstance.transform.SetParent(props);
                    treeInstance.transform.localScale = Vector3.one * Random.Range(0.85f, 1.25f);
                    treeInstance.transform.Rotate(0, Random.Range(0, 360f), 0);

                    // Unpack the prefab first so we can modify/remove its components (like colliders)
                    PrefabUtility.UnpackPrefabInstance(treeInstance, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

                    // Remove colliders on decoration props so they don't interfere with combat
                    Collider[] colliders = treeInstance.GetComponentsInChildren<Collider>();
                    foreach (var col in colliders)
                    {
                        DestroyImmediate(col);
                    }
                }
                else
                {
                    CreateTree(props, "ProvisionalTree_" + i, treePos);
                }
            }

            // 6. Grass Tufts (Scatter grass from package)
            GameObject grassPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Fantasy Forest Environment Free Sample/Meshes/Prefabs/grass01.prefab");
            if (grassPrefab != null)
            {
                for (int i = 0; i < 20; i++)
                {
                    float x = Random.Range(-28f, 28f);
                    float z = Random.Range(-2.5f, 2.5f);
                    if (Mathf.Abs(z) < 0.5f) z += (z >= 0) ? 0.6f : -0.6f;

                    GameObject grassInstance = (GameObject)PrefabUtility.InstantiatePrefab(grassPrefab);
                    grassInstance.name = "ForestGrass_" + i;
                    grassInstance.transform.position = new Vector3(x, 0, z);
                    grassInstance.transform.SetParent(props);
                    grassInstance.transform.localScale = Vector3.one * Random.Range(0.8f, 1.5f);
                    grassInstance.transform.Rotate(0, Random.Range(0, 360f), 0);

                    // Unpack the prefab first so we can remove its colliders
                    PrefabUtility.UnpackPrefabInstance(grassInstance, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

                    Collider[] colliders = grassInstance.GetComponentsInChildren<Collider>();
                    foreach (var col in colliders)
                    {
                        DestroyImmediate(col);
                    }
                }
            }

            // 7. Spawn Player and Combat Dummies
            GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Player.prefab");
            GameObject passiveDummy = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/PassiveDummy.prefab");
            GameObject attackingDummy = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/AttackingDummy.prefab");
            GameObject blockingDummy = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/BlockingDummy.prefab");

            // Ensure a DamageNumberManager exists in the scene
            if (Object.FindObjectOfType<VeinsOfMalice.UI.DamageNumberManager>() == null)
            {
                GameObject dmgMgrObj = new GameObject("DamageNumberManager");
                dmgMgrObj.AddComponent<VeinsOfMalice.UI.DamageNumberManager>();
                dmgMgrObj.transform.SetParent(levelRoot.transform);
            }

            // Spawn Player
            if (playerPrefab != null)
            {
                GameObject p = (GameObject)PrefabUtility.InstantiatePrefab(playerPrefab);
                p.name = "Player_Kairo";
                p.transform.position = new Vector3(-12, 1, 0);
                p.transform.SetParent(gameplay);
            }
            else
            {
                Debug.LogWarning("[Forest Builder] Player prefab not found at Assets/Prefabs/Player.prefab");
            }

            // Helper to set dummy layer and add collider if missing
            void SetupDummy(GameObject dummyObj)
            {
                // Set layer to "Enemy" if it exists; NEVER use Ground layer on characters
                int enemyLayer = LayerMask.NameToLayer("Enemy");
                if (enemyLayer != -1)
                    dummyObj.layer = enemyLayer;

                // Only add a collider if the dummy (and ALL its children) have none —
                // avoids double-colliders that confuse the player's ground raycasts.
                Collider[] existingColliders = dummyObj.GetComponentsInChildren<Collider>();
                if (existingColliders.Length == 0)
                {
                    CapsuleCollider cap = dummyObj.AddComponent<CapsuleCollider>();
                    cap.height = 2f;
                    cap.radius = 0.4f;
                    cap.center = new Vector3(0, 1f, 0);
                }
            }

            // Passive Dummy
            if (passiveDummy != null)
            {
                GameObject d = (GameObject)PrefabUtility.InstantiatePrefab(passiveDummy);
                d.name = "PassiveDummy";
                d.transform.position = new Vector3(-3, 0.5f, 0);
                d.transform.SetParent(gameplay);
                SetupDummy(d);
            }

            // Attacking Dummy
            if (attackingDummy != null)
            {
                GameObject d = (GameObject)PrefabUtility.InstantiatePrefab(attackingDummy);
                d.name = "AttackingDummy";
                d.transform.position = new Vector3(5, 0.5f, 0);
                d.transform.SetParent(gameplay);
                SetupDummy(d);
            }

            // Blocking Dummy
            if (blockingDummy != null)
            {
                GameObject d = (GameObject)PrefabUtility.InstantiatePrefab(blockingDummy);
                d.name = "BlockingDummy";
                d.transform.position = new Vector3(12, 0.5f, 0);
                d.transform.SetParent(gameplay);
                SetupDummy(d);
            }

            // 8. Boundaries
            CreateBox(gameplay, "Level_Start", new Vector3(-30.5f, 5, 0), new Vector3(1, 10, 5), Color.red, "Wall", false);
            CreateBox(gameplay, "Level_End", new Vector3(30.5f, 5, 0), new Vector3(1, 10, 5), Color.red, "Wall", false);

            // 9. Camera setup (Find Main Camera, add CameraController, and target the Player)
            Camera mainCam = Camera.main;
            if (mainCam == null)
            {
                GameObject camObj = GameObject.FindWithTag("MainCamera");
                if (camObj == null) camObj = GameObject.Find("Main Camera");
                if (camObj != null) mainCam = camObj.GetComponent<Camera>();
            }

            if (mainCam != null)
            {
                // Align camera to starting player tracking position (Player is at X=-12, Y=1, Z=0. Offset is Y=2, Z=-10 -> Cam is at X=-12, Y=3, Z=-10)
                mainCam.transform.position = new Vector3(-12f, 3f, -10f);
                mainCam.transform.rotation = Quaternion.identity;

                // Add or configure CameraController
                CameraController camCtrl = mainCam.GetComponent<CameraController>();
                if (camCtrl == null)
                {
                    camCtrl = mainCam.gameObject.AddComponent<CameraController>();
                }

                GameObject playerObj = GameObject.Find("Player_Kairo");
                if (playerObj != null)
                {
                    // Use SerializedObject to safely assign private serialized properties in the editor
                    SerializedObject so = new SerializedObject(camCtrl);
                    so.FindProperty("target").objectReferenceValue = playerObj.transform;
                    so.FindProperty("offset").vector3Value = new Vector3(0f, 2f, -10f);
                    so.FindProperty("smoothTime").floatValue = 0.15f;
                    so.FindProperty("deadZoneX").floatValue = 0.5f;
                    so.FindProperty("deadZoneY").floatValue = 0.3f;
                    so.FindProperty("useBounds").boolValue = false;
                    so.ApplyModifiedProperties();
                }
            }

            Selection.activeGameObject = levelRoot;
            SceneView.FrameLastActiveSceneView();
            
            Debug.Log("<color=green>[Forest Builder]</color> A beautiful 2.5D Fantasy Forest Combat Scene has been generated successfully! Ready for combat testing.");
        }

        private static void CreateTree(Transform parent, string name, Vector3 pos)
        {
            GameObject treeGroup = new GameObject(name);
            treeGroup.transform.SetParent(parent);
            treeGroup.transform.position = pos;

            // Trunk
            GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            trunk.name = "Trunk";
            trunk.transform.SetParent(treeGroup.transform, false);
            trunk.transform.localScale = new Vector3(0.4f, 2, 0.4f);
            trunk.transform.localPosition = new Vector3(0, 2, 0);
            trunk.GetComponent<Renderer>().material = GetLitMaterial(new Color(0.35f, 0.2f, 0.1f));

            // Foliage
            GameObject foliage = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            foliage.name = "Leaves";
            foliage.transform.SetParent(treeGroup.transform, false);
            foliage.transform.localScale = new Vector3(2.5f, 3f, 2.5f);
            foliage.transform.localPosition = new Vector3(0, 4.5f, 0);
            foliage.GetComponent<Renderer>().material = GetLitMaterial(new Color(0.1f, 0.4f, 0.1f));
            
            // Disable collisions for trees (props)
            DestroyImmediate(trunk.GetComponent<Collider>());
            DestroyImmediate(foliage.GetComponent<Collider>());
        }

        private static void CreateBench(Transform parent, string name, Vector3 pos)
        {
            GameObject bench = new GameObject(name);
            bench.transform.SetParent(parent);
            bench.transform.position = pos;
            Color benchColor = new Color(0.5f, 0.4f, 0.3f);

            // Seat
            GameObject seat = GameObject.CreatePrimitive(PrimitiveType.Cube);
            seat.transform.SetParent(bench.transform, false);
            seat.transform.localScale = new Vector3(2.5f, 0.2f, 0.8f);
            seat.GetComponent<Renderer>().material = GetLitMaterial(benchColor);

            // Back
            GameObject back = GameObject.CreatePrimitive(PrimitiveType.Cube);
            back.transform.SetParent(bench.transform, false);
            back.transform.localScale = new Vector3(2.5f, 0.6f, 0.1f);
            back.transform.localPosition = new Vector3(0, 0.3f, 0.4f);
            back.GetComponent<Renderer>().material = GetLitMaterial(benchColor);
        }

        private static GameObject CreateBox(Transform parent, string name, Vector3 pos, Vector3 scale, Color color, string layerName, bool showColor = true)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.name = name;
            obj.transform.SetParent(parent);
            obj.transform.position = pos;
            obj.transform.localScale = scale;

            if (!showColor)
            {
                Renderer rend = obj.GetComponent<Renderer>();
                if (rend) rend.enabled = false;
            }
            else
            {
                Renderer rend = obj.GetComponent<Renderer>();
                rend.material = GetLitMaterial(color);
            }

            int layer = LayerMask.NameToLayer(layerName);
            // If the specified layer does not exist, fallback to Default layer (index 0)
            if (layer != -1) obj.layer = layer;
            else obj.layer = 0; // Default

            return obj;
        }

        private static Material GetLitMaterial(Color color)
        {
            Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.color = color;
            return mat;
        }
    }
}
