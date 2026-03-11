using UnityEngine;
using UnityEditor;
using VeinsOfMalice.AI;

namespace VeinsOfMalice.Editor
{
    public class PhysicsCleanupTool
    {
        [MenuItem("Tools/Veins of Malice/Sync Enemy Physics (MassFix)")]
        public static void SyncPhysics()
        {
            EnemyMotor[] motors = Object.FindObjectsByType<EnemyMotor>(FindObjectsSortMode.None);
            int count = 0;

            foreach (var motor in motors)
            {
                Rigidbody rb = motor.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.mass = 50f;
                    rb.linearDamping = 2f;
                    rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
                    EditorUtility.SetDirty(rb);
                    count++;
                }
            }

            Debug.Log($"<color=green>[PhysicsFix]</color> Updated {count} enemies to Mass=5.");
        }
    }
}
