using UnityEngine;
using UnityEditor;
using VeinsOfMalice.AI;

namespace VeinsOfMalice.Editor
{
    public class EnemyCreator : EditorWindow
    {
        [MenuItem("Tools/Veins of Malice/Create Grade 4 Enemy")]
        public static void CreateEnemy()
        {
            GameObject enemyObj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            enemyObj.name = "Enemy_Grade4_New";
            
            // Setup visual
            Renderer rend = enemyObj.GetComponent<Renderer>();
            rend.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            rend.material.color = Color.gray;

            // Physics
            Rigidbody rb = enemyObj.AddComponent<Rigidbody>();
            rb.mass = 50f;
            rb.linearDamping = 2f;
            rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

            // AI Components
            enemyObj.AddComponent<EnemyMotor>();
            enemyObj.AddComponent<EnemyCombat>();
            enemyObj.AddComponent<EnemyHealth>();
            enemyObj.AddComponent<Grade4Enemy>();

            // Ensure Z=0 and Y=1.1 (standard capsule bottom at 0 approx)
            enemyObj.transform.position = new Vector3(0, 1.1f, 0);

            Selection.activeGameObject = enemyObj;
            SceneView.FrameLastActiveSceneView();
            
            Debug.Log("<color=green>[EnemyCreator]</color> Created a new Grade 4 Enemy at Z=0.");
            Debug.Log("<color=yellow>Checklist if it doesn't work:</color>\n" +
                      "1. Ensure Player has the <b>'Player'</b> Tag.\n" +
                      "2. Ensure the Enemy is on the same <b>Layer</b> that is assigned to the 'Enemy Layer' field in the <b>PlayerCombat</b> component.\n" +
                      "3. Check the Console for <b>[AI]</b> and <b>[EnemyHealth]</b> logs.");
        }
    }
}
