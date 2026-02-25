using System;
using UnityEngine;

namespace VeinsOfMalice.Player
{
    /// <summary>
    /// PlayerEnergy — Gestiona el pool de Energía Maldita, su consumo y recarga.
    /// </summary>
    public class PlayerEnergy : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private InputReader inputReader;

        [Header("Settings")]
        [SerializeField] private float maxEnergy = 100f;
        [SerializeField] private float passiveDrainRate = 2f; // Energía por segundo cuando está "quieto"
        [SerializeField] private float regenRate = 5f;        // Energía por segundo al estar inactivo
        [SerializeField] private float minEnergyToStart = 20f;    // Energía mínima para poder activar el modo

        // ── State ─────────────────────────────────────────────────────────────────
        private float currentEnergy;
        private bool isEnergyModeActive;

        // ── Events ────────────────────────────────────────────────────────────────
        public event Action<float, float> OnEnergyChanged; // (current, max)
        public event Action<bool> OnEnergyModeToggled;     // (isActive)

        private void Awake()
        {
            currentEnergy = maxEnergy;
        }

        private void OnEnable()
        {
            if (inputReader != null)
            {
                inputReader.OnToggleEnergyStarted += ToggleEnergyMode;
                Debug.Log("<color=green>[PlayerEnergy]</color> Subscribed to OnToggleEnergyStarted");
            }
            else
            {
                Debug.LogWarning("<color=red>[PlayerEnergy]</color> InputReader is NULL! Assign it in the Inspector.");
            }
        }

        private void OnDisable()
        {
            if (inputReader != null)
                inputReader.OnToggleEnergyStarted -= ToggleEnergyMode;
        }

        private void Update()
        {
            if (isEnergyModeActive)
            {
                DrainEnergy();
            }
            else
            {
                RegenerateEnergy();
            }
        }

        private void ToggleEnergyMode()
        {
            Debug.Log("[PlayerEnergy] ToggleEnergyMode called");
            if (!isEnergyModeActive)
            {
                // Intentar activar
                if (currentEnergy >= minEnergyToStart)
                {
                    SetEnergyMode(true);
                }
                else
                {
                    Debug.Log("<color=orange>[Energy]</color> Not enough energy to activate!");
                }
            }
            else
            {
                // Desactivar manualmente
                SetEnergyMode(false);
            }
        }

        private void SetEnergyMode(bool active)
        {
            if (isEnergyModeActive == active) return;

            isEnergyModeActive = active;
            OnEnergyModeToggled?.Invoke(isEnergyModeActive);
            
            Debug.Log(isEnergyModeActive ? 
                "<color=cyan><b>[ENERGY MODE]</b></color> Aura Active!" : 
                "<color=gray>[ENERGY MODE]</color> Aura Deactivated.");
        }

        private void DrainEnergy()
        {
            currentEnergy -= passiveDrainRate * Time.deltaTime;
            currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
            
            OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);

            if (currentEnergy <= 0)
            {
                SetEnergyMode(false);
            }
        }

        private void RegenerateEnergy()
        {
            if (currentEnergy < maxEnergy)
            {
                currentEnergy += regenRate * Time.deltaTime;
                currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
                
                OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);
            }
        }

        // ── Public API ────────────────────────────────────────────────────────────
        public bool IsEnergyModeActive => isEnergyModeActive;
        public float CurrentEnergyNormalized => currentEnergy / maxEnergy;

        /// <summary>
        /// Reduce la energía instantáneamente (por ejemplo, al atacar).
        /// </summary>
        public void UseEnergy(float amount)
        {
            if (!isEnergyModeActive) return;

            currentEnergy -= amount;
            currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
            
            OnEnergyChanged?.Invoke(currentEnergy, maxEnergy);

            if (currentEnergy <= 0)
            {
                SetEnergyMode(false);
            }
        }
    }
}
