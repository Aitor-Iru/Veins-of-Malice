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
            // After attacking, go back to chase or idle
            // In a more complex system, we'd wait for animation or a timer
            if (controller.Combat.CanAttack(controller.Target))
            {
                controller.Combat.PerformAttack(controller.Target);
            }
            else
            {
                controller.TransitionToState(new ChaseState(controller));
            }
        }

        public override void Exit() { }
    }
}
