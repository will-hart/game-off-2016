using UnityEngine;
using System.Collections;

public class HealthBarController : MonoBehaviour
{
	public GameObject healthBarPrefab;
	public Transform target;

	private HealthComp healthComp;
	private UISlider slider;
	// Use this for initialization
	void Start ()
	{
		var ent = GetComponent<EntityWrapper>().entity;
		healthComp = ent.GetComponent<HealthComp>();
		// We need the HUD object to know where in the hierarchy to put the element
		if (HUDRoot.go == null)
		{
			GameObject.Destroy(this);
			return;
		}

		GameObject child = NGUITools.AddChild(HUDRoot.go, healthBarPrefab);
		//hudText = child.GetComponentInChildren<HUDText>();

		// Make the UI follow the target
		child.AddComponent<UIFollowTarget>().target = target;
		slider = child.GetComponentDownward<UISlider>();
		var team = ent.GetComponent<UnitPropertiesComp>().teamID;
		var bar = slider.foregroundWidget as UISprite;
		if (team == 1) // player
		{
			bar.color = new Color(1f, 0, 0, 1f);
		}
		else
		{
			bar.color = new Color(0.5f, 0.5f, 1f, 1f);
		}
	}

	void Update()
	{
		slider.value = healthComp.currentHealth/healthComp.maxHealth;
	}
	
}
