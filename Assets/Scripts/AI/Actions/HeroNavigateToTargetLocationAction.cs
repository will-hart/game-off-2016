// /** 
//  * HeroNavigateToTargetLocationAction.cs
//  * Will Hart
//  * 20161112
// */

namespace GameGHJ.AI.Actions
{
    #region Dependencies

    using System.Collections.Generic;
    using GameGHJ.AI.Axes;
    using GameGHJ.AI.Core;
    using UnityEngine;

    #endregion

    /// <summary>
    ///     Navigates the hero to a given location using the AI pathfinding system.
    ///     The navigation target is set by strategic AI or the player, not handled within tactical AI
    /// </summary>
    public class HeroNavigateToTargetLocationAction : AbstractNavigationAction
    {
        private const float PathfindingFrequency = 1.5f;
        private float _nextPathfindingTime;


        public HeroNavigateToTargetLocationAction() : base(new List<IAxis<TacticalAiStateComp>>
        {
            new HasTargetAxis(),
            new IsFarFromTargetAxis()
        })
        {
        }

        public override int Priority => 1;

        public override void Update(AiContext<TacticalAiStateComp> context)
        {
            if ((context.State.AttackTarget == null) || context.State.AttackTarget.isDead)
            {
                IsComplete = true;
                context.State.AttackTarget = null;
                context.State.AttackTargetUpdated = true;
                return;
            }

            base.Update(context);
        }

        protected override Vector3 GetPathFindingTarget(AiContext<TacticalAiStateComp> context)
        {
            return context.State.AttackTarget?.Owner.GetComponent<PositionComp>().position 
                ?? context.GetComponent<PositionComp>().position;
        }

        public override void OnEnter(AiContext<TacticalAiStateComp> context)
        {
            base.OnEnter(context);
            RunPathFinding(context);
            _nextPathfindingTime = Time.time + PathfindingFrequency;
        }

        protected override bool ShouldRunPathfinding(AiContext<TacticalAiStateComp> context)
        {
            if (Time.time < _nextPathfindingTime) return false;

            _nextPathfindingTime = Time.time + PathfindingFrequency;
            return true;
        }
    }
}