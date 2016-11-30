using System;
using GameGHJ.Systems;
using UnityEngine;

public class BuildingConstructionController : ZenSingleton<BuildingConstructionController>, IOnAwake, IOnUpdate
{
	public override Type ObjectType => typeof(BuildingConstructionController);
	public override int ExecutionPriority => 0;

	public GameObject factoryPrefab;
	public GameObject towerPrefab;
	public GameObject placeableBuilding;

	private int terrainLayerMask;
	private ConstructionHelper ch;
	private ConstructableTypes typeBeingConstructed;
	private bool isConstructing;
	public bool IsConstructing => isConstructing;

	public void OnAwake()
	{
		terrainLayerMask = LayerManager.TerrainMask;
		placeableBuilding = null;
	}

    public static GameObject BeginAiBuildProcess(ConstructableTypes cType, Vector3 position)
    {
        var go = CreateBuildingInstance(cType);
        go.transform.position = position;

        var consHelper = go.GetComponent<ConstructionHelper>();
        consHelper.StartAiConstruction();

        return go;
    }

	/// <summary>
	/// Called by the build button, spawns the hologram image
	/// </summary>
	public void BeginBuildProcess(ConstructableTypes cType)
	{
		placeableBuilding = CreateBuildingInstance(cType);
        typeBeingConstructed = cType;
		Vector3 initpos = ZenUtils.GetTerrainPositionFromCursor(LayerManager.TerrainMask);
		ch = placeableBuilding.GetComponent<ConstructionHelper>();
		placeableBuilding.transform.position = initpos;
		isConstructing = true;
		PlayerControlSystem.Instance.SetState(CursorSelectStates.Construction);
	}

	public void CancelBuildProcess()
	{
		if (!isConstructing) return;
		Destroy(placeableBuilding);
		placeableBuilding = null;
		isConstructing = false;
		PlayerControlSystem.Instance.SetState(CursorSelectStates.Normal);
	}

    public void OnUpdate()
	{
		if (placeableBuilding == null) return;

		placeableBuilding.transform.position = ZenUtils.GetTerrainPositionFromCursor(LayerManager.TerrainMask);

		if (Input.GetMouseButtonUp(0)) // try to place
		{
			if (ch.TryPlaceBuilding())
			{
				Debug.Log("Placed");
				placeableBuilding = null;
				PlayerControlSystem.Instance.SetState(CursorSelectStates.Normal);
			}
		}
	}

	public void TriggerConstructionFinished(GameObject constructedObject)
	{
		Debug.Log($"In BuildingConstructionController.TriggerConstructionFinished");
		isConstructing = false;
		var ew = constructedObject.GetComponent<EntityWrapper>();
		var bc = ew.entity.GetComponent<BuildingComp>();
    }

    private static GameObject CreateBuildingInstance(ConstructableTypes cType)
    {
        return EntityFactory.Instance.CreateGameObjectFromTemplate(
                cType == ConstructableTypes.Factory ? "BaseFactory" : "BaseTower");
    }
}

public enum ConstructableTypes
{
	Factory,
	Tower
}