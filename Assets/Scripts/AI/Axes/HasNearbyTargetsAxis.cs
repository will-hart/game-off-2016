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

    public class HasNearbyTargetsAxis : IAxis<TacticalAiStateComp>
    {
        public float Score(AiContext<TacticalAiStateComp> context)
        {
            return Mathf.Clamp01(context.GetComponent<CombatComp>().EnemiesInRange.Count);
        }

        public string Name => "Has Nearby Targets";
        public string Description => "Returns 0 if there are no nearby targets, otherwise returns 1";
    }
}