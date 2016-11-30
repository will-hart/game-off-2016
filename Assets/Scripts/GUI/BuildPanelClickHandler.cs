#pragma warning disable 0414, 0219, 649, 169, 618, 1570
using System;
using GameGHJ.Systems;
using UnityEngine;

public class BuildPanelClickHandler : ZenBehaviour, IOnAwake
{
	public override int ExecutionPriority => 0;
	public override Type ObjectType => typeof(BuildPanelClickHandler);

	public void OnAwake()
	{

	}

	public void BuildFactory()
	{
		//PlayerControlSystem.Instance.SetState(CursorSelectStates.Construction);
		Debug.Log("Clicked build factory");
		//var css = FindObjectOfType<ConstructionShaderScript>();
		//css.TriggerConstruct();
		BuildingConstructionController.Instance.BeginBuildProcess(ConstructableTypes.Factory);
	}

	public void BuildTower()
	{
		Debug.Log($"Clicked build tower");
		BuildingConstructionController.Instance.BeginBuildProcess(ConstructableTypes.Tower);
	}
}
