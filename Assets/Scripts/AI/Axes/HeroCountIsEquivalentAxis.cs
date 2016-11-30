
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

    public class HeroCountIsEquivalentAxis : IAxis<StrategicAiStateComp>
    {
        public float Score(AiContext<StrategicAiStateComp> context)
        {
            var heroes = ZenBehaviourManager.Instance.Get<HeroComp>(ComponentTypes.HeroComp)
                .Select(h => h.Owner.GetComponent<UnitPropertiesComp>())
                .ToList();
            var friendly = (float) heroes.Count(prop => prop.teamID == context.State.FriendlyTeamId);
            var enemy = (float) heroes.Count(prop => prop.teamID != context.State.FriendlyTeamId);
            
            return Functions.Logistic(friendly/enemy - 0.5f);
        }

        public string Name => "Hero Count Near Maximum";
        public string Description => "Returns the percentage of maximum hero count reached for this side";
    }
}