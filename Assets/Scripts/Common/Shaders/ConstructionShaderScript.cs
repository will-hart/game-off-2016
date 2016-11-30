using System;
using UnityEngine;
using System.Collections;

public class ConstructionShaderScript : ZenBehaviour, IOnAwake, IOnUpdate
{
	public Material currentMat;
	
	public float minY = 0;
	public float maxY = 2;
	public float duration = 5;
	//private Renderer rendererForMat;
	private Material[] objMats;
	private MeshFilter objMesh;
	public float startDelay;
	public Material holoMat;
	public Material scanMat;
	public Material standardMat;
	private bool constructionBegun = false;
	private bool constructionFinished = false;
	public GameObject meshGO;
	public Renderer meshRenderer;
    private BuildingComp _buildingComp;
	
	public void OnAwake()
	{
		//rendererForMat = nonHoloMeshGO.GetComponent<Renderer>();
		meshRenderer = meshGO.GetComponent<Renderer>();
		objMats = meshRenderer.materials;

		objMesh = meshGO.GetComponent<MeshFilter>();
		maxY = objMesh.mesh.bounds.max.y;
		minY = objMesh.mesh.bounds.min.y;
		//progress = Time.time;
		//rendererForMat.enabled = true;
		meshRenderer.enabled = true;
		// HERE: Change this to use one mesh
		IsEnabled = true;

		ChangeMainMaterial(holoMat);
		currentMat.SetColor("_ConstructColor", Color.cyan);

		holoMat = meshRenderer.materials[0];
		holoMat.SetFloat("_Fade", 0.05f);
    }

	public void TriggerConstruct()
    {
        Debug.Log("Triggered construct");
		constructionBegun = true;
		ChangeMainMaterial(scanMat);
	    _buildingComp = gameObject.GetComponent<EntityWrapper>()?.entity.GetComponent<BuildingComp>();

	    if (_buildingComp == null)
	    {
	        Debug.LogWarning("Unable to find building component for construction, aborting");
	        return;
	    }

        _buildingComp.ConstructionPercentage = 0f;
	}

	//Overwhelmingly inefficient and pooly coded, can't be assed to fix now since it 'works'
	public void OnUpdate()
	{
		if (!constructionBegun || constructionFinished) return;

        _buildingComp.ConstructionPercentage += Time.deltaTime;
		float percentDone = _buildingComp.ConstructionPercentage / duration;
		
		//Apply scan-in effect by setting shader's ConstructY to lerp value
		float y = Mathf.Lerp(minY, maxY, percentDone);
		currentMat.SetFloat("_ConstructY", y);
		
		if (percentDone >= 1.0f) // Build progress done
		{
		    _buildingComp.ConstructionPercentage = 1;
            constructionFinished = true;
			Debug.Log("Completed construction");
			ChangeMainMaterial(standardMat);
			GetComponent<ConstructionHelper>().TriggerConstructionFinished();
			enabled = false;
		}
	}

	void ChangeMainMaterial(Material mat)
	{
		objMats[0] = mat;
		meshRenderer.materials = objMats;
		currentMat = meshRenderer.materials[0];
	}

	public void SetHologramColorRed()
	{
		currentMat.SetColor("_bLayerColorA", new Color(1.0f, 0, 0));
	}

	public void SetHologramColorGreen()
	{
		currentMat.SetColor("_bLayerColorA", new Color(0f, 1.0f, 0));
	}

	public override int ExecutionPriority => -100;
	public override Type ObjectType => typeof(ConstructionShaderScript);
}
