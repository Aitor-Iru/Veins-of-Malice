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

            // 2. Ensure Tag is "Player"
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
