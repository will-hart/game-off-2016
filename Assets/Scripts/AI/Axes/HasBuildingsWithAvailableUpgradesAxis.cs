// /** 
//  * HasBuildingsWithAvailableUpgradesAxis.cs
//  * Will Hart
//  * 20161127
// */

namespace GameGHJ.AI.Axes
{
    #region Dependencies

    using System.Linq;
    using GameGHJ.AI.Core;
    using UnityEngine;

    #endregion

    public class HasBuildingsWithAvailableUpgradesAxis : IAxis<StrategicAiStateComp>
    {
        public float Score(AiContext<StrategicAiStateComp> context)
        {
            var count = ZenBehaviourManager.Instance.Get<BuildingUpgradeComp>(ComponentTypes.BuildingUpgradeComp)
                .Count(
                    buc =>
                        buc.Owner.GetComponent<UnitPropertiesComp>().teamID == context.State.FriendlyTeamId &&
                        buc.possibleBuildingTypes > 0);
            return Mathf.Clamp01(count);
        }

        public string Name => "Has Buildings With Available Upgrades";
        public string Description => "Returns 0 if there are no available upgrades, 1 if there are any";
    }
}