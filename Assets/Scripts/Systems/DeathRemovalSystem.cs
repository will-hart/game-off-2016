// /** 
//  * AbilitySystem.cs
//  * Will Hart
//  * 20161109
// */

namespace GameGHJ.Systems
{
    #region Dependencies

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    #endregion

    /// <summary>
    ///     Updates influence maps once every second
    /// </summary>
    public class DeathRemovalSystem : ZenBehaviour, IOnUpdate
    {
        public override Type ObjectType => typeof(DeathRemovalSystem);
        public override int ExecutionPriority => 1500;

	    [SerializeField] private GameObject ExplosionPrefab;
		private Vector3 ExplosionVerticalOffset = new Vector3(0, 2, 0);


        public void OnUpdate()
        {
            var casualties = new List<Entity>(
                ZenBehaviourManager.Instance.Get<HealthComp>(ComponentTypes.HealthComp)
                    .Where(h => h.isDead)
                    .Select(h => h.Owner));

            foreach (var destroyMe in casualties)
            {

				//Explosion
				ExplosionPrefab.InstantiateFromPool(destroyMe.Wrapper.transform.position + ExplosionVerticalOffset, Quaternion.identity);

				if (destroyMe.HasComponent(ComponentTypes.CreepComp))
                {
                    CleanupCreep(destroyMe);
                    DestroyEntity(destroyMe);
                }
                else if (destroyMe.HasComponent(ComponentTypes.HeroComp))
                {
                    RespawnHero(destroyMe);
                }
                else if (destroyMe.HasComponent(ComponentTypes.TowerComp) || destroyMe.HasComponent(ComponentTypes.BuildingComp))
                {
	                CheckIfCortex(destroyMe);
                    DestroyEntity(destroyMe);
                }
            }
        }

        private static void CleanupCreep(Entity entity)
        { 
            // decrement hero count
            if (entity.GetComponent<CreepComp>().AssignedHero != null)
            {
                entity.GetComponent<CreepComp>().AssignedHero.Owner.GetComponent<HeroComp>().DeployedCreeps -= 1;
            }
        }

        private static void DestroyEntity(Entity entity)
        {
            // destroy the entity
            foreach (var comp in entity.Components)
            {
                ZenBehaviourManager.Instance.DestroyComponent(comp);
            }
			
            Destroy(entity.Wrapper.gameObject);
        }

        private static void RespawnHero(Entity heroEntity)
        {
            var sideId = heroEntity.GetComponent<UnitPropertiesComp>().teamID;
            var side = ZenBehaviourManager.Instance
                .Get<SidePropertiesComp>(ComponentTypes.SidePropertiesComp)
                .FirstOrDefault(s => s.SideId == sideId);

            if (side == null)
            {
                Debug.LogWarning($"Unable to find a side component for hero {heroEntity.Wrapper}, deleting");
                DestroyEntity(heroEntity);
                return;
            }

            heroEntity.GetComponent<PositionComp>().transform.position =
                side.Owner.GetComponent<PositionComp>().position + new Vector3(5, 3, 5);

            // TODO add a death timer and require rebuilding from the factory, see issue #25
            var health = heroEntity.GetComponent<HealthComp>();
            health.currentHealth = health.maxHealth;
        }

	    private void CheckIfCortex(Entity ent)
	    {
		    if (ent.HasComponent(ComponentTypes.BuildingComp))
		    {
			    var bc = ent.GetComponent<BuildingComp>();
			    if (bc.buildingType == BuildingType.Cortex)
			    {
				    if (ent.GetComponent<UnitPropertiesComp>().teamID == 1) //is player
					    GameFlowManager.Instance.PlayerLost();
				    else GameFlowManager.Instance.PlayerWon();
			    }
		    }
	    }
    }
}