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

    public class CreepNavigateToHeroAction : AbstractNavigationAction
    {
        private const float PathfindingFrequency = 1.5f;
        private float _nextPathfindingTime;

        public CreepNavigateToHeroAction() : base(new List<IAxis<TacticalAiStateComp>>
        {
            new HasAssignedHeroAxis(),
            new AssignedHeroIsAliveAxis(),
            new InvertAxis<TacticalAiStateComp>(new IsNearHeroAxis())
        })
        {
        }

        public override int Priority => 1;

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