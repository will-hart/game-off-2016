#pragma warning disable 0414, 0219, 649, 169, 618, 1570
using System;
using System.Collections.Generic;
using GameGHJ.Systems;
using UnityEngine;

public class GUIManagementSystem : ZenSingleton<GUIManagementSystem>, IOnAwake, IOnStart, IOnUpdate
{
	public override int ExecutionPriority => 0;
	public override System.Type ObjectType => typeof(GUIManagementSystem);

	public PlayerControlSystem uss;
	public bool IsDirty;

	[SerializeField] private UIGrid selectedUnitsGrid;
	[SerializeField] private GameObject selectedUnitPrefab;
	[SerializeField] private GameObject BuildsWidget;
	[SerializeField] private GameObject CommandsWidget;
	[SerializeField]
	private GameObject FactoryUpgradeWidget;
	[SerializeField]
	private GameObject TowerUpgradeWidget;

	[SerializeField] private GameObject FactoryProductionWidget;

	private List<Transform> selectedObjectList = new List<Transform>();

	public void OnAwake()
	{
		IsDirty = true;
		if (!BuildsWidget) BuildsWidget = GameObject.Find("BuildsWidget");
		if (!CommandsWidget) CommandsWidget = GameObject.Find("CommandsWidget");
	}

	public void OnStart()
	{
 
	}

	public void OnUpdate()
	{
		if (!IsDirty) return;
		selectedUnitsGrid.enabled = true;

		IsDirty = false;

		//Selected units panel
		var gridchild = selectedUnitsGrid.GetChildList();
		foreach (var ob in gridchild)
		{
			//ob.gameObject.Release(); // return to pool
			Destroy(ob.gameObject);
		}

		gridchild.Clear();
		//selectedObjectList.Clear();

		foreach (var sel in  PlayerControlSystem.Instance.selectedObjects)
		{
			var newItem = selectedUnitPrefab.InstantiateFromPool();
			
			newItem.transform.SetParent(selectedUnitsGrid.transform);
			//selectedUnitsGrid.AddChild(newItem.transform);
			newItem.GetComponent<UISprite>().spriteName = ResourcesManager.GuiIconMapping[sel.GuiIconType];

			var tf = newItem.transform;
			var lp = tf.localPosition;
			tf.localScale = Vector3.one;
			//tf.localPosition = new Vector3(lp.x, lp.y, 0);
			tf.localPosition = Vector3.zero;

			//selectedObjectList.Add(newItem.transform);
		}

		//enable grid so it spreads properly
		
		//gridchild.AddRange(selectedObjectList);
		
		selectedUnitsGrid.Reposition();

		UpdateContextBox();
	}

	public void UpdateContextBox()
	{
		// todo: This is just all kinds of horrid hardcoding
		int selcount = PlayerControlSystem.Instance.selectedObjects.Count;
		if (selcount < 1) //Nothing selected, display build stuff
		{
			Debug.Log($"Nothing Selected");
			ActivateNoSelectionWidget();
		} else if (selcount == 1 && PlayerControlSystem.Instance.selectedObjects[0].Owner.HasComponent(ComponentTypes.BuildingComp))
		{
			//building selected
			var bc = PlayerControlSystem.Instance.selectedObjects[0].Owner.GetComponent<BuildingComp>();
			if (bc.buildingType == BuildingType.BaseFactory)
			{
				Debug.Log($"Base factory selected");
				ActivateFactoryUpgradeWidget();
			} else if (bc.buildingType == BuildingType.BaseTower)
			{
				Debug.Log($"Base tower selected");
				ActivateTowerUpgradeWidget();
			} else if (bc.buildingType == BuildingType.MeleeCreepBuilding || bc.buildingType == BuildingType.RangedCreepBuilding)
			{
				Debug.Log($"Prod building selected");
				ShowFactoryHeroPanel(bc);
			}
			else // Just turn everything off, building upgraded to full already
			{
				Debug.Log($"Upgraded building or cortex selected");
				DeactivateWidgets();
			}
			
		}
		else // units selected
		{
			Debug.Log($"Units selected");
			ActivateUnitCommandsWidget();
		}
	}

	private void ShowFactoryHeroPanel(BuildingComp bc)
	{
		DeactivateWidgets();
		var cpc = bc.Owner.GetComponent<CreepProductionComp>();
		if (cpc.AssignedHero?.Name == null)
		{
			Debug.Log($"assigned hero null, showing hero build");
			FactoryProductionWidget.SetActive(true);
		}

	}

	//ghetto method of removing hero build ability of a factory immediately after successfully building one
	public void HideFactoryHeroPanel()
	{
		FactoryProductionWidget.SetActive(false);
	}

	private void ActivateUnitCommandsWidget()
	{
		DeactivateWidgets();
		CommandsWidget.SetActive(true);
	}

	private void ActivateNoSelectionWidget()
	{
		DeactivateWidgets();
		BuildsWidget.SetActive(true);
	}

	private void ActivateFactoryUpgradeWidget()
	{
		DeactivateWidgets();
		FactoryUpgradeWidget.SetActive(true);
	}

	private void ActivateTowerUpgradeWidget()
	{
		DeactivateWidgets();
		TowerUpgradeWidget.SetActive(true);
	}

	private void DeactivateWidgets()
	{
		BuildsWidget.SetActive(false);
		CommandsWidget.SetActive(false);
		FactoryUpgradeWidget.SetActive(false);
		TowerUpgradeWidget.SetActive(false);
		FactoryProductionWidget.SetActive(false);
	}
}
