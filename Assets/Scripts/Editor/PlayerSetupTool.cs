using UnityEngine;
using UnityEditor;
using VeinsOfMalice.Player;

namespace VeinsOfMalice.Editor
{
    public class PlayerSetupTool : EditorWindow
    {
        [MenuItem("Tools/Veins of Malice/Finalize Player Setup")]
        public static void FinalizeSetup()
        {
            string prefabPath = "Assets/Prefabs/Player.prefab";
            GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (playerPrefab == null)
            {
                Debug.LogError("<color=red>[PlayerTool]</color> Player Prefab not found at " + prefabPath);
                return;
            }

            // Open the prefab for editing
            GameObject instance = PrefabUtility.LoadPrefabContents(prefabPath);

            bool changed = false;

            // 1. Add PlayerInventory if missing
            if (instance.GetComponent<PlayerInventory>() == null)
            {
                instance.AddComponent<PlayerInventory>();
                Debug.Log("<color=cyan>[PlayerTool]</color> Added PlayerInventory to Player.");
                changed = true;
            }

            // 2. Add and Configure PlayerInteraction
            PlayerInteraction interact = instance.GetComponent<PlayerInteraction>();
            if (interact == null)
            {
                interact = instance.AddComponent<PlayerInteraction>();
                Debug.Log("<color=cyan>[PlayerTool]</color> Added PlayerInteraction to Player.");
                changed = true;
            }

            // Configure Interaction
            if (interact != null)
            {
                // Find InputReader asset
                string readerPath = "Assets/Input/InputReader.asset";
                InputReader reader = AssetDatabase.LoadAssetAtPath<InputReader>(readerPath);
                
                // Use SerializedObject to set private fields safely
                SerializedObject so = new SerializedObject(interact);
                
                if (reader != null)
                {
                    so.FindProperty("inputReader").objectReferenceValue = reader;
                    Debug.Log("<color=cyan>[PlayerTool]</color> Assigned InputReader to PlayerInteraction.");
                }
                
                // Set LayerMask to "Gameplay"
                int gameplayLayer = LayerMask.NameToLayer("Gameplay");
                if (gameplayLayer != -1)
                {
                    so.FindProperty("interactableLayer").intValue = 1 << gameplayLayer;
                    Debug.Log($"<color=cyan>[PlayerTool]</color> Set interactableLayer to '{gameplayLayer}' (Gameplay).");
                }
                else
                {
                    Debug.LogWarning("<color=red>[PlayerTool]</color> 'Gameplay' layer not found! Using Default (0).");
                }
                
                so.ApplyModifiedProperties();
                changed = true;
            }

            // 3. Ensure Tag is "Player"
            if (!instance.CompareTag("Player"))
            {
                instance.tag = "Player";
                Debug.Log("<color=cyan>[PlayerTool]</color> Set tag to 'Player'.");
                changed = true;
            }

            if (changed)
            {
                PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
                Debug.Log("<color=green>[The Robot]</color> Player prefab updated successfully!");
            }
            else
            {
                Debug.Log("<color=yellow>[The Robot]</color> Player prefab was already correctly configured.");
            }

            PrefabUtility.UnloadPrefabContents(instance);
            AssetDatabase.Refresh();
        }
    }
}
