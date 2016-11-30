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

    public class HasAssignedHeroAxis : IAxis<TacticalAiStateComp>
    {
        public float Score(AiContext<TacticalAiStateComp> context)
        {
            return Functions.OneIfFalse(context.GetComponent<CreepComp>().AssignedHero == null);
        }

        public string Name => "Has Assigned Hero";
        public string Description => "Returns 1 if there is an assigned hero, otherwise returns 0";
    }
}