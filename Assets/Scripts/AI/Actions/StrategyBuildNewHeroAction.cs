// /** 
//  * StrategyBuildNewHeroAction.cs
//  * Will Hart
//  * 20161127
// */

namespace GameGHJ.AI.Actions
{
    #region Dependencies

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GameGHJ.AI.Axes;
    using GameGHJ.AI.Core;
    using UnityEngine;

    #endregion

    public class StrategyBuildNewHeroAction : AbstractAiAction<StrategicAiStateComp>
    {
        public StrategyBuildNewHeroAction() : base(new List<IAxis<StrategicAiStateComp>>
        {
            new HasBuildingsRequiringHeroesAxis(),
            new HasEnoughDnaForHeroAxis()
        })
        {
        }

        public override int Priority => 2;

        public override void OnEnter(AiContext<StrategicAiStateComp> context)
        {
            base.OnEnter(context);

            var building = ZenBehaviourManager.Instance
                .Get<CreepProductionComp>(ComponentTypes.CreepProductionComp)
                .FirstOrDefault(cpc => (cpc.AssignedHero == null) &&
                           (cpc.Owner.GetComponent<UnitPropertiesComp>().teamID == context.State.FriendlyTeamId));

            if (building == null)
            {
                IsComplete = true;
                return;
            }

            context.GetComponent<SidePropertiesComp>().Dna -= 100; // TODO is it always 100 to buy a hero?

            var offset = building.SpawnOffset;
            var bt = building.Owner.GetComponent<BuildingComp>().buildingType;
            var pos = building.Owner.GetComponent<PositionComp>().position;

            var hero = CreateEntity(
                "Heroes",
                bt == BuildingType.MeleeCreepBuilding ? "HeroMelee" : "HeroRanged",
                pos + offset);

            var entity = hero.GetComponent<EntityWrapper>().entity;
            entity.GetComponent<UnitPropertiesComp>().teamID = context.State.FriendlyTeamId;

            building.AssignedHero = entity.GetComponent<HeroComp>();

            IsComplete = true;
        }

        private static GameObject CreateEntity(string parentName, string type, Vector3 position)
        {
            var parentObj = GameObject.Find(parentName);
            var go = EntityFactory.Instance.CreateGameObjectFromTemplate(type, parentObj.transform);
            go.GetComponent<Rigidbody>().MovePosition(position);
            return go;
        }
    }
}