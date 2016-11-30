// /** 
//  * IsNotNearHeroAxis.cs
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

    public class IsNearHeroAxis : IAxis<TacticalAiStateComp>
    {
        private readonly float _nearDistance;

        public IsNearHeroAxis(float nearDistance = 12)
        {
            _nearDistance = nearDistance;
        }

        public float Score(AiContext<TacticalAiStateComp> context)
        {
            var hero = context.GetComponent<CreepComp>().AssignedHero;
            if (hero == null) return 0;

            var pos = context.GetComponent<PositionComp>().position;
            var sqrMag = (hero.position - pos).sqrMagnitude;

            return Functions.Quartic(_nearDistance*_nearDistance/sqrMag);
        }

        public string Name => "Is Near Hero";
        public string Description => "Returns 1 when the position is near the hero, and returns 0 as the hero moves outside the passed nearDistance";
    }
}