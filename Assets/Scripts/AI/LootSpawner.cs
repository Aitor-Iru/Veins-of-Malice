using UnityEngine;
using VeinsOfMalice.AI;

namespace VeinsOfMalice.AI
{
    /// <summary>
    /// LootSpawner — Genera botín al morir el enemigo.
    /// </summary>
    [RequireComponent(typeof(EnemyHealth))]
    public class LootSpawner : MonoBehaviour
    {
        [Header("Loot Settings")]
        [SerializeField] private GameObject essencePrefab;
        [SerializeField] private int minEssence = 1;
        [SerializeField] private int maxEssence = 3;
        [SerializeField] private float spawnRadius = 0.5f;

        private EnemyHealth health;

        private void Awake()
        {
            health = GetComponent<EnemyHealth>();
        }

        private void OnEnable()
        {
            if (health != null) health.OnDeath += SpawnLoot;
        }

        private void OnDisable()
        {
            if (health != null) health.OnDeath -= SpawnLoot;
        }

        private void SpawnLoot()
        {
            if (essencePrefab == null) return;

            int count = 1; // Fixed to 1 as requested
            
            for (int i = 0; i < count; i++)
            {
                Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
                randomOffset.z = 0;
                
                Instantiate(essencePrefab, transform.position + randomOffset, Quaternion.identity);
            }
            
            Debug.Log($"<color=yellow>[Loot]</color> {gameObject.name} dropped {count} essence(s).");
        }
    }
}
