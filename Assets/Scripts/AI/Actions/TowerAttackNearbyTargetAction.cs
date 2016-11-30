// /** 
//  * TowerAttackNearbyTargetAction.cs
//  * Will Hart
//  * 20161116
// */

namespace GameGHJ.AI.Actions
{
    #region Dependencies

    using System.Collections.Generic;
    using System.Linq;
    using GameGHJ.AI.Axes;
    using GameGHJ.AI.Core;

    #endregion

    public class TowerAttackNearbyTargetAction : AbstractAttackingAction
    {
        public TowerAttackNearbyTargetAction() : base(new List<IAxis<TacticalAiStateComp>>
        {
            new HasNearbyTargetsAxis()
        })
        {
        }

        public override int Priority => 1;

        protected override bool ShouldRunPathfinding(AiContext<TacticalAiStateComp> context) => false;

        protected override float UpdateSpeed(bool hw, MovementComp m, AiContext<TacticalAiStateComp> c) => 0;

        public override void Update(AiContext<TacticalAiStateComp> context)
        {
            if (!CheckTargetIsStillValid(context))
            {
                UpdateTargetedComponent(context.GetComponent<CombatComp>());
            }
        }
    }
}