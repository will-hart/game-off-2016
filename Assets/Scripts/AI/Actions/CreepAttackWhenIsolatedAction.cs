// /** 
//  * CreepNavigateToHero.cs
//  * Will Hart
//  * 20161103
// */

namespace GameGHJ.AI.Actions
{
    #region Dependencies

    using System;
    using System.Collections.Generic;

    using Axes;
    using GameGHJ.AI.Core;
    using UniRx;
    using UnityEngine;

    #endregion

    public class CreepAttackWhenIsolatedAction : AbstractAttackingAction
    {
        private Vector3 _currentFiringPosition;

        public CreepAttackWhenIsolatedAction() : base(new List<IAxis<TacticalAiStateComp>>
        {
            new HasNearbyTargetsAxis(), // targets nearby
            new InvertAxis<TacticalAiStateComp>(new AssignedHeroIsAliveAxis()), // hero is dead
            new InvertAxis<TacticalAiStateComp>(new IsOutnumberedAxis()) // not outnumbered
        })
        {
            PathfindingFrequency = 0.7f;
        }

        public override int Priority => 1;

        public override void OnEnter(AiContext<TacticalAiStateComp> context)
        {
            base.OnEnter(context);
            context.State.Waypoints?.Clear();
            context.State.CurrentWaypoint = 0;
            _nextPathfindingTime = Time.time;

            _currentFiringPosition = context.GetComponent<PositionComp>().position;
        }

        public override void Update(AiContext<TacticalAiStateComp> context)
        {
            // find a relevant target
            var combat = context.GetComponent<CombatComp>();
            if (!CheckTargetIsStillValid(context))
                UpdateTargetedComponent(combat);
            
            if (combat.TargetedEnemy == null || combat.selectedWeapon == null)
            {
                IsComplete = true;
                base.Update(context);
                return;
            }

            // set a navigation waypoint within range of the target
            var pos = context.GetComponent<PositionComp>();
            var dirToTarget = (combat.TargetedEnemy.position - pos.position).normalized;
            var movementRange = combat.selectedWeapon.AttackType == AttackType.Melee
                ? 1
                : combat.selectedWeapon.attackRange;
            _currentFiringPosition = movementRange*dirToTarget;

            base.Update(context);
        }

        protected override Vector3 GetPathFindingTarget(AiContext<TacticalAiStateComp> context)
        {
            return _currentFiringPosition;
        }
    }
}