using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace VeinsOfMalice.Editor
{
    public class URPMaterialFixer : EditorWindow
    {
        [MenuItem("Tools/Veins of Malice/Fix Magenta Materials (URP)")]
        public static void FixMaterials()
        {
            // 1. Create or load the default URP material
            string folderPath = "Assets/Art/Materials";
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                System.IO.Directory.CreateDirectory(folderPath);
                AssetDatabase.Refresh();
            }

            string defaultMatPath = folderPath + "/URP_Default_Lit.mat";
            Material defaultUrpMat = AssetDatabase.LoadAssetAtPath<Material>(defaultMatPath);

            if (defaultUrpMat == null)
            {
                Shader urpShader = Shader.Find("Universal Render Pipeline/Lit");
                if (urpShader != null)
                {
                    defaultUrpMat = new Material(urpShader);
                    AssetDatabase.CreateAsset(defaultUrpMat, defaultMatPath);
                }
            }

            // 2. Fix all material assets (.mat files) in the project
            string[] materialGuids = AssetDatabase.FindAssets("t:Material");
            int fixedMaterialsCount = 0;

            foreach (string guid in materialGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                
                // Skip package materials or internal editor assets
                if (path.StartsWith("Packages/") || !path.StartsWith("Assets/"))
                    continue;

                Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (mat == null || mat.shader == null) continue;

                string shaderName = mat.shader.name;
                string matNameLower = mat.name.ToLower();

                // Skip shaders that are already compatible with URP
                if (shaderName.StartsWith("Universal Render Pipeline/") ||
                    shaderName.StartsWith("Shader Graphs/") ||
                    shaderName.StartsWith("TextMeshPro/") ||
                    shaderName.StartsWith("GUI/") ||
                    shaderName.StartsWith("UI/") ||
                    shaderName.StartsWith("Sprites/") ||
                    shaderName.StartsWith("Skybox/") ||
                    shaderName.StartsWith("Hidden/"))
                {
                    continue;
                }

                // Identify target URP shader
                string targetShaderName = "Universal Render Pipeline/Lit";
                if (shaderName.StartsWith("Particles/"))
                {
                    targetShaderName = shaderName.Contains("Unlit") 
                        ? "Universal Render Pipeline/Particles/Unlit" 
                        : "Universal Render Pipeline/Particles/Lit";
                }
                else if (shaderName == "Unlit/Texture" || shaderName == "Unlit/Color")
                {
                    targetShaderName = "Universal Render Pipeline/Unlit";
                }
                else if (shaderName.StartsWith("Nature/Terrain/") || shaderName.StartsWith("Terrain/"))
                {
                    targetShaderName = "Universal Render Pipeline/Terrain/Lit";
                }

                Shader newShader = Shader.Find(targetShaderName);
                if (newShader == null) continue;

                // Cache original properties to map them to URP naming
                Color originalColor = mat.HasProperty("_Color") ? mat.GetColor("_Color") : Color.white;
                Texture originalMainTex = mat.HasProperty("_MainTex") ? mat.GetTexture("_MainTex") : null;
                Vector2 originalMainTexScale = mat.HasProperty("_MainTex") ? mat.GetTextureScale("_MainTex") : Vector2.one;
                Vector2 originalMainTexOffset = mat.HasProperty("_MainTex") ? mat.GetTextureOffset("_MainTex") : Vector2.zero;
                
                Texture originalBumpMap = mat.HasProperty("_BumpMap") ? mat.GetTexture("_BumpMap") : null;
                float originalBumpScale = mat.HasProperty("_BumpScale") ? mat.GetFloat("_BumpScale") : 1f;

                float originalSmoothness = 0.5f;
                if (mat.HasProperty("_Glossiness"))
                    originalSmoothness = mat.GetFloat("_Glossiness");
                else if (mat.HasProperty("_GlossMapScale"))
                    originalSmoothness = mat.GetFloat("_GlossMapScale");

                float originalMetallic = mat.HasProperty("_Metallic") ? mat.GetFloat("_Metallic") : 0f;
                Texture originalMetallicGlossMap = mat.HasProperty("_MetallicGlossMap") ? mat.GetTexture("_MetallicGlossMap") : null;

                // Detect double-sided requirement (e.g. leaves, hair, clothes)
                bool isDoubleSided = shaderName.Contains("NoCulling") || shaderName.Contains("_ds") || 
                                     matNameLower.Contains("leaves") || matNameLower.Contains("branch") || 
                                     matNameLower.Contains("hair") || matNameLower.Contains("grass");

                // Detect if it needs alpha clipping (Cutout)
                bool isCutout = false;
                if (mat.shaderKeywords.Length > 0)
                {
                    foreach (var kw in mat.shaderKeywords)
                    {
                        if (kw == "_ALPHATEST_ON") { isCutout = true; break; }
                    }
                }
                if (mat.HasProperty("_Mode") && mat.GetFloat("_Mode") == 1f) isCutout = true;
                if (matNameLower.Contains("branch") || matNameLower.Contains("leaves") || 
                    matNameLower.Contains("grass") || matNameLower.Contains("eyelash") || 
                    shaderName.Contains("Cutout") || shaderName.Contains("NoCulling"))
                {
                    isCutout = true;
                }

                // Detect if it needs transparency
                bool isTransparent = false;
                if (mat.HasProperty("_Mode") && (mat.GetFloat("_Mode") == 2f || mat.GetFloat("_Mode") == 3f)) isTransparent = true;
                if (shaderName.Contains("Blend") || matNameLower.Contains("blend") || 
                    matNameLower.Contains("cheek") || matNameLower.Contains("eye"))
                {
                    isTransparent = true;
                    isCutout = false; // Transparency takes precedence
                }

                // Clear leftover legacy keywords that could corrupt URP rendering modes
                mat.shaderKeywords = new string[0];

                // Set alpha to 1.0 for opaque and cutout materials to prevent them from becoming invisible in URP
                if (!isTransparent)
                {
                    originalColor.a = 1f;
                }

                // Apply new shader
                mat.shader = newShader;

                // Re-apply properties to URP shader variables
                if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", originalColor);
                if (mat.HasProperty("_Cutoff") && isCutout)
                {
                    float cutoff = mat.GetFloat("_Cutoff");
                    if (cutoff <= 0f) cutoff = 0.5f; // Keep original or default to 0.5f if 0
                    mat.SetFloat("_Cutoff", cutoff);
                }
                if (mat.HasProperty("_BaseMap") && originalMainTex != null)
                {
                    mat.SetTexture("_BaseMap", originalMainTex);
                    mat.SetTextureScale("_BaseMap", originalMainTexScale);
                    mat.SetTextureOffset("_BaseMap", originalMainTexOffset);
                }
                if (mat.HasProperty("_BumpMap") && originalBumpMap != null)
                {
                    mat.SetTexture("_BumpMap", originalBumpMap);
                    if (mat.HasProperty("_BumpScale")) mat.SetFloat("_BumpScale", originalBumpScale);
                }
                if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", originalSmoothness);
                if (mat.HasProperty("_Metallic")) mat.SetFloat("_Metallic", originalMetallic);
                if (mat.HasProperty("_MetallicGlossMap") && originalMetallicGlossMap != null) mat.SetTexture("_MetallicGlossMap", originalMetallicGlossMap);

                // Set Double Sided rendering if needed
                if (isDoubleSided && mat.HasProperty("_Cull"))
                {
                    mat.SetFloat("_Cull", 0f); // Cull Off (Double-sided)
                }

                if (mat.HasProperty("_QueueControl")) mat.SetFloat("_QueueControl", 0f); // Auto queue control
                if (mat.HasProperty("_QueueOffset")) mat.SetFloat("_QueueOffset", 0f);

                // Set Rendering Mode properties and precise URP keywords
                if (isCutout)
                {
                    mat.SetFloat("_Surface", 0f); // Opaque
                    mat.SetFloat("_AlphaClip", 1f);
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    mat.SetInt("_ZWrite", 1);
                    
                    // Disable Specular Highlights and reflections to remove the blue environment tint
                    mat.SetFloat("_SpecularHighlights", 0f);
                    mat.SetFloat("_EnvironmentReflections", 0f);
                    
                    List<string> keywords = new List<string> { "_ALPHATEST_ON", "_SPECULARHIGHLIGHTS_OFF", "_ENVIRONMENTREFLECTIONS_OFF" };
                    if (isDoubleSided) keywords.Add("_DOUBLE_SIDED_ON");
                    mat.shaderKeywords = keywords.ToArray();

                    mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
                }
                else if (isTransparent)
                {
                    mat.SetFloat("_Surface", 1f); // Transparent
                    mat.SetFloat("_Blend", 0f); // Alpha blend
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    mat.SetInt("_ZWrite", 0);
                    
                    mat.shaderKeywords = new string[] { "_SURFACE_TYPE_TRANSPARENT" };
                    mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                }
                else
                {
                    mat.SetFloat("_Surface", 0f); // Opaque
                    mat.SetFloat("_AlphaClip", 0f);
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    mat.SetInt("_ZWrite", 1);
                    
                    mat.shaderKeywords = new string[0];
                    mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;
                }

                EditorUtility.SetDirty(mat);
                fixedMaterialsCount++;
                Debug.Log($"[The Robot v3] Converted material '{mat.name}' ({shaderName} -> {targetShaderName})");
            }

            // 3. Fix built-in default materials in prefabs (only if they are missing or using standard read-only default-material)
            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");
            int fixedPrefabsCount = 0;

            foreach (string guid in prefabGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                
                // Skip Packages folder
                if (!path.StartsWith("Assets/"))
                    continue;

                GameObject instance = PrefabUtility.LoadPrefabContents(path);
                if (instance == null) continue;

                Renderer[] renderers = instance.GetComponentsInChildren<Renderer>(true);
                bool changed = false;

                foreach (Renderer r in renderers)
                {
                    if (r is MeshRenderer || r is SkinnedMeshRenderer)
                    {
                        Material[] materials = r.sharedMaterials;
                        for (int i = 0; i < materials.Length; i++)
                        {
                            // If the material is null, or is the Unity built-in default standard material
                            // We check the asset path: if it doesn't start with "Assets/", it means it's a built-in read-only material
                            if (materials[i] == null || !AssetDatabase.GetAssetPath(materials[i]).StartsWith("Assets/"))
                            {
                                if (defaultUrpMat != null)
                                {
                                    materials[i] = defaultUrpMat;
                                    changed = true;
                                }
                            }
                        }
                        if (changed)
                        {
                            r.sharedMaterials = materials;
                        }
                    }
                }

                if (changed)
                {
                    PrefabUtility.SaveAsPrefabAsset(instance, path);
                    fixedPrefabsCount++;
                }
                PrefabUtility.UnloadPrefabContents(instance);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"<color=green>[The Robot v3]</color> Fix complete! Successfully upgraded {fixedMaterialsCount} material assets, and fixed default materials in {fixedPrefabsCount} prefabs across the project.");
        }
    }
}
