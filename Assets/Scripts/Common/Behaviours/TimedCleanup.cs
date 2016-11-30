using UnityEngine;
using System.Collections;

public class TimedCleanup : MonoBehaviour 
{

	void Awake()
	{
		gameObject.SetActive(false);
	}

	// Use this for initialization
	void OnEnable() 
	{
		this.gameObject.ReleaseDelayed(4f);
	}
}
