// /** 
//  * DebugSystemWilsk.cs
//  * Will Hart
//  * 20161110
// */

namespace GameGHJ.Systems
{

    #region Dependencies

    using System.Collections;
    using UnityEngine;

    #endregion

    public class DebugSystemWilsk : ZenBehaviour, IOnStart
    {
        public override System.Type ObjectType => typeof(DebugSystemWilsk);
        public override int ExecutionPriority => 0;

        private static readonly Vector3 SpawnOffsetToPreventFallingThroughTheGround = new Vector3(12, 0, 0);

        public void OnStart()
        {
            // sides
            CreateTeam("SideA", 1, new Vector3(30, 0, 30));
            CreateTeam("SideB", 2, new Vector3(135, 0, 170));

            // resource points
            CreateEntity("Buildings", "BuildingResourcePoint", new Vector3(30, 0, 60));
            CreateEntity("Buildings", "BuildingResourcePoint", new Vector3(175, 0, 30));
            CreateEntity("Buildings", "BuildingResourcePoint", new Vector3(165, 0, 132));
            CreateEntity("Buildings", "BuildingResourcePoint", new Vector3(33, 0, 170));
            CreateEntity("Buildings", "BuildingResourcePoint", new Vector3(95, 0, 106));

            // update pathfinding
            AstarPath.active.Scan();
        }

        /// <summary>
        /// Creates a tower, ranged building and a hero for the given side
        /// </summary>
        /// <param name="sideName"></param>
        /// <param name="sideId"></param>
        /// <param name="hqPos"></param>
        private static void CreateTeam(string sideName, int sideId, Vector3 hqPos)
        {
            var sideEntity = EntityFactory.Instance.CreateGameObjectFromTemplate(sideName);
            sideEntity.transform.position = hqPos;

            var crtx = CreateEntity("Buildings", "Cortex", hqPos);
            crtx.GetComponent<EntityWrapper>().entity.GetComponent<UnitPropertiesComp>().teamID = sideId;

            var bldg = CreateEntity("Buildings", "BuildingRangedCreep1", hqPos + SpawnOffsetToPreventFallingThroughTheGround);
            bldg.GetComponent<EntityWrapper>().entity.GetComponent<UnitPropertiesComp>().teamID = sideId;

            var hero = CreateEntity("Heroes", "HeroRanged", hqPos + 2 * SpawnOffsetToPreventFallingThroughTheGround);
            hero.GetComponent<EntityWrapper>().entity.GetComponent<UnitPropertiesComp>().teamID = sideId;
            hero.GetComponent<EntityWrapper>().entity.GetComponent<HeroComp>().Name = "Zentropy";
            bldg.GetComponent<EntityWrapper>().entity.GetComponent<CreepProductionComp>().AssignedHero =
                hero.GetComponent<EntityWrapper>().entity.GetComponent<HeroComp>();
        }

        private static GameObject CreateEntity(string parentName, string type, Vector3 position)
        {
            var parentObj = GameObject.Find(parentName);
            var go = EntityFactory.Instance.CreateGameObjectFromTemplate(type, parentObj.transform);
            go.transform.position = position;
            return go;
        }
    }
}