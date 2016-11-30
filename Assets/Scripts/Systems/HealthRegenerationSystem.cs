// /** 
//  * HealthRegenerationSystem.cs
//  * Will Hart
//  * 20161105
// */

namespace GameGHJ.Systems
{
    #region Dependencies

    using System;
    using UnityEngine;

    #endregion

    /// <summary>
    ///     Regenerate health on living items with a health component
    /// </summary>
    public class HealthRegenerationSystem : ZenBehaviour, IOnUpdate
    {
        public override Type ObjectType => typeof(HealthRegenerationSystem);
        public override int ExecutionPriority => 0;

        public void OnUpdate()
        {
            var healths = ZenBehaviourManager.Instance.Get<HealthComp>(ComponentTypes.HealthComp);
            var delta = Time.deltaTime;

            foreach (var health in healths)
            {
                if (health.isDead) continue;
                health.currentHealth = Mathf.Min(health.maxHealth, health.currentHealth + delta*health.regenRate);
            }
        }
    }
}