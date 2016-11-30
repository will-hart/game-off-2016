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

    public class TowerIdleAction : AbstractAiAction<TacticalAiStateComp>
    {
        public TowerIdleAction() : base(new List<IAxis<TacticalAiStateComp>>
        {
            new InvertAxis<TacticalAiStateComp>(new HasNearbyTargetsAxis())
        })
        {
        }

        public override int Priority => 1;
    }
}