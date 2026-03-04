using UnityEngine;
using UnityEditor;
using VeinsOfMalice.AI;

namespace VeinsOfMalice.Editor
{
    public class LootAssignmentTool : EditorWindow
    {
        [MenuItem("Tools/Veins of Malice/Assign Loot to All Enemies")]
        public static void AssignLoot()
        {
            // 1. Find the Essence Prefab
            string prefabPath = "Assets/Prefabs/CursedEssence.prefab";
            GameObject essencePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (essencePrefab == null)
            {
                Debug.LogError("<color=red>[LootTool]</color> Essence Prefab not found at " + prefabPath + ". Create it first using the Essence Prefab Tool.");
                return;
            }

            // 2. Find all Enemy Prefabs (those with EnemyHealth)
            string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Prefabs" });
            int affectedCount = 0;

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                
                if (prefab != null && prefab.GetComponent<EnemyHealth>() != null)
                {
                    // Add LootSpawner if missing
                    LootSpawner spawner = prefab.GetComponent<LootSpawner>();
                    if (spawner == null)
                    {
                        spawner = prefab.AddComponent<LootSpawner>();
                    }

                    // Assign the prefab using SerializedObject to ensure it saves
                    SerializedObject so = new SerializedObject(spawner);
                    so.FindProperty("essencePrefab").objectReferenceValue = essencePrefab;
                    
                    // Set default count to 1 as requested
                    so.FindProperty("minEssence").intValue = 1;
                    so.FindProperty("maxEssence").intValue = 1;
                    
                    so.ApplyModifiedProperties();
                    
                    EditorUtility.SetDirty(prefab);
                    affectedCount++;
                    Debug.Log("<color=cyan>[LootTool]</color> Assigned loot to: " + prefab.name);
                }
            }

            AssetDatabase.SaveAssets();
            Debug.Log("<color=green>[The Robot]</color> Loot assignment complete! " + affectedCount + " enemies updated.");
        }
    }
}
