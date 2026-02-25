using UnityEngine;

namespace VeinsOfMalice.AI
{
    /// <summary>
    /// Base class for all AI states.
    /// </summary>
    public abstract class AIState
    {
        protected AIBaseController controller;

        public AIState(AIBaseController controller)
        {
            this.controller = controller;
        }

        public abstract void Enter();
        public abstract void Update();
        public virtual void FixedUpdate() { }
        public abstract void Exit();
    }
}
