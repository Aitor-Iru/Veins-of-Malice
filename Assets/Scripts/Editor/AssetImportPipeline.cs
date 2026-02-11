using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;

public class AssetImportPipeline : AssetPostprocessor
{
    // Define the target shader for materials
    private const string TargetShaderName = "Universal Render Pipeline/Lit";

    /// <summary>
    /// Applies standard settings to Models (FBX, Blend, etc.) upon import.
    /// </summary>
    void OnPreprocessModel()
    {
        ModelImporter modelImporter = assetImporter as ModelImporter;
        if (modelImporter == null) return;

        // --- Clean up Import ---
        // We don't want cameras or lights from Blender scenes
        modelImporter.importCameras = false;
        modelImporter.importLights = false;

        // --- Optimization ---
        // Combine meshes where possible
        modelImporter.optimizeMeshVertices = true; 
        modelImporter.optimizeMeshPolygons = true;
        
        // Medium compression is usually a good balance for this style
        modelImporter.meshCompression = ModelImporterMeshCompression.Medium;

        // --- Hierarchy ---
        // Preserving hierarchy is usually safer for rigged characters, 
        // but for static props, we might want to enabling 'Preserve Hierarchy' only if needed.
        // For now, let's leave it default (unchecked usually for props, checked for characters).
        
        // --- Animation ---
        // If the file name contains "@", it's likely an animation clip file.
        if (!assetPath.Contains("@"))
        {
            // Likely a mesh definition, ensure read/write enabled if we need to access mesh data at runtime (e.g. VFX)
            // modelImporter.isReadable = true; // Unleash only if necessary to save memory
        }
    }

    /// <summary>
    /// Enforces URP Lit Material creation if materials are imported.
    /// </summary>
    void OnPreprocessMaterialDescription(MaterialDescription description, Material material, AnimationClip[] materialAnimation)
    {
        // Only affect new materials or when re-importing with "Regenerate Materials"
        // We want to ensure it uses URP Lit
        
        Shader urpLit = Shader.Find(TargetShaderName);
        if (urpLit == null)
        {
            Debug.LogWarning($"[AssetImportPipeline] Could not find shader '{TargetShaderName}'. Ensure URP is installed.");
            return;
        }

        if (material.shader.name != TargetShaderName)
        {
            material.shader = urpLit;
        }
        
        // Optional: Map textures if they follow standard naming (Albedo, Normal, etc.)
        // This is handled by Unity's default mapper usually, but we can enforce it here if needed.
    }
}
