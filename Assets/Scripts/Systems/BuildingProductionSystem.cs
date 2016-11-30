// /** 
//  * BuildingProductionSystem.cs
//  * Will Hart
//  * 20161104
// */

namespace GameGHJ.Systems
{
    #region Dependencies

    using System;
    using System.Linq;
    using UnityEngine;

    #endregion

    /// <summary>
    /// Periodically produces creeps from production buildings
    /// </summary>
    public class BuildingProductionSystem : ZenBehaviour, IOnUpdate
    {
        public override Type ObjectType  => typeof(BuildingProductionSystem);
        public override int ExecutionPriority => 0;

        public void OnUpdate()
        {
            var buildings = ZenBehaviourManager.Instance.Get<CreepProductionComp>(ComponentTypes.CreepProductionComp);

            foreach (var building in buildings)
            {
                var now = Time.time;
                var heroEnt = building.AssignedHero?.Owner;

				
				
                // don't produce if there is no hero, the hero is dead or the hero is at their creep limit
                if (building.NextProductionTime > now ||
                    heroEnt == null ||
                    heroEnt.GetComponent<HealthComp>().isDead ||
                    heroEnt.GetComponent<HeroComp>().DeployedCreeps >= heroEnt.GetComponent<HeroComp>().MaxCreeps) continue;

                building.NextProductionTime = now + building.ProductionPeriod;

                var pos = building.Owner.GetComponent<PositionComp>();
                
	            GenerateCreep(pos);
            }
        }

	    private static void GenerateCreep(PositionComp pc)
	    {
		    var prod = pc.Owner.GetComponent<CreepProductionComp>();
	        var sideId = pc.Owner.GetComponent<UnitPropertiesComp>().teamID;

            // check if we can afford the creep
            var side = ZenBehaviourManager.Instance.Get<SidePropertiesComp>(ComponentTypes.SidePropertiesComp)
                .FirstOrDefault(s => s.SideId == sideId);
            var creepData = ZenBehaviourManager.Instance.GetComponent<EntityFactory>().GetTemplate(prod.CreepTypeToProduce.ToString());
            var a = creepData.AsDictionary["_components"];
            var b = a.AsDictionary["CreepComp"];
            var c = b.AsDictionary["ConstructionCost"];
            var cost = (int)c.AsDouble;

            if (cost > side.Dna) return;
	        side.Dna -= cost;

            // produce the creep
            var creeptypename = ResourcesManager.CreepTypeMapping[prod.CreepTypeToProduce];

			var creepParent = GameObject.Find("Creeps");
			var go = EntityFactory.Instance.CreateGameObjectFromTemplate(creeptypename, creepParent.transform);
            go.GetComponent<Rigidbody>().MovePosition(pc.position + prod.SpawnOffset);
            var creepEntity = go.GetComponent<EntityWrapper>().entity;

	        creepEntity.GetComponent<UnitPropertiesComp>().teamID = sideId;
	        creepEntity.GetComponent<CreepComp>().AssignedHero = prod.AssignedHero.Owner.GetComponent<PositionComp>();
            prod.AssignedHero.DeployedCreeps++;

            //Debug.Log($"Created {go.name}");
	    }
    }
}