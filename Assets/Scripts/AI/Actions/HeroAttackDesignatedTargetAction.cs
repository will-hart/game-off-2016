// /** 
//  * HeroAttackDesignatedTargetAction.cs
//  * Will Hart
//  * 20161112
// */

namespace GameGHJ.AI.Actions
{
    #region Dependencies

    using System.Collections.Generic;
    using System.Net.Mime;
    using GameGHJ.AI.Axes;
    using GameGHJ.AI.Core;
    using UnityEngine;

    #endregion

    public class HeroAttackDesignatedTargetAction : AbstractAttackingAction
    {
        public HeroAttackDesignatedTargetAction() : base(new List<IAxis<TacticalAiStateComp>>
        {
            new HasTargetAxis(),
            new InvertAxis<TacticalAiStateComp>(new IsFarFromTargetAxis())
        })
        {
        }

        public override void OnEnter(AiContext<TacticalAiStateComp> context)
        {
            context.GetComponent<CombatComp>().TargetedEnemy = context.State.AttackTarget.Owner.GetComponent<PositionComp>();
        }

        public override void Update(AiContext<TacticalAiStateComp> context)
        {
            if (context.State.AttackTarget == null || context.State.AttackTargetUpdated ||
                context.State.AttackTarget.isDead)
            {
                IsComplete = true;
                return;
            }
            
            var newTarget = GetAttackingPosition(context);
            if ((newTarget - context.State.NavigationTarget).sqrMagnitude < 1) return;

            context.State.NavigationTarget = newTarget;
            context.State.NavigationTargetUpdated = true;
        }

        protected override Vector3 GetPathFindingTarget(AiContext<TacticalAiStateComp> context)
            => context.State.NavigationTarget;

        protected override bool ShouldRunPathfinding(AiContext<TacticalAiStateComp> context)
            => context.State.NavigationTargetUpdated;

        public override int Priority => 3;
    }
}