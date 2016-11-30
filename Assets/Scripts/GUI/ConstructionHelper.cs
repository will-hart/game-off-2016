using System;
using System.Linq;
using Unitilities;
using UnityEngine;

public class ConstructionHelper : ZenBehaviour, IOnAwake, IOnUpdate
{
	public override Type ObjectType => typeof(ConstructionHelper);
	public override int ExecutionPriority => 0;

	private int numObstructions = 0;
	private ConstructionShaderScript css;
	private bool buildingPlaced = false;
	//private bool IsGreen;
	private SidePropertiesComp playerProps;
	private EntityWrapper ew;

	public void OnAwake()
	{
		playerProps = ZenBehaviourManager.Instance.Get<SidePropertiesComp>(ComponentTypes.SidePropertiesComp).First(x => x.IsPlayerControlled == true);
		css = GetComponent<ConstructionShaderScript>();
		ew = GetComponent<EntityWrapper>();
	}

	public void OnUpdate()
	{
		if (buildingPlaced) return;
		if (numObstructions > 0)
			css.SetHologramColorRed();
		else css.SetHologramColorGreen();
	}

	public bool TryPlaceBuilding()
	{
		if (numObstructions > 0)
		{
			Debug.Log($"Stuff in the way, can't place here.");
			//todo: Make bzzt sound effect
			GUIStatLabelManager.Instance.SetNotificationText("Can't Place.");
			return false;
		}

		//todo: Get this cost from the json
		//var cost = ew.entity.GetComponent<BuildingComp>().ConstructionCost;

		if (100 > playerProps.Dna)
		{
			//Don't have enough dna resources
			GUIStatLabelManager.Instance.SetNotificationText("Not enough resources");
			return false;
		}

		playerProps.Dna -= 100;
        StartConstruction();
		return true;
	}

    public void StartAiConstruction()
    {
        StartConstruction();
    }

    public void TriggerConstructionFinished()
	{
		Debug.Log($"Construction finished, swap to factory/tower prefab");
		BuildingConstructionController.Instance.TriggerConstructionFinished(this.gameObject);
	}

	public void OnTriggerEnter(Collider other)
	{
		//Debug.Log($"Trig enter: {other.gameObject.name}");
		numObstructions++;
	}

	public void OnTriggerExit(Collider other)
	{
		//Debug.Log($"Trig exit: {other.gameObject.name}");
		numObstructions--;
    }

    private void StartConstruction()
    {
        buildingPlaced = true;
        css.TriggerConstruct();
        var rb = this.GetComponentDownward<Rigidbody>();
        var col = this.GetComponentDownward<Collider>();
        rb.isKinematic = false;
        col.isTrigger = false;
        gameObject.SetLayerRecursively(LayerManager.LayerFromName(LayerNames.SelectableObstacles));
		
    }
}