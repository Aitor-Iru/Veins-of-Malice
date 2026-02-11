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
        string[] tags = new string[] { "Kenny", "Ground", "Wall" }; // Player and MainCamera are default tags

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
        Debug.Log("Tags and Layers Setup Complete.");
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
