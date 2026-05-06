using UnityEngine;
using System;

namespace VeinsOfMalice.Player
{
    public enum PlayerGrade
    {
        Grade4,
        Grade3,
        SemiGrade2,
        Grade2,
        SemiGrade1,
        Grade1,
        SpecialGrade
    }

    /// <summary>
    /// PlayerExperience — Gestiona los niveles, la cantidad de XP y el Grado del jugador.
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
        [SerializeField] private PlayerGrade currentGrade = PlayerGrade.Grade4;
        
        private PlayerEnergy playerEnergy;

        // ── Events ────────────────────────────────────────────────────────────────
        public event Action<int, int> OnXPChanged;       // parameters: currentXP, xpToNextLevel
        public event Action<int> OnLevelChanged;         // parameter: newLevel
        public event Action<PlayerGrade> OnGradeChanged; // parameter: newGrade
        public event Action OnGradeUpEvent;              // Se dispara al subir de grado

        public int CurrentLevel => currentLevel;
        public int CurrentXP => currentXP;
        public int XPPerLevel => xpPerLevel;
        public int MaxLevel => maxLevel; 
        public PlayerGrade CurrentGrade => currentGrade;

        private void Start()
        {
            playerEnergy = GetComponent<PlayerEnergy>();
            
            // Initial UI trigger
            OnXPChanged?.Invoke(currentXP, xpPerLevel);
            OnLevelChanged?.Invoke(currentLevel);
            OnGradeChanged?.Invoke(currentGrade);
        }

        public void AddXP(int amount)
        {
            if (currentLevel >= AbsoluteMaxLevel && currentGrade == PlayerGrade.SpecialGrade) return;

            currentXP += amount;
            Debug.Log($"<color=cyan>[XP]</color> Gained {amount} XP. Total: {currentXP}/{xpPerLevel}");

            bool leveledUp = false;

            // Bucle en caso de que gane mucha experiencia de una sola vez
            // AHORA SE DETIENE EN maxLevel (100, 200, etc.)
            while (currentXP >= xpPerLevel && currentLevel < maxLevel)
            {
                currentXP -= xpPerLevel;
                currentLevel++;
                leveledUp = true;
                Debug.Log($"<color=green>[Level Up!]</color> Reached Level {currentLevel}");

                // Si llegamos al tope de este tramo, nos detenemos
                if (currentLevel >= maxLevel)
                {
                    Debug.Log($"<color=yellow>[XP]</color> Reached Level {maxLevel}! Rebirth required.");
                    break; 
                }
            }

            // Si somos nivel máximo de este tramo, capamos XP
            if (currentLevel >= maxLevel)
            {
                currentXP = xpPerLevel;
            }

            if (leveledUp)
            {
                OnLevelChanged?.Invoke(currentLevel);
            }
            
            OnXPChanged?.Invoke(currentXP, xpPerLevel);
        }

        public bool TryRebirth()
        {
            // Si llegamos al nivel 500, toca Grade Up
            if (currentLevel >= AbsoluteMaxLevel)
            {
                return TryGradeUp();
            }

            // Si llegamos al nivel máximo actual (100, 200...), subimos el tramo
            if (currentLevel >= maxLevel && maxLevel < AbsoluteMaxLevel)
            {
                maxLevel += 100;
                if (maxLevel > AbsoluteMaxLevel) maxLevel = AbsoluteMaxLevel;
                
                // Bonus de energía por rebirth
                if (playerEnergy != null) playerEnergy.UpgradeMaxEnergy(50f);
                
                Debug.Log($"<color=magenta>[Rebirth]</color> New Max Level: {maxLevel}");
                
                OnLevelChanged?.Invoke(currentLevel);
                OnXPChanged?.Invoke(currentXP, xpPerLevel);
                return true;
            }
            return false;
        }

        public bool TryGradeUp()
        {
            if (currentLevel < AbsoluteMaxLevel) return false;
            if (currentGrade == PlayerGrade.SpecialGrade) return false;

            // Subir grado
            currentGrade++;
            
            // Reiniciar progreso para el nuevo grado
            currentLevel = 1;
            currentXP = 0;
            maxLevel = 100; 

            Debug.Log($"<color=gold><b>[GRADE UP!]</b></color> New Grade: {currentGrade}");

            OnGradeChanged?.Invoke(currentGrade);
            OnLevelChanged?.Invoke(currentLevel);
            OnXPChanged?.Invoke(currentXP, xpPerLevel);
            OnGradeUpEvent?.Invoke();

            return true;
        }

        public string GetGradeName()
        {
            switch (currentGrade)
            {
                case PlayerGrade.Grade4: return "GRADE 4";
                case PlayerGrade.Grade3: return "GRADE 3";
                case PlayerGrade.Grade2: return "GRADE 2";
                case PlayerGrade.SemiGrade2: return "SEMI-GRADE 2";
                case PlayerGrade.SemiGrade1: return "SEMI-GRADE 1";
                case PlayerGrade.Grade1: return "GRADE 1";
                case PlayerGrade.SpecialGrade: return "SPECIAL GRADE";
                default: return "UNKNOWN";
            }
        }
    }
}
