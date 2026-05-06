using UnityEngine;

namespace VeinsOfMalice.AI.States
{
    public class AttackState : AIState
    {
        public AttackState(AIBaseController controller) : base(controller) { }

        public override void Enter()
        {
            controller.Motor.Stop();
            controller.Combat.PerformAttack(controller.Target);
        }

        public override void Update()
        {
            if (controller.Target == null)
            {
                controller.TransitionToState(new IdleState(controller));
                return;
            }

            if (!controller.Combat.IsTargetInRange(controller.Target))
            {
                controller.TransitionToState(new ChaseState(controller));
                return;
            }

            // Still in range, stop motor and attack if cooldown allows
            controller.Motor.Stop();
            
            if (controller.Combat.CanAttack(controller.Target))
            {
                controller.Combat.PerformAttack(controller.Target);
            }
        }

        public override void Exit() { }
    }
}
