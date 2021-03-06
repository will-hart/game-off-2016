﻿#pragma warning disable 0414, 0219, 649, 169, 618, 1570
using System;
using System.Linq;
using GameGHJ.Systems;
using UnityEngine;

public class ProductionFactoryClickHandler : ZenBehaviour
{
	public override int ExecutionPriority => 0;
	public override Type ObjectType => typeof(ProductionFactoryClickHandler);

	private SidePropertiesComp _playerProps;

	public void GetPlayerProps()
	{
		_playerProps = ZenBehaviourManager.Instance.Get<SidePropertiesComp>(ComponentTypes.SidePropertiesComp).First(x => x.IsPlayerControlled == true);
	}
	
	private Vector3 SpawnOffsetToPreventFallingThroughTheGround;

	public void BuildHero()
	{
		Debug.Log("Clicked build hero");

		if (!CheckAndDeductCost()) return;

		var bldg = PlayerControlSystem.Instance.selectedObjects[0];
		Vector3 initpos = bldg.Owner.GetComponent<PositionComp>().position;

		GameObject hero;
		var bt = bldg.Owner.GetComponent<BuildingComp>().buildingType;
		SpawnOffsetToPreventFallingThroughTheGround = bldg.Owner.GetComponent<CreepProductionComp>().SpawnOffset;
	    hero = CreateEntity(
	        "Heroes",
	        bt == BuildingType.MeleeCreepBuilding ? "HeroMelee" : "HeroRanged",
	        initpos + SpawnOffsetToPreventFallingThroughTheGround);

	    hero.GetComponent<EntityWrapper>().entity.GetComponent<UnitPropertiesComp>().teamID = 1; //hardcoded player team
		bldg.Owner.GetComponent<CreepProductionComp>().AssignedHero =
			hero.GetComponent<EntityWrapper>().entity.GetComponent<HeroComp>();
		bldg.Owner.GetComponent<CreepProductionComp>().AssignedHero.Name = "NewProducedHero";

		GUIManagementSystem.Instance.HideFactoryHeroPanel();
	}

	private static GameObject CreateEntity(string parentName, string type, Vector3 position)
	{
		var parentObj = GameObject.Find(parentName);
		var go = EntityFactory.Instance.CreateGameObjectFromTemplate(type, parentObj.transform);
		go.GetComponent<Rigidbody>().MovePosition(position);
		return go;
	}

	private bool CheckAndDeductCost()
	{
	    if (_playerProps == null)
	    {
            GetPlayerProps();

            if (_playerProps == null) Debug.LogError("Unable to find a player, aborting");
	        return false;
	    }

	    if (_playerProps.Dna < 100)
		{
			GUIStatLabelManager.Instance.SetNotificationText("Insufficient DNA");
			return false;
		}

		_playerProps.Dna -= 100;
		return true;
	}
}
