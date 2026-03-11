using UnityEngine;
using UnityEditor;
using VeinsOfMalice.World;

namespace VeinsOfMalice.Editor
{
    public class ItemCreationTool
    {
        [MenuItem("Tools/Veins of Malice/Create Default Essence Item")]
        public static void CreateEssenceItem()
        {
            // Ensure directory exists
            if (!AssetDatabase.IsValidFolder("Assets/Data"))
                AssetDatabase.CreateFolder("Assets", "Data");
            if (!AssetDatabase.IsValidFolder("Assets/Data/Items"))
                AssetDatabase.CreateFolder("Assets/Data", "Items");

            string path = "Assets/Data/Items/CursedEssenceItem.asset";
            ItemData essenceItem = AssetDatabase.LoadAssetAtPath<ItemData>(path);
            
            if (essenceItem == null)
            {
                essenceItem = ScriptableObject.CreateInstance<ItemData>();
                essenceItem.itemName = "Cursed Essence";
                essenceItem.description = "A fragment of pure malice. Used as currency and for dark rituals.";
                essenceItem.value = 1;
                AssetDatabase.CreateAsset(essenceItem, path);
                Debug.Log($"<color=green>[ItemTool]</color> Created new ItemData at {path}");
            }

            // Always try to update the icon from the requested path
            Sprite customIcon = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Items/ItemImages/CursedEssence_Icon-Photoroom.png");
            if (customIcon != null)
            {
                essenceItem.icon = customIcon;
                EditorUtility.SetDirty(essenceItem);
                AssetDatabase.SaveAssets();
                Debug.Log($"<color=green>[ItemTool]</color> Updated icon for {essenceItem.itemName}");
            }
            else
            {
                Debug.LogWarning("<color=orange>[ItemTool]</color> Could not find icon at Assets/Art/Items/ItemImages/CursedEssence_Icon-Photoroom.png");
            }

            EditorGUIUtility.PingObject(essenceItem);
            Selection.activeObject = essenceItem;
        }
    }
}
