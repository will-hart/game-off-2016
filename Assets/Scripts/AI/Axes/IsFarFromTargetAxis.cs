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

    public class IsFarFromTargetAxis : IAxis<TacticalAiStateComp>
    {
        private readonly float _nearDistance;

        public IsFarFromTargetAxis(float nearDistance = 10)
        {
            _nearDistance = nearDistance;
        }

        public float Score(AiContext<TacticalAiStateComp> context)
        {
            var target = context.State.AttackTarget;

            if (target?.Owner == null) return 1;

            var targetPos = target.Owner.GetComponent<PositionComp>();
            var pos = context.GetComponent<PositionComp>().position;
            var sqrMag = (targetPos.position - pos).sqrMagnitude;

            return Functions.Octic(sqrMag/_nearDistance);
        }

        public string Name => "Is Not Near Hero";

        public string Description
            => "Returns 1 when the hero is far from the target, and returns 0 as the hero moves within nearDistance of the target";
    }
}