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

    public class AlwaysOneAxis : IAxis<TacticalAiStateComp>
    {
        public float Score(AiContext<TacticalAiStateComp> context) => 1;

        public string Name => "Always one axis";
        public string Description => "Returns 1";
    }
}