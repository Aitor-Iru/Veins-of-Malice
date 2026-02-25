using UnityEngine;

namespace VeinsOfMalice.Player
{
    /// <summary>
    /// CursedAura — Controla el aspecto visual de la activación del modo energía.
    /// Crea un aura cian sutil alrededor del jugador.
    /// </summary>
    public class CursedAura : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerEnergy playerEnergy;
        [SerializeField] private GameObject auraVisual; // El objeto que contiene el aura (p.ej. un cilindro semi-transparente)

        [Header("Settings")]
        [SerializeField] private Color auraColor = new Color(0, 1, 1, 0.2f); // Cian sutil
        [SerializeField] private float pulseSpeed = 2f;
        [SerializeField] private float minAlpha = 0.05f;
        [SerializeField] private float maxAlpha = 0.25f;

        private Material auraMaterial;
        private bool isEffectActive;

        private void Awake()
        {
            if (playerEnergy == null)
                playerEnergy = GetComponentInParent<PlayerEnergy>();

            // Si no tenemos un objeto visual, lo creamos dinámicamente como fallback
            if (auraVisual == null)
            {
                CreateAuraPrimitive();
            }

            if (auraVisual != null)
            {
                Renderer rend = auraVisual.GetComponent<Renderer>();
                if (rend) auraMaterial = rend.material;
                auraVisual.SetActive(false);
            }
        }

        private void OnEnable()
        {
            if (playerEnergy != null)
                playerEnergy.OnEnergyModeToggled += HandleModeToggled;
        }

        private void OnDisable()
        {
            if (playerEnergy != null)
                playerEnergy.OnEnergyModeToggled -= HandleModeToggled;
        }

        private void Update()
        {
            if (isEffectActive && auraMaterial != null)
            {
                // Efecto de pulso suave
                float alpha = Mathf.Lerp(minAlpha, maxAlpha, (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f);
                Color c = auraColor;
                c.a = alpha;
                auraMaterial.color = c;
            }
        }

        private void HandleModeToggled(bool active)
        {
            isEffectActive = active;
            if (auraVisual != null) auraVisual.SetActive(active);
        }

        private void CreateAuraPrimitive()
        {
            // Creamos un cilindro sutil alrededor del jugador si no hay un prefab asignado
            GameObject aura = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            aura.name = "Aura_Cyan_Auto";
            aura.transform.SetParent(transform);
            aura.transform.localPosition = new Vector3(0, 1, 0); // Centrado en un personaje de 2m
            aura.transform.localScale = new Vector3(1.2f, 1.1f, 1.2f); // Un poco más ancho que el jugador
            
            // Quitar el collider para que no interfiera con físicas
            Destroy(aura.GetComponent<Collider>());

            Renderer rend = aura.GetComponent<Renderer>();
            if (rend)
            {
                // Usar un shader transparente si es posible (fallback a Lit con transparencia)
                rend.material = new Material(Shader.Find("Sprites/Default")); // Usamos Shaders simples para el aura sutil
                rend.material.color = auraColor;
            }

            auraVisual = aura;
        }
    }
}
