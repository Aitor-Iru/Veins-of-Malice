using UnityEngine;
using VeinsOfMalice.World;

namespace VeinsOfMalice.Player
{
    /// <summary>
    /// EssenceCollector — Atrae automáticamente las esencias cercanas hacia el jugador.
    /// Requiere un SphereCollider configurado como Trigger.
    /// </summary>
    public class EssenceCollector : MonoBehaviour
    {
        [Header("Magnet Settings")]
        [SerializeField] private float collectionRadius = 5f;
        
        private SphereCollider trigger;

        private void Awake()
        {
            trigger = gameObject.AddComponent<SphereCollider>();
            trigger.isTrigger = true;
            trigger.radius = collectionRadius;
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.TryGetComponent<CursedEssence>(out var essence))
            {
                essence.AttractTo(transform);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0, 1, 1, 0.2f);
            Gizmos.DrawWireSphere(transform.position, collectionRadius);
        }
    }
}
