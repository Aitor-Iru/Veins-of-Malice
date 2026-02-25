using UnityEngine;
using System.Collections;

namespace VeinsOfMalice.AI.States
{
    /// <summary>
    /// Handles the enemy death sequence: shaking, darkening, falling, and disappearing.
    /// </summary>
    public class DeathState : AIState
    {
        private float shakeIntensity = 0.1f;
        private float deathDuration = 2.0f;
        private Vector3 originalLocalPos;

        public DeathState(AIBaseController controller) : base(controller) { }

        public override void Enter()
        {
            controller.Motor.Stop();
            // Disable motor and combat to avoid further actions
            controller.Motor.enabled = false;
            controller.Combat.enabled = false;
            
            // Disable main collider and make Rigidbody kinematic to allow manual movement
            Collider col = controller.GetComponent<Collider>();
            if (col) col.enabled = false;

            Rigidbody rb = controller.GetComponent<Rigidbody>();
            if (rb) rb.isKinematic = true;

            originalLocalPos = controller.transform.position;
            controller.StartCoroutine(DeathSequenceRoutine());
        }

        private IEnumerator DeathSequenceRoutine()
        {
            Renderer rend = controller.GetComponentInChildren<Renderer>();
            Color originalColor = rend ? rend.material.color : Color.white;
            float elapsed = 0f;

            // 1. Shake and Fall over
            Quaternion targetRotation = Quaternion.Euler(0, 0, 90); // Lay down on Z for a 2.5D game (side view)
            Quaternion startRotation = controller.transform.rotation;

            while (elapsed < deathDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / deathDuration;

                // Shake effect
                if (t < 0.5f) // Shake mostly at the start
                {
                    Vector3 shakeOffset = Random.insideUnitSphere * shakeIntensity;
                    controller.transform.position = originalLocalPos + shakeOffset;
                }
                else
                {
                    controller.transform.position = originalLocalPos;
                }

                // Color darkening
                if (rend)
                {
                    rend.material.color = Color.Lerp(originalColor, Color.black, t);
                }

                // Falling over and sinking to lie on ground
                controller.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);
                
                // If the capsule height is 2 and radius is 0.5, when it's flat its "bottom" 
                // is 0.5 units from its center. We lower the center towards 0.5.
                float targetY = 0.5f; 
                float newY = Mathf.Lerp(originalLocalPos.y, targetY, t);
                controller.transform.position = new Vector3(controller.transform.position.x, newY, controller.transform.position.z);

                yield return null;
            }

            // Final darkening
            if (rend) rend.material.color = Color.black;
            
            yield return new WaitForSeconds(1.0f);
            
            // Disappear
            GameObject.Destroy(controller.gameObject);
        }

        public override void Update() { }
        public override void Exit() { }
    }
}
