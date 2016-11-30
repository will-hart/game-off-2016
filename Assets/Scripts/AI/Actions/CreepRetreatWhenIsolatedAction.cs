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
    using System.Linq;
    using Axes;
    using GameGHJ.AI.Core;
    using UniRx;
    using UnityEngine;

    #endregion

    public class CreepRetreatWhenIsolatedAction : AbstractNavigationAction
    {
        private const float PathfindingFrequency = 0.7f;
        private float _nextPathfindingTime;

        private Vector3? _sideHqPos;

        public CreepRetreatWhenIsolatedAction() : base(new List<IAxis<TacticalAiStateComp>>
        {
            new HasNearbyTargetsAxis(),
            new InvertAxis<TacticalAiStateComp>(new IsNearHeroAxis(20)),
            new IsOutnumberedAxis(0.35f)
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

            var teamId = context.GetComponent<UnitPropertiesComp>().teamID;
            _sideHqPos = ZenBehaviourManager.Instance.Get<SidePropertiesComp>(ComponentTypes.SidePropertiesComp)
                .FirstOrDefault(o => o.SideId == teamId)?
                .Owner.GetComponent<PositionComp>().position;

            if (_sideHqPos == null) IsComplete = true;

            RunPathFinding(context);
        }
        
        /// <summary>
        /// Only run pathfinding if we can't raycast to the hero and it is time
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override bool ShouldRunPathfinding(AiContext<TacticalAiStateComp> context)
        {
            if (Time.time < _nextPathfindingTime) return false;
            _nextPathfindingTime = Time.time + PathfindingFrequency;
            return true;
        }

        protected override Vector3 GetPathFindingTarget(AiContext<TacticalAiStateComp> context)
        {
            return _sideHqPos ?? context.GetComponent<PositionComp>().position;
        }

        /// <summary>
        /// Finds a new target for the creep to fire at
        /// </summary>
        /// <param name="combat"></param>
        private static void UpdateTargetedComponent(CombatComp combat)
        {
            var pos = combat.Owner.GetComponent<PositionComp>();

            // check if our current enemy is valid
            if (combat.TargetedEnemy != null)
            {
                var dist = (pos.position - combat.TargetedEnemy.position).sqrMagnitude;
                if (!combat.TargetedEnemy.Owner.GetComponent<HealthComp>().isDead && dist < 1) return;
                combat.TargetedEnemy = null;
            }

            // find a new enemy
            if (combat.EnemiesInRange.Count == 0) return;
            combat.TargetedEnemy = combat.EnemiesInRange[0];
        }
    }
}