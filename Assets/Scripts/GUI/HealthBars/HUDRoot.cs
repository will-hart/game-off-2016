using UnityEngine;

public class HUDRoot : MonoBehaviour
{
	public static GameObject go;
	
	public float scaleCoeff = 0.1f;

	//scale bigger as Y gets smaller
	private Transform mainCam;

	void Awake()
	{
		go = gameObject;
		mainCam = Camera.main.transform;
	}

	void Update()
	{
		var x = mainCam.position.y;
		float scaleRatioX =  0.5f + (x - 20) * -0.005f;
		float scaleRatioY = 0.5f + (x - 20) * -0.003f;
		transform.localScale = new Vector3(scaleRatioX, scaleRatioY, transform.localScale.z);
	}
}