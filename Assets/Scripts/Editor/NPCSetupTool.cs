using UnityEngine;
using UnityEditor;
using VeinsOfMalice.World;

namespace VeinsOfMalice.Editor
{
    public class NPCSetupTool : EditorWindow
    {
        [MenuItem("Tools/Veins of Malice/Create NPC Triangle")]
        public static void CreateNPCTriangle()
        {
            GameObject npc = GameObject.CreatePrimitive(PrimitiveType.Cube); // Temporary mesh
            npc.name = "NPC_Triangle";
            
            // Try to make it look like a triangle/cone for now
            npc.transform.localScale = new Vector3(1, 1, 0.2f);
            
            // Add components
            npc.AddComponent<InteractableNPC>();
            int gameplayLayer = LayerMask.NameToLayer("Gameplay");
            if (gameplayLayer != -1) npc.layer = gameplayLayer;
            else Debug.LogWarning("<color=red>[NPC Tool]</color> 'Gameplay' layer not found! Using Default.");
            
            // Add a trigger collider for detection if not already present
            SphereCollider trigger = npc.AddComponent<SphereCollider>();
            trigger.isTrigger = false; // Make it solid if it's an NPC, or true if just a trigger. 
            // Actually, the PlayerInteraction uses OverlapSphere, which works with both triggers and non-triggers.
            // But usually NPCs have physics. Let's keep it as is or make it a normal collider.
            trigger.radius = 1f; // Visual size is 1x1x0.2, so radius 1 is plenty.
            
            // Move it to the center of view but force Z to 0 for 2.5D
            SceneView sceneView = SceneView.lastActiveSceneView;
            if (sceneView != null)
            {
                Vector3 pos = sceneView.pivot;
                pos.z = 0;
                npc.transform.position = pos;
            }

            Selection.activeGameObject = npc;
            Debug.Log("<color=green>[NPC Tool]</color> Created NPC with InteractableNPC script.");
        }
    }
}
