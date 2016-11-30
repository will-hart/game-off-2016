#pragma warning disable 0414, 0219, 649, 169, 618, 1570
using System;
using GameGHJ.Common.ZenECS;
using UnityEngine;

public class DebugSystem : ZenBehaviour, IOnStart, IOnUpdate
{
	
	public override System.Type ObjectType => typeof(DebugSystem);
	public override int ExecutionPriority => 0;

	private static readonly Vector3 SpawnOffsetToPreventFallingThroughTheGround = new Vector3(12, 5, 0);

	public void OnStart()
	{
		// sides
		CreateTeam("SideA", 1, new Vector3(105, 1, 125));
		CreateTeam("SideB", 2, new Vector3(135, 1, 175));

		// resource points
		//CreateEntity("Buildings", "ResourcePoint", new Vector3(30, 5, 60));
		//CreateEntity("Buildings", "ResourcePoint", new Vector3(175, 5, 30));
		//CreateEntity("Buildings", "ResourcePoint", new Vector3(165, 5, 132));
		//CreateEntity("Buildings", "ResourcePoint", new Vector3(33, 5, 170));
		//CreateEntity("Buildings", "ResourcePoint", new Vector3(95, 5, 106));

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
		sideEntity.GetComponent<EntityWrapper>().entity.GetComponent<SidePropertiesComp>().Dna = 5000;

		var bldg = CreateEntity("Buildings", "BuildingRangedCreep1", hqPos);
		bldg.GetComponent<EntityWrapper>().entity.GetComponent<UnitPropertiesComp>().teamID = sideId;
		
		var hero = CreateEntity("Heroes", "HeroMoveTest", hqPos + SpawnOffsetToPreventFallingThroughTheGround);
		hero.GetComponent<EntityWrapper>().entity.GetComponent<UnitPropertiesComp>().teamID = sideId;
		bldg.GetComponent<EntityWrapper>().entity.GetComponent<CreepProductionComp>().AssignedHero =
			hero.GetComponent<EntityWrapper>().entity.GetComponent<HeroComp>();

		hero.GetComponent<EntityWrapper>().entity.GetComponent<HeroComp>().MaxCreeps = 2;

		//var tower = CreateEntity("Buildings", "Tower", hqPos + 2 * SpawnOffsetToPreventFallingThroughTheGround);
		//tower.GetComponent<EntityWrapper>().entity.GetComponent<UnitPropertiesComp>().teamID = sideId;
	}

	private static GameObject CreateEntity(string parentName, string type, Vector3 position)
	{
		var parentObj = GameObject.Find(parentName);
		var go = EntityFactory.Instance.CreateGameObjectFromTemplate(type, parentObj.transform);
		go.GetComponent<Rigidbody>().MovePosition(position);
		return go;
	}


	public void OnUpdate()
	{

	}

}
