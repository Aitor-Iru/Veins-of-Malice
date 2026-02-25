using UnityEngine;
using VeinsOfMalice.AI.States;

namespace VeinsOfMalice.AI
{
    /// <summary>
    /// Concrete implementation for Grade 4 Enemies (Basic Mobs).
    /// </summary>
    public class Grade4Enemy : AIBaseController
    {
        protected override void Awake()
        {
            base.Awake();
            
            if (Health != null) Health.OnDeath += HandleDeath;

            // Start in Idle
            TransitionToState(new IdleState(this));
        }

        private void OnDestroy()
        {
            if (Health != null) Health.OnDeath -= HandleDeath;
        }

        private void HandleDeath()
        {
            TransitionToState(new DeathState(this));
        }
        
        // Grade 4 specific logic could go here (e.g., unique death events, XP drops)
    }
}
