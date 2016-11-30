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

    public class HasAnUpgradeOrderAxis : IAxis<StrategicAiStateComp>
    {
        public float Score(AiContext<StrategicAiStateComp> context)
        {
            return Mathf.Clamp01(context.State.UpgradeOrder.Count);
        }

        public string Name => "Has An Upgrade Order";
        public string Description => "Returns 1 if there is an upgrade order, otherwise returns 0";
    }
}