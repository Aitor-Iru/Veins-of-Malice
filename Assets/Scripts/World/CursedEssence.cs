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

            if (target != null && target.TryGetComponent<VeinsOfMalice.Player.PlayerInventory>(out var inv))
            {
                inv.AddEssence(value);
            }
            
            // Efecto visual o sonido aquí si se desea
            Destroy(gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (canBeCollected && other.CompareTag("Player"))
            {
                if (target == null) target = other.transform;
                Collect();
            }
        }
    }
}
