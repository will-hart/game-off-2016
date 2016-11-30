// /** 
//  * CreepAttackNearbyTagets.cs
//  * Will Hart
//  * 20161103
// */

namespace GameGHJ.AI.Actions
{
    #region Dependencies

    using System.Collections.Generic;
    using System.Linq;
    using GameGHJ.AI.Axes;
    using GameGHJ.AI.Core;
    using GameGHJ.AI.Influence;
    using GameGHJ.Systems;
    using UnityEngine;

    #endregion

    public class StrategyApplyUpgradesAction : AbstractAiAction<StrategicAiStateComp>
    {
        public StrategyApplyUpgradesAction() : base(new List<IAxis<StrategicAiStateComp>>
        {
            new HasBuildingsWithAvailableUpgradesAxis(),
            new HasEnoughDnaForBuildingUpgradeAxis()
        })
        {
        }

        public override int Priority => 1;

        public override void OnEnter(AiContext<StrategicAiStateComp> context)
        {
            base.OnEnter(context);
            
            var buc = ZenBehaviourManager.Instance.Get<BuildingUpgradeComp>(ComponentTypes.BuildingUpgradeComp)
                .FirstOrDefault(
                    b =>
                        b.Owner.GetComponent<UnitPropertiesComp>().teamID == context.State.FriendlyTeamId &&
                        b.possibleBuildingTypes > 0);
            
            if (buc == null) return;

            var teamId = buc.Owner.GetComponent<UnitPropertiesComp>().teamID;
            var side = ZenBehaviourManager.Instance.Get<SidePropertiesComp>(ComponentTypes.SidePropertiesComp)
                .FirstOrDefault(s => s.SideId == teamId);

            if (side == null) return;
            side.Dna -= 50;

            var isTower = buc.Owner.HasComponent(ComponentTypes.TowerComp);

            var buildingType = GetBuildingType(isTower);
            CreateBuilding(buc, buildingType);
        }

        public override void Update(AiContext<StrategicAiStateComp> context)
        {
            IsComplete = true;
            return;
        }

        private static void CreateBuilding(ComponentECS buc, string buildingType)
        {
            //BuildingConstructionController.Instance.BeginBuildProcess(ConstructableTypes.Factory);
            var teamId = buc.Owner.GetComponent<UnitPropertiesComp>().teamID;
            var initpos = buc.Owner.Wrapper.gameObject.transform.position; // NOTE need to do it this way I think because of insta-build / upgrade and not having time to set the position comp data

            Debug.Log($"Upgrading {buc.Owner.Wrapper.name} at {initpos} to {buildingType}");

            Object.Destroy(buc.Owner.Wrapper.gameObject);

            var newbld = EntityFactory.Instance.CreateGameObjectFromTemplate(buildingType);
            newbld.GetComponent<EntityWrapper>().entity.GetComponent<UnitPropertiesComp>().teamID = teamId;
            newbld.transform.position = initpos;
        }

        private static string GetBuildingType(bool isTower)
        {
            if (isTower)
            {
                return Random.value > 0.5 ? "TowerLaser" : "TowerMissile";
            }

            return Random.value > 0.5 ? "BuildingMeleeCreep" : "BuildingRangedCreep1";
        }
    }
}