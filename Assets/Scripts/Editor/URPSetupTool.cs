using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.IO;

public class URPSetupTool
{
    private const string AssetPath = "Assets/Settings/VeinsOfMalice_URP.asset";
    private const string DataPath = "Assets/Settings/VeinsOfMalice_URP_Renderer.asset";

    [MenuItem("Veins of Malice/Setup/Force Reinstall URP Config")]
    public static void ForceSetup()
    {
        SetupURP();
    }

    [InitializeOnLoadMethod]
    private static void SetupURP()
    {
        // 1. Check if URP is already configured
        if (GraphicsSettings.defaultRenderPipeline != null && GraphicsSettings.defaultRenderPipeline.GetType() == typeof(UniversalRenderPipelineAsset))
        {
            // Already set up
            return;
        }

        Debug.Log("Configuring URP for Veins of Malice...");

        // Ensure Settings folder exists
        if (!Directory.Exists("Assets/Settings"))
        {
            Directory.CreateDirectory("Assets/Settings");
        }

        // 2. Create Renderer Data (Forward Renderer)
        UniversalRendererData rendererData = AssetDatabase.LoadAssetAtPath<UniversalRendererData>(DataPath);
        if (rendererData == null)
        {
            rendererData = ScriptableObject.CreateInstance<UniversalRendererData>();
            rendererData.name = "VeinsOfMalice_URP_Renderer";
            
            // Enable Post Processing by default
            rendererData.postProcessData = AssetDatabase.LoadAssetAtPath<PostProcessData>("Packages/com.unity.render-pipelines.universal/Runtime/Data/PostProcessData.asset");
            
            AssetDatabase.CreateAsset(rendererData, DataPath);
            Debug.Log("Created URP Renderer Data.");
        }

        // 3. Create Pipeline Asset
        UniversalRenderPipelineAsset pipelineAsset = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(AssetPath);
        if (pipelineAsset == null)
        {
            pipelineAsset = UniversalRenderPipelineAsset.Create(rendererData);
            pipelineAsset.name = "VeinsOfMalice_URP";

            // Configure for High Quality (Desktops)
            pipelineAsset.shadowDistance = 50f;
            pipelineAsset.shadowCascadeCount = 4;
            pipelineAsset.supportsHDR = true;
            pipelineAsset.msaaSampleCount = 4;
            pipelineAsset.renderScale = 1.0f;

            AssetDatabase.CreateAsset(pipelineAsset, AssetPath);
            Debug.Log("Created URP Pipeline Asset.");
        }

        // 4. Assign to Graphics Settings
        GraphicsSettings.defaultRenderPipeline = pipelineAsset;
        QualitySettings.renderPipeline = pipelineAsset; // Assign to current level

        // 5. Save Changes
        AssetDatabase.SaveAssets();
        Debug.Log("URP Configured Successfully! Lit Shaders are now ready.");
    }
}
