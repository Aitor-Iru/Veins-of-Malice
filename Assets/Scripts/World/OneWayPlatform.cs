using UnityEngine;

namespace VeinsOfMalice.World
{
    /// <summary>
    /// Allows the player to jump through the platform from below or sides, 
    /// but stand on it when falling from above.
    /// </summary>
    public class OneWayPlatform : MonoBehaviour
    {
        private Collider platformCollider;
        private Collider playerCollider;
        private Transform playerTransform;

        [Header("Settings")]
        [Tooltip("Extra margin to ensure the player is considered 'above' the platform")]
        [SerializeField] private float verticalMargin = 0.1f;

        private void Start()
        {
            platformCollider = GetComponent<Collider>();
            
            // Try to find the player and its collider
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
                playerCollider = player.GetComponent<Collider>();
            }
        }

        private void Update()
        {
            if (playerTransform == null || playerCollider == null || platformCollider == null) return;

            // 1. Get the world Y position of the platform's top surface using bounds
            float platformTop = platformCollider.bounds.max.y;

            // 2. Get the world Y position of the player's bottom (feet) using bounds
            float playerBottom = playerCollider.bounds.min.y;

            // 3. Logic: Should we ignore collision? 
            // We ignore IF we are below the top OR if we are jumping up.
            // Using a generous 0.3f margin to prevent falling through due to physics jitter.
            bool isAbove = playerBottom >= (platformTop - 0.3f);
            
            Rigidbody rb = playerCollider.attachedRigidbody;
            bool isMovingUp = rb != null && rb.linearVelocity.y > 0.1f;

            // We only collide if we are on top AND not flying upwards
            bool shouldIgnore = !isAbove || isMovingUp;

            Physics.IgnoreCollision(playerCollider, platformCollider, shouldIgnore);
            
            // Optional: Debugging
            // if (isAbove) Debug.DrawLine(playerTransform.position, new Vector3(playerTransform.position.x, platformTop, playerTransform.position.z), Color.green);
        }

        // Visual debug in editor
        private void OnDrawGizmosSelected()
        {
            float platformTop = transform.position.y + (transform.localScale.y * 0.5f);
            Gizmos.color = Color.yellow;
            Vector3 lineStart = transform.position + Vector3.up * (transform.localScale.y * 0.5f);
            Gizmos.DrawWireCube(lineStart, new Vector3(transform.localScale.x, 0.05f, transform.localScale.z));
        }
    }
}
