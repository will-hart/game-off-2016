
// /** 
//  * HeroPopulationMaxedOutAxis.cs
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

    public class HeroPopulationMaxedOutAxis : IAxis<StrategicAiStateComp>
    {
        public float Score(AiContext<StrategicAiStateComp> context)
        {
            var friendlyId = context.State.FriendlyTeamId;

            // TODO is Side.LivingHeroes actually populated? If yes, use that instead
            return Mathf.Clamp01(ZenBehaviourManager.Instance.Get<HeroComp>(ComponentTypes.HeroComp)
                .Where(h => h.Owner.GetComponent<UnitPropertiesComp>().teamID == friendlyId)
                .DefaultIfEmpty(new HeroComp
                {
                    DeployedCreeps = 0,
                    MaxCreeps = 1
                })
                .Average(h => h.DeployedCreeps / (float)h.MaxCreeps));
        }

        public string Name => "Hero Population Maxed Out";
        public string Description => "Returns the average percentage creep population fill for all heroes";
    }
}