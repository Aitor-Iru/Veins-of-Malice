using UnityEngine;
using System.Collections;

namespace VeinsOfMalice.World
{
    /// <summary>
    /// CursedEssence — Item recolectable que sueltan los enemigos.
    /// Tiene un comportamiento de "atracción" hacia el jugador.
    /// </summary>
    public class CursedEssence : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int value = 1;
        [SerializeField] private float attractSpeed = 10f;
        [SerializeField] private float idleFloatAmplitude = 0.2f;
        [SerializeField] private float idleFloatSpeed = 2f;
        [SerializeField] private float collectionDelay = 2f;
        [SerializeField] private ItemData itemData; // Optional: To show in grid slots

        private Transform target;
        private bool isAttracted = false;
        private bool canBeCollected = false;
        private Vector3 startPos;
        private float floatTimer;
        private float spawnTimer;

        private void Start()
        {
            startPos = transform.position;
            spawnTimer = 0f;
            canBeCollected = false;

            // Pequeño salto inicial aleatorio
            GetComponent<Rigidbody>()?.AddForce(Random.insideUnitSphere * 2f, ForceMode.Impulse);
        }

        private void Update()
        {
            if (!canBeCollected)
            {
                spawnTimer += Time.deltaTime;
                if (spawnTimer >= collectionDelay)
                {
                    canBeCollected = true;
                }
            }

            if (isAttracted && target != null && canBeCollected)
            {
                MoveTowardsTarget();
            }
            else
            {
                FloatInPlace();
            }
        }

        private void MoveTowardsTarget()
        {
            transform.position = Vector3.Lerp(transform.position, target.position, attractSpeed * Time.deltaTime);

            // Si está muy cerca, recolectar
            if (Vector3.Distance(transform.position, target.position) < 0.5f)
            {
                Collect();
            }
        }

        private void FloatInPlace()
        {
            floatTimer += Time.deltaTime;
            float newY = startPos.y + Mathf.Sin(floatTimer * idleFloatSpeed) * idleFloatAmplitude;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }

        /// <summary>
        /// Activa la atracción hacia un objetivo (el jugador).
        /// </summary>
        public void AttractTo(Transform playerTransform)
        {
            target = playerTransform;
            isAttracted = true;
        }

        private void Collect()
        {
            if (!canBeCollected) return;

            if (target != null)
            {
                var inv = target.GetComponentInParent<VeinsOfMalice.Player.PlayerInventory>();
                if (inv != null)
                {
                    inv.AddEssence(value);
                    if (itemData != null)
                    {
                        inv.AddItem(itemData);
                    }
                    else
                    {
                        Debug.LogWarning($"<color=red>[Essence]</color> ItemData is MISSING on {gameObject.name}. It won't show in the grid!");
                    }
                }
                else
                {
                    Debug.LogWarning("[CursedEssence] Player found but PlayerInventory component missing or not in parents!");
                }
            }
            
            // Efecto visual o sonido aquí si se desea
            Destroy(gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log($"[CursedEssence] Collision with: {other.name} (Tag: {other.tag})");
            
            if (canBeCollected && other.CompareTag("Player"))
            {
                if (target == null) target = other.transform;
                Collect();
            }
        }
    }
}
