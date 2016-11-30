// /** 
//  * TowerAiBundle.cs
//  * Will Hart
//  * 20161103
// */

namespace GameGHJ.AI.Bundles
{
    #region Dependencies

    using System.Collections.Generic;

    using Actions;

    #endregion

    public class TowerAiBundle : AiActionBundle<TacticalAiStateComp>
    {
        public TowerAiBundle() : base(new List<AbstractAiAction<TacticalAiStateComp>>
        {
            new TowerIdleAction(),
            new TowerAttackNearbyTargetAction()
        })
        {
        }
    }
}