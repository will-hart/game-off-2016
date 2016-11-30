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

    public class HeroAttackNearbyTargetAction : AbstractAttackingAction
    {
        public HeroAttackNearbyTargetAction() : base(new List<IAxis<TacticalAiStateComp>>
        {
            new HasNearbyTargetsAxis()
        })
        {
            PathfindingFrequency = 0.3f;
        }

        public override int Priority => 2;

        public override void OnEnter(AiContext<TacticalAiStateComp> context)
        {
            base.OnEnter(context);
            
            context.State.NavigationTarget = context.GetComponent<PositionComp>().position;
            context.State.NavigationTargetUpdated = true;
        }

        protected override Vector3 GetPathFindingTarget(AiContext<TacticalAiStateComp> context)
        {
            return context.State.NavigationTarget;
        }

        /// <summary>
        /// Run pathfinding perioidically to prevent thrashing the pathfinder
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override bool ShouldRunPathfinding(AiContext<TacticalAiStateComp> context) => false;

    }
}