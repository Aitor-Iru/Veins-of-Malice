using UnityEngine;
using UnityEditor;

// Enforces art standards upon import
public class AssetImportPipeline : AssetPostprocessor
{
    // Standard PPU for the project
    private const int TargetPPU = 100; 

    void OnPreprocessTexture()
    {
        // Only apply to assets in the "Art" folder to avoid messing with packages or UI default resources
        if (!assetPath.Contains("Assets/Art")) return;

        TextureImporter importer = (TextureImporter)assetImporter;

        // General 2D Settings
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single; 
        importer.spritePixelsPerUnit = TargetPPU;
        
        // "Stylized 2D without pixel art" -> High quality, Bilinear usually preferred over Point
        importer.filterMode = FilterMode.Bilinear; 
        
        // Compression Settings (High Quality for 2D usually matters more than extreme compression)
        TextureImporterPlatformSettings settings = new TextureImporterPlatformSettings();
        settings.overridden = true;
        settings.name = "Standalone";
        settings.maxTextureSize = 2048;
        settings.format = TextureImporterFormat.RGBA32; // Uncompressed/High Quality
        
        importer.SetPlatformTextureSettings(settings);
    }

    void OnPreprocessModel()
    {
        // Only apply to assets in the "Art" folder
        if (!assetPath.Contains("Assets/Art")) return;

        ModelImporter importer = (ModelImporter)assetImporter;

        // General Model Settings for 2.5D
        importer.globalScale = 1.0f;
        importer.useFileScale = true;
        // importer.importCameras = false; // Deprecated/Removed in newer Unity versions
        // importer.importLights = false;  // Deprecated/Removed in newer Unity versions
        
        // Mesh Settings
        importer.meshCompression = ModelImporterMeshCompression.Medium;
        importer.isReadable = false; // Optimize memory unless needed for scripts
        importer.optimizeMesh = true; 

        // Animation Settings (Mecanim)
        // Default to Generic for monsters/props, user can change to Humanoid manually if needed
        importer.animationType = ModelImporterAnimationType.Generic; 
        importer.importAnimation = true;
        importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;

        // Material Settings
        // We generally want to use external materials or embedded if properly set up
        importer.materialImportMode = ModelImporterMaterialImportMode.ImportViaMaterialDescription;
        // importer.materialLocation = ModelImporterMaterialLocation.Embedded; // Causing errors, skipping for now
        // importer.materialName = ModelImporterMaterialName.BasedOnMaterialName;
        // importer.materialSearch = ModelImporterMaterialSearch.Local;
    }
}
