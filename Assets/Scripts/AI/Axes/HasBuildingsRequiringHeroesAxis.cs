// /** 
//  * HasBuildingsRequiringHeroesAxis.cs
//  * Will Hart
//  * 20161127
// */

namespace GameGHJ.AI.Axes
{
    #region Dependencies

    using System.Linq;
    using GameGHJ.AI.Core;

    #endregion

    public class HasBuildingsRequiringHeroesAxis : IAxis<StrategicAiStateComp>
    {
        public float Score(AiContext<StrategicAiStateComp> context)
        {
            var anyNull = ZenBehaviourManager.Instance.Get<CreepProductionComp>(ComponentTypes.CreepProductionComp)
                .Where(cpc => cpc.AssignedHero == null)
                .ToList();

            var anyBuildings = ZenBehaviourManager.Instance
                .Get<CreepProductionComp>(ComponentTypes.CreepProductionComp)
                .Any(
                    cpc => (cpc.AssignedHero == null) &&
                           (cpc.Owner.GetComponent<UnitPropertiesComp>().teamID == context.State.FriendlyTeamId));

            return Functions.OneIfTrue(anyBuildings);
        }

        public string Name => "Has A Build Order";
        public string Description => "Returns 1 if there is a build order, otherwise returns 0";
    }
}