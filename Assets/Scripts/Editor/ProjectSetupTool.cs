using UnityEngine;
using UnityEditor;

public class ProjectSetupTool : EditorWindow
{
    [MenuItem("Tools/Veins of Malice/Setup Tags and Layers")]
    public static void SetupTagsAndLayers()
    {
        bool success = true;
        
        // Define Layers and Tags from Roadmap
        string[] sortingLayers = new string[] { "Background", "Middleground", "Player", "Foreground", "UI" };
        string[] tags = new string[] { "Ground", "Wall", "MiniBoss", "Boss" }; // Player, Enemy and MainCamera are default tags or will be added

        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

        // Setup Sorting Layers
        SerializedProperty sortingLayersProp = tagManager.FindProperty("m_SortingLayers");
        
        foreach (string layerName in sortingLayers)
        {
            if (!PropertyExists(sortingLayersProp, "name", layerName))
            {
                int index = sortingLayersProp.arraySize;
                sortingLayersProp.InsertArrayElementAtIndex(index);
                SerializedProperty layer = sortingLayersProp.GetArrayElementAtIndex(index);
                layer.FindPropertyRelative("name").stringValue = layerName;
                layer.FindPropertyRelative("uniqueID").intValue = layerName.GetHashCode(); // Simple ID generation
                Debug.Log($"Created Sorting Layer: {layerName}");
            }
        }

        // Setup Tags
        SerializedProperty tagsProp = tagManager.FindProperty("tags");
        
        foreach (string tagName in tags)
        {
            if (!GetTagExists(tagsProp, tagName))
            {
                int index = tagsProp.arraySize;
                tagsProp.InsertArrayElementAtIndex(index);
                SerializedProperty tag = tagsProp.GetArrayElementAtIndex(index);
                tag.stringValue = tagName;
                Debug.Log($"Created Tag: {tagName}");
            }
        }

        tagManager.ApplyModifiedProperties();

        // Setup Physics Layers
        SetupPhysicsLayers(tagManager);
        
        // Setup Camera
        SetupCameraCulling();

        Debug.Log("Tags, Layers, and Camera Setup Complete.");
    }

    private static void SetupPhysicsLayers(SerializedObject tagManager)
    {
        SerializedProperty layersProp = tagManager.FindProperty("layers");
        // Layer 0-5 are Builtin. 6+ are User defined.
        // We want: 6: Player, 7: Enemy, 8: Ground, 9: Gameplay
        string[] layerNames = new string[] { "Player", "Enemy", "Ground", "Gameplay" };
        int startIndex = 6;

        for (int i = 0; i < layerNames.Length; i++)
        {
            int layerIndex = startIndex + i;
            SerializedProperty layerProp = layersProp.GetArrayElementAtIndex(layerIndex);
            if (string.IsNullOrEmpty(layerProp.stringValue))
            {
                layerProp.stringValue = layerNames[i];
                Debug.Log($"Defined Layer {layerIndex}: {layerNames[i]}");
            }
        }
        tagManager.ApplyModifiedProperties();
    }

    private static void SetupCameraCulling()
    {
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            // Set Culling Mask to include everything EXCEPT UI (Layer 5)
            // or explicitly include Default (0) + Player (6) + Enemy (7) + Ground (8) + Gameplay (9)
            
            // Generic approach: Everything except UI
            int uiLayer = LayerMask.NameToLayer("UI");
            if (uiLayer > 0)
            {
                mainCam.cullingMask = ~(1 << uiLayer); // ~Bitmask inverts it, so "Everything BUT UI"
                Debug.Log("Main Camera Culling Mask updated (Excluding UI).");
            }
        }
    }

    private static bool PropertyExists(SerializedProperty arrayProp, string relativePropName, string value)
    {
        for (int i = 0; i < arrayProp.arraySize; i++)
        {
            SerializedProperty element = arrayProp.GetArrayElementAtIndex(i);
            if (element.FindPropertyRelative(relativePropName).stringValue == value)
            {
                return true;
            }
        }
        return false;
    }

    private static bool GetTagExists(SerializedProperty tagsProp, string tagName)
    {
        for (int i = 0; i < tagsProp.arraySize; i++)
        {
            SerializedProperty element = tagsProp.GetArrayElementAtIndex(i);
            if (element.stringValue == tagName)
            {
                return true;
            }
        }
        return false;
    }
}
