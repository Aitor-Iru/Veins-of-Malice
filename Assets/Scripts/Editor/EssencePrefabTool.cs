using UnityEngine;
using UnityEditor;
using VeinsOfMalice.World;

namespace VeinsOfMalice.Editor
{
    public class EssencePrefabTool : EditorWindow
    {
        [MenuItem("Tools/Veins of Malice/Create Cursed Essence Prefab")]
        public static void CreatePrefab()
        {
            // 1. Create the GameObject
            GameObject essenceObj = new GameObject("CursedEssence_Prefab");
            
            // 2. Add Components
            essenceObj.AddComponent<SphereCollider>().isTrigger = true;
            Rigidbody rb = essenceObj.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = false;
            rb.constraints = RigidbodyConstraints.FreezeRotation;

            essenceObj.AddComponent<CursedEssence>();

            // 3. Create Visuals (Sprite)
            GameObject visuals = new GameObject("Visuals");
            visuals.transform.SetParent(essenceObj.transform);
            SpriteRenderer sr = visuals.AddComponent<SpriteRenderer>();
            
            // Try to find the texture we just copied
            string texturePath = "Assets/Art/Items/CursedEssence_Icon.png";
            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
            if (tex != null)
            {
                // Note: User might need to change texture type to Sprite in Unity first
                sr.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                sr.color = new Color(1, 1, 1, 1);
            }
            else
            {
                Debug.LogWarning("[EssenceTool] Texture not found at " + texturePath + ". Assign it manually.");
            }

            // 4. Save as Prefab
            string prefabPath = "Assets/Prefabs/CursedEssence.prefab";
            bool success;
            PrefabUtility.SaveAsPrefabAsset(essenceObj, prefabPath, out success);
            
            DestroyImmediate(essenceObj);

            if (success)
            {
                Debug.Log("<color=green>[The Robot]</color> Cursed Essence Prefab created at: " + prefabPath);
                AssetDatabase.Refresh();
            }
        }
    }
}
