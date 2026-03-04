using UnityEngine;
using UnityEditor;

namespace VeinsOfMalice.Editor
{
    public class URPMaterialFixer : EditorWindow
    {
        [MenuItem("Tools/Veins of Malice/Fix Magenta Materials (URP)")]
        public static void FixMaterials()
        {
            // 1. Create a basic URP Lit material if it doesn't exist
            string folderPath = "Assets/Art/Materials";
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                System.IO.Directory.CreateDirectory(folderPath);
                AssetDatabase.Refresh();
            }

            string matPath = folderPath + "/URP_Default_Lit.mat";
            Material urpLit = AssetDatabase.LoadAssetAtPath<Material>(matPath);

            if (urpLit == null)
            {
                Shader urpShader = Shader.Find("Universal Render Pipeline/Lit");
                if (urpShader == null)
                {
                    Debug.LogError("<color=red>[URP Fix]</color> Could not find 'Universal Render Pipeline/Lit' shader. Is URP installed?");
                    return;
                }
                urpLit = new Material(urpShader);
                AssetDatabase.CreateAsset(urpLit, matPath);
            }

            // 2. Find all prefabs in Assets/Prefabs
            string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Prefabs" });
            int fixedCount = 0;

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject instance = PrefabUtility.LoadPrefabContents(path);
                
                Renderer[] renderers = instance.GetComponentsInChildren<Renderer>(true);
                bool changed = false;

                foreach (Renderer r in renderers)
                {
                    // If it's a MeshRenderer (3D model), it likely needs URP Lit
                    if (r is MeshRenderer)
                    {
                        r.sharedMaterial = urpLit;
                        changed = true;
                    }
                    // SpriteRenderers usually work with Sprite-Default, but URP sometimes 
                    // likes "Universal Render Pipeline/2D/Sprite-Lit-Default" if using 2D Lights
                }

                if (changed)
                {
                    PrefabUtility.SaveAsPrefabAsset(instance, path);
                    fixedCount++;
                }
                PrefabUtility.UnloadPrefabContents(instance);
            }

            AssetDatabase.SaveAssets();
            Debug.Log("<color=green>[The Robot]</color> Material fix complete! " + fixedCount + " prefabs updated with URP materials.");
        }
    }
}
