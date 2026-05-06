using UnityEngine;

namespace VeinsOfMalice.AI.States
{
    public class ChaseState : AIState
    {
        private float looseRange = 10f;

        public ChaseState(AIBaseController controller) : base(controller) { }

        public override void Enter() { }

        public override void Update()
        {
            if (controller.Target == null)
            {
                controller.TransitionToState(new IdleState(controller));
                return;
            }

            float distance = Vector3.Distance(controller.transform.position, controller.Target.position);

            if (distance > looseRange)
            {
                controller.TransitionToState(new IdleState(controller));
                return;
            }

            if (controller.Combat.IsTargetInRange(controller.Target))
            {
                controller.TransitionToState(new AttackState(controller));
                return;
            }

            // Move towards target
            Vector3 direction = (controller.Target.position - controller.transform.position).normalized;
            controller.Motor.SetMoveDirection(direction);
        }

        public override void Exit()
        {
            controller.Motor.Stop();
        }
    }
}
