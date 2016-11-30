using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using GameGHJ.Systems;

public class BaseTowerClickHandler : ZenBehaviour, IOnAwake
{
	public override int ExecutionPriority => 0;
	public override Type ObjectType => typeof(BaseTowerClickHandler);

	private SidePropertiesComp playerProps;

	public void OnAwake()
	{
		playerProps = ZenBehaviourManager.Instance.Get<SidePropertiesComp>(ComponentTypes.SidePropertiesComp).First(x => x.IsPlayerControlled == true);
	}

	public void BuildLaserTower()
	{
		//PlayerControlSystem.Instance.SetState(CursorSelectStates.Construction);
		Debug.Log("Clicked build laser tower");

		//Check price
		if (!CheckAndDeductCost())
			return;

		//BuildingConstructionController.Instance.BeginBuildProcess(ConstructableTypes.Factory);
		var bldg = PlayerControlSystem.Instance.selectedObjects[0];
        var teamId = bldg.Owner.GetComponent<UnitPropertiesComp>().teamID;
        Vector3 initpos = bldg.Owner.GetComponent<PositionComp>().position;

		Destroy(bldg.Owner.Wrapper.gameObject);
		var newbld = EntityFactory.Instance.CreateGameObjectFromTemplate("TowerLaser");
        newbld.GetComponent<EntityWrapper>().entity.GetComponent<UnitPropertiesComp>().teamID = teamId;
        newbld.transform.position = initpos;
	}

	public void BuildMissileTower()
	{
		//PlayerControlSystem.Instance.SetState(CursorSelectStates.Construction);
		Debug.Log("Clicked build missile tower");

		//Check price
		if (!CheckAndDeductCost())
			return;

		//BuildingConstructionController.Instance.BeginBuildProcess(ConstructableTypes.Factory);
		var bldg = PlayerControlSystem.Instance.selectedObjects[0];
        var teamId = bldg.Owner.GetComponent<UnitPropertiesComp>().teamID;
        Vector3 initpos = bldg.Owner.GetComponent<PositionComp>().position;

		Destroy(bldg.Owner.Wrapper.gameObject);
		var newbld = EntityFactory.Instance.CreateGameObjectFromTemplate("TowerMissile");
        newbld.GetComponent<EntityWrapper>().entity.GetComponent<UnitPropertiesComp>().teamID = teamId;
        newbld.transform.position = initpos;
	}

	private bool CheckAndDeductCost()
	{
		if (playerProps.Dna < 50)
		{
			GUIStatLabelManager.Instance.SetNotificationText("Insufficient DNA");
			return false;
		}

		playerProps.Dna -= 50;
		return true;
	}
}
