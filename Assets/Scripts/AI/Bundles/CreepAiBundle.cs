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

    public class CreepAiBundle : AiActionBundle<TacticalAiStateComp>
    {
        public CreepAiBundle() : base(new List<AbstractAiAction<TacticalAiStateComp>>
        {
            new CreepNavigateToHeroAction(),
            new CreepFollowHeroAction(),
            new CreepAttackInHeroSupportAction(),
            new CreepAttackWhenIsolatedAction(),
            new CreepRetreatWhenIsolatedAction()
        })
        {
        }
    }
}