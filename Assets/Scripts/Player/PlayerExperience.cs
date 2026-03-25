using UnityEngine;
using System;

namespace VeinsOfMalice.Player
{
    /// <summary>
    /// PlayerExperience — Gestiona los niveles y la cantidad de XP del jugador.
    /// </summary>
    public class PlayerExperience : MonoBehaviour
    {
        public const int AbsoluteMaxLevel = 500;

        [Header("Experience Settings")]
        [SerializeField] private int maxLevel = 100;
        [SerializeField] private int xpPerLevel = 100;
        
        [Header("Current State")]
        [SerializeField] private int currentLevel = 1;
        [SerializeField] private int currentXP = 0;

        // ── Events ────────────────────────────────────────────────────────────────
        public event Action<int, int> OnXPChanged;       // parameters: currentXP, xpToNextLevel
        public event Action<int> OnLevelChanged;         // parameter: newLevel

        public int CurrentLevel => currentLevel;
        public int CurrentXP => currentXP;
        public int XPPerLevel => xpPerLevel;
        public int MaxLevel => maxLevel;

        private void Start()
        {
            // Initial UI trigger
            OnXPChanged?.Invoke(currentXP, xpPerLevel);
            OnLevelChanged?.Invoke(currentLevel);
        }

        public void AddXP(int amount)
        {
            if (currentLevel >= maxLevel) return; // Ya está al nivel máximo

            currentXP += amount;
            Debug.Log($"<color=cyan>[XP]</color> Gained {amount} XP. Total: {currentXP}/{xpPerLevel}");

            bool leveledUp = false;

            // Bucle en caso de que gane mucha experiencia de una sola vez
            while (currentXP >= xpPerLevel && currentLevel < maxLevel)
            {
                currentXP -= xpPerLevel;
                currentLevel++;
                leveledUp = true;
                Debug.Log($"<color=green>[Level Up!]</color> Reached Level {currentLevel}");
            }

            // Si tras el bucle somos nivel máximo, capamos la XP
            if (currentLevel >= maxLevel)
            {
                currentXP = xpPerLevel; // Se queda llena visualmente
            }

            if (leveledUp)
            {
                OnLevelChanged?.Invoke(currentLevel);
            }
            
            OnXPChanged?.Invoke(currentXP, xpPerLevel);
        }

        public bool TryRebirth()
        {
            if (currentLevel >= maxLevel && maxLevel < AbsoluteMaxLevel)
            {
                maxLevel += 100;
                if (maxLevel > AbsoluteMaxLevel) maxLevel = AbsoluteMaxLevel;
                
                Debug.Log($"<color=magenta>[Rebirth]</color> Rebirth successful! New Max Level: {maxLevel}");
                
                OnLevelChanged?.Invoke(currentLevel);
                OnXPChanged?.Invoke(currentXP, xpPerLevel);
                
                return true;
            }
            return false;
        }
    }
}
