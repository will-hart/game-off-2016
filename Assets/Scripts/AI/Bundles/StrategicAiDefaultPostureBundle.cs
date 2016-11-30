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

    public class StrategicAiDefaultPostureBundle : AiActionBundle<StrategicAiStateComp>
    {
        public StrategicAiDefaultPostureBundle() : base(new List<AbstractAiAction<StrategicAiStateComp>>
        {
            new StrategyPostureAcquisitiveAction(),
            new StrategyPostureAggressiveAction(),
            new StrategyPostureDefensiveAction(),
            new StrategyPostureDefensiveScrambleAction()
        })
        {
        }
    }
}