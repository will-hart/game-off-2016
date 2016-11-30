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

    public class CreepAttackInHeroSupportAction : AbstractAttackingAction
    {
        private Vector3 _currentFiringPosition;

        public CreepAttackInHeroSupportAction() : base(new List<IAxis<TacticalAiStateComp>>
        {
            new HasNearbyTargetsAxis(),
            new IsNearHeroAxis(),
        })
        {
        }

        public override int Priority => 2;

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
            _currentFiringPosition = GetAttackingPosition(context);

            base.Update(context);
        }

        /// <summary>
        /// Gets the current firing position of the creep
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override Vector3 GetPathFindingTarget(AiContext<TacticalAiStateComp> context)
        {
            return _currentFiringPosition;
        }
    }
}