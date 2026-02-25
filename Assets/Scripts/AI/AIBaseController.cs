using UnityEngine;

namespace VeinsOfMalice.AI
{
    /// <summary>
    /// Base class for AI controllers. Manages the Finite State Machine.
    /// </summary>
    public abstract class AIBaseController : MonoBehaviour
    {
        [Header("References")]
        public EnemyMotor Motor;
        public EnemyCombat Combat;
        public EnemyHealth Health;
        public Transform Target;

        protected AIState currentState;

        protected virtual void Awake()
        {
            Motor = GetComponent<EnemyMotor>();
            Combat = GetComponent<EnemyCombat>();
            Health = GetComponent<EnemyHealth>();
            
            // Default target is player if not set
            if (Target == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null) Target = player.transform;
            }
        }

        protected virtual void Update()
        {
            currentState?.Update();
        }

        protected virtual void FixedUpdate()
        {
            currentState?.FixedUpdate();
        }

        public void TransitionToState(AIState newState)
        {
            string oldStateName = currentState != null ? currentState.GetType().Name : "None";
            currentState?.Exit();
            currentState = newState;
            currentState?.Enter();
            
            Debug.Log($"<color=cyan>[AI]</color> <b>{gameObject.name}</b> transitioned: <color=white>{oldStateName}</color> -> <color=yellow>{newState.GetType().Name}</color>");
        }
    }
}
