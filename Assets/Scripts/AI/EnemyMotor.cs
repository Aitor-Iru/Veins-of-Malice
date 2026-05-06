using UnityEngine;

namespace VeinsOfMalice.AI
{
    /// <summary>
    /// Handles physical movement for AI entities, respecting 2.5D constraints.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class EnemyMotor : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private float acceleration = 40f; // Higher acceleration means stronger "grip" to current movement state
        
        private Rigidbody rb;
        private Vector3 moveDirection;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            
            // Apply 2.5D constraints
            rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.mass = 50f; // High mass makes them feel heavy and hard to push
            rb.linearDamping = 2f; // Linear drag to stop them quickly if pushed
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        public void SetMoveDirection(Vector3 direction)
        {
            moveDirection = new Vector3(direction.x, 0, 0).normalized;
            
            // Look direction
            if (moveDirection.x != 0)
            {
                float yRot = moveDirection.x > 0 ? 90f : -90f;
                transform.rotation = Quaternion.Euler(0, yRot, 0);
            }
        }

        public void Stop()
        {
            moveDirection = Vector3.zero;
        }

        private void FixedUpdate()
        {
            Vector3 targetVelocity = moveDirection * moveSpeed;
            Vector3 currentVelocity = rb.linearVelocity;
            
            // Only affect X axis for horizontal movement
            float newX = Mathf.MoveTowards(currentVelocity.x, targetVelocity.x, acceleration * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector3(newX, currentVelocity.y, 0);
        }
        
        public float GetMoveSpeed() => moveSpeed;

        public void ApplyKnockback(Vector3 force)
        {
            if (rb != null)
            {
                rb.AddForce(force, ForceMode.Impulse);
            }
        }
    }
}
