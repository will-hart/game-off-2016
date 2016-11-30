
// /** 
//  * HeroCountNearMaximumAxis.cs
//  * Will Hart
//  * 20161119
// */

namespace GameGHJ.AI.Axes
{
    #region Dependencies

    using System.Linq;
    using GameGHJ.AI.Core;
    using UnityEngine;

    #endregion

    public class HeroCountNearMaximumAxis : IAxis<StrategicAiStateComp>
    {
        public float Score(AiContext<StrategicAiStateComp> context)
        {
            var friendlyId = context.State.FriendlyTeamId;
            var side = ZenBehaviourManager.Instance.Get<SidePropertiesComp>(ComponentTypes.SidePropertiesComp)
                .FirstOrDefault(s => s.SideId == friendlyId);

            if (side == null) return 0;
            if (side.HeroLimit == 0) return 1;

            // TODO is Side.LivingHeroes actually populated? If yes, use that instead
            return Mathf.Clamp01(ZenBehaviourManager.Instance
                .Get<HeroComp>(ComponentTypes.HeroComp)
                .Count(h => h.Owner.GetComponent<UnitPropertiesComp>().teamID == friendlyId) / (float)side.HeroLimit);
        }

        public string Name => "Hero Count Near Maximum";
        public string Description => "Returns the percentage of maximum hero count reached for this side";
    }
}