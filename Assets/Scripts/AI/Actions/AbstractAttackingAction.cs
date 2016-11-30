// /** 
//  * CreepNavigateToHero.cs
//  * Will Hart
//  * 20161103
// */

namespace GameGHJ.AI.Actions
{
    #region Dependencies

    using System.Collections.Generic;

    using Axes;
    using GameGHJ.AI.Core;
    using UnityEngine;

    #endregion

    /// <summary>
    /// Sets navigation targets at set intervals
    /// </summary>
    public abstract class AbstractAttackingAction : AbstractNavigationAction
    {
        protected float PathfindingFrequency = 0.3f;
        protected float _nextPathfindingTime;

        protected AbstractAttackingAction(IEnumerable<IAxis<TacticalAiStateComp>> axes) : base(axes)
        {
        }

        /// <summary>
        /// Gets a navigation position within range of the assigned target
        /// </summary>
        /// <param name="context"></param>
        protected virtual Vector3 GetAttackingPosition(AiContext<TacticalAiStateComp> context)
        {
            var pos = context.GetComponent<PositionComp>();
            var combat = context.GetComponent<CombatComp>();

            var dirToTarget = (combat.TargetedEnemy.position - pos.position).normalized;
            var movementRange = combat.selectedWeapon.AttackType == AttackType.Melee
                ? 1
                : combat.selectedWeapon.attackRange;
            return movementRange * dirToTarget;
        }

        /// <summary>
        /// Invalidate expired or out of range targets
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual bool CheckTargetIsStillValid(AiContext<TacticalAiStateComp> context)
        {
            var pos = context.GetComponent<PositionComp>();
            var combat = context.GetComponent<CombatComp>();

            // check if our current enemy is valid
            if (combat.TargetedEnemy == null) return false;

            var dist = (pos.position - combat.TargetedEnemy.position).sqrMagnitude;
            if (!combat.TargetedEnemy.Owner.GetComponent<HealthComp>().isDead && 
                dist < combat.selectedWeapon.attackRange) return true;

            combat.TargetedEnemy = null;
            return false;
        }

        /// <summary>
        /// Finds a new target for the creep to fire at
        /// </summary>
        /// <param name="combat"></param>
        protected virtual void UpdateTargetedComponent(CombatComp combat)
        {
            // find a new enemy
            if (combat.EnemiesInRange.Count == 0) return;
            combat.TargetedEnemy = combat.EnemiesInRange[0];
        }

        /// <summary>
        /// Run pathfinding perioidically to prevent thrashing the pathfinder
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override bool ShouldRunPathfinding(AiContext<TacticalAiStateComp> context)
        {
            if (Time.time < _nextPathfindingTime) return false;
            _nextPathfindingTime = Time.time + PathfindingFrequency;
            return true;
        }
    }
}