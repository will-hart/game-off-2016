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
    using UnityEngine;

    #endregion

    public class HasABuildOrderAxis : IAxis<StrategicAiStateComp>
    {
        public float Score(AiContext<StrategicAiStateComp> context)
        {
            return Mathf.Clamp01(context.State.BuildOrder.Count);
        }

        public string Name => "Has A Build Order";
        public string Description => "Returns 1 if there is a build order, otherwise returns 0";
    }
}