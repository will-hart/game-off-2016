// /** 
//  * CreepAiBundle.cs
//  * Will Hart
//  * 20161103
// */

namespace GameGHJ.AI.Bundles
{
    #region Dependencies

    using System.Collections.Generic;
    using GameGHJ.AI.Actions;

    #endregion

    public class HeroAiBundle : AiActionBundle<TacticalAiStateComp>
    {
        public HeroAiBundle() : base(new List<AbstractAiAction<TacticalAiStateComp>>
        {
            new HeroAttackDesignatedTargetAction(),
            new HeroAttackNearbyTargetAction(),
            new HeroNavigateToLocationAction(),
            new HeroNavigateToTargetLocationAction()
        })
        {
        }
    }
}