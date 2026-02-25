using UnityEngine;
using UnityEditor;
using VeinsOfMalice.Player;
using VeinsOfMalice.UI;

namespace VeinsOfMalice.Editor
{
    public class CursedEnergySetupTool : EditorWindow
    {
        [MenuItem("Tools/Veins of Malice/Fix Cursed Energy Setup")]
        public static void FixSetup()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                Debug.LogError("[CursedEnergySetupTool] Player not found! Make sure your player has the 'Player' tag.");
                return;
            }

            // 1. Ensure PlayerEnergy exists and is wired
            PlayerEnergy energy = player.GetComponent<PlayerEnergy>();
            if (energy == null) energy = player.AddComponent<PlayerEnergy>();

            // Find InputReader asset
            InputReader inputReader = AssetDatabase.LoadAssetAtPath<InputReader>("Assets/Input/InputReader.asset");
            if (inputReader == null)
            {
                // Try alternate path if not found
                string[] guids = AssetDatabase.FindAssets("t:InputReader");
                if (guids.Length > 0)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    inputReader = AssetDatabase.LoadAssetAtPath<InputReader>(path);
                }
            }

            if (inputReader != null)
            {
                // Assign InputReader to Energy
                var energySerialized = new SerializedObject(energy);
                energySerialized.FindProperty("inputReader").objectReferenceValue = inputReader;
                energySerialized.ApplyModifiedProperties();
                
                // Assign InputReader to PlayerCombat (for its own input if needed)
                PlayerCombat combat = player.GetComponent<PlayerCombat>();
                if (combat != null)
                {
                    var combatSerialized = new SerializedObject(combat);
                    combatSerialized.FindProperty("inputReader").objectReferenceValue = inputReader;
                    // Also wire PlayerEnergy to PlayerCombat for the damage boost
                    combatSerialized.FindProperty("playerEnergy").objectReferenceValue = energy;
                    combatSerialized.ApplyModifiedProperties();
                }
            }
            else
            {
                Debug.LogError("[CursedEnergySetupTool] Could not find InputReader asset!");
            }

            // 2. Ensure CursedAura exists
            CursedAura aura = player.GetComponent<CursedAura>();
            if (aura == null) aura = player.AddComponent<CursedAura>();
            
            var auraSerialized = new SerializedObject(aura);
            auraSerialized.FindProperty("playerEnergy").objectReferenceValue = energy;
            auraSerialized.ApplyModifiedProperties();

            // 3. Link HUDManager
            HUDManager hud = Object.FindFirstObjectByType<HUDManager>();
            if (hud != null)
            {
                var hudSerialized = new SerializedObject(hud);
                hudSerialized.FindProperty("playerEnergy").objectReferenceValue = energy;
                hudSerialized.ApplyModifiedProperties();
                Debug.Log("[CursedEnergySetupTool] HUDManager connected to PlayerEnergy.");
            }

            EditorUtility.SetDirty(player);
            if (hud != null) EditorUtility.SetDirty(hud);
            
            Debug.Log("<color=green><b>[SUCCESS]</b></color> Cursed Energy system wired correctly on " + player.name);
            
            // Focus on the player to see the results
            Selection.activeGameObject = player;
        }
    }
}
