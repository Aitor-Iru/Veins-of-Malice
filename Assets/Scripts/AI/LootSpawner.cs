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

        [Header("Item Drops")]
        [SerializeField] private GameObject pickupPrefab; // Prefab with WorldItemPickup script
        [SerializeField] private World.ItemData[] possibleItems;
        [Range(0, 100)]
        [SerializeField] private float dropChance = 30f;

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

            // Chance to drop an item
            if (pickupPrefab != null && possibleItems != null && possibleItems.Length > 0)
            {
                if (Random.Range(0f, 100f) < dropChance)
                {
                    World.ItemData selectedItem = possibleItems[Random.Range(0, possibleItems.Length)];
                    Vector3 itemOffset = Random.insideUnitSphere * spawnRadius;
                    itemOffset.z = 0;
                    
                    GameObject pickupObj = Instantiate(pickupPrefab, transform.position + itemOffset, Quaternion.identity);
                    
                    // Assign item data to the pickup
                    if (pickupObj.TryGetComponent<World.WorldItemPickup>(out var pickup))
                    {
                        // Note: Requires a public setter or SerializedObject in a real scenario, 
                        // but since we are writing the script, I'll add a setter.
                        pickup.SetItem(selectedItem);
                    }
                }
            }
            
            Debug.Log($"<color=yellow>[Loot]</color> {gameObject.name} dropped loot.");
        }
    }
}
