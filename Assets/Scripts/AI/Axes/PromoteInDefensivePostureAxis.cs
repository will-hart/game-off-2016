// /** 
//  * HasNearbyTargetsAxis.cs
//  * Will Hart
//  * 20161103
// */

namespace GameGHJ.AI.Axes
{
    #region Dependencies

    using System.Linq;
    using GameGHJ.AI.Core;
    using GameGHJ.AI.Strategy;
    using UnityEngine;

    #endregion

    public class PromoteInDefensivePostureAxis : IAxis<StrategicAiStateComp>
    {
        public float Score(AiContext<StrategicAiStateComp> context)
        {
            return context.State.Posture == StrategicPostures.Defensive ? 0 : 0.75f;
        }

        public string Name => "Promote in Defensive";
        public string Description => "Returns 1 if the AI is in defensive posture, 0.75 otherwise";
    }
}