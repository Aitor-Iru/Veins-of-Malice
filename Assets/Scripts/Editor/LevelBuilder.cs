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
            if (layer != -1) obj.layer = layer;

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
