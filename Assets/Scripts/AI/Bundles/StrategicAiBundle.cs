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

    public class StrategicAiBundle : AiActionBundle<StrategicAiStateComp>
    {
        public StrategicAiBundle() : base(new List<AbstractAiAction<StrategicAiStateComp>>
        {
            new StrategyBuildNewBuildingAction(),
            new StrategyBuildNewHeroAction(),
            new StrategyApplyUpgradesAction()
        })
        {
        }
    }
}