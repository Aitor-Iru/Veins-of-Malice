using UnityEngine;

namespace VeinsOfMalice.AI.States
{
    public class IdleState : AIState
    {
        private float detectionRange = 7f;

        public IdleState(AIBaseController controller) : base(controller) { }

        public override void Enter()
        {
            controller.Motor.Stop();
        }

        public override void Update()
        {
            if (controller.Target != null)
            {
                float distance = Vector3.Distance(controller.transform.position, controller.Target.position);
                if (distance <= detectionRange)
                {
                    controller.TransitionToState(new ChaseState(controller));
                }
            }
        }

        public override void Exit() { }
    }
}
