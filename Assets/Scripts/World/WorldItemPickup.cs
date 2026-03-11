using UnityEngine;
using VeinsOfMalice.Player;

namespace VeinsOfMalice.World
{
    /// <summary>
    /// WorldItemPickup — Objeto físico en el mundo que se puede recoger.
    /// Al entrar en contacto con el Player, se añade al inventario.
    /// </summary>
    public class WorldItemPickup : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private ItemData itemData;
        [SerializeField] private int amount = 1;
        [SerializeField] private float collectionDelay = 0.5f;

        private bool canBeCollected = false;
        private float timer = 0f;

        private void Start()
        {
            canBeCollected = false;
            timer = 0f;
            
            // Opcional: Pequeño impulso al aparecer
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
                rb.AddForce(Random.insideUnitSphere * 2f, ForceMode.Impulse);
        }

        public void SetItem(ItemData data, int qty = 1)
        {
            itemData = data;
            amount = qty;
        }

        private void Update()
        {
            if (!canBeCollected)
            {
                timer += Time.deltaTime;
                if (timer >= collectionDelay) canBeCollected = true;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log($"[WorldItemPickup] Collision with: {other.name} (Tag: {other.tag})");
            
            if (canBeCollected && other.CompareTag("Player"))
            {
                Collect(other.gameObject);
            }
        }

        private void Collect(GameObject player)
        {
            var inv = player.GetComponentInParent<PlayerInventory>();
            if (inv != null)
            {
                if (itemData == null)
                {
                    Debug.LogError($"<color=red>[Pickup]</color> ItemData is NULL on {gameObject.name}! Can't pick up.");
                    return;
                }

                Debug.Log($"<color=orange>[Pickup]</color> Adding to Inv ID: {inv.GetInstanceID()} on {inv.gameObject.name}");
                if (inv.AddItem(itemData))
                {
                    Debug.Log($"<color=green>[Pickup]</color> Picked up {itemData.itemName}");
                    Destroy(gameObject);
                }
            }
            else
            {
                Debug.LogWarning("[WorldItemPickup] Hit Player but PlayerInventory component not found!");
            }
        }
    }
}
