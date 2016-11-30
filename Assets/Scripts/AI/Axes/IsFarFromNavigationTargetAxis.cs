// /** 
//  * IsNotNearNavigationTargetAxis.cs
//  * Will Hart
//  * 20161103
// */

namespace GameGHJ.AI.Axes
{
    #region Dependencies

    using UnityEngine;

    using Core;

    #endregion

    public class IsFarFromNavigationTargetAxis : IAxis<TacticalAiStateComp>
    {
        private readonly float _nearDistance;

        public IsFarFromNavigationTargetAxis(float nearDistance = 15)
        {
            _nearDistance = nearDistance;
        }

        public float Score(AiContext<TacticalAiStateComp> context)
        {
            var target = context.State.NavigationTarget;
            var pos = context.GetComponent<PositionComp>().position;
            var sqrMag = (target - pos).sqrMagnitude;

            return Functions.Octic(sqrMag/_nearDistance);
        }

        public string Name => "Is Not Near Hero";

        public string Description
            => "Returns 1 when the hero is far from the navigation target, and returns 0 as the hero moves within nearDistance of the target";
    }
}