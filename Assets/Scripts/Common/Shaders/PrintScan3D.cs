using UnityEngine;
using System.Collections;

public class PrintScan3D : MonoBehaviour
{
	public Material currentMat;
	public Material holoMat;
	public float minY = 0;
	public float maxY = 2;
	public float duration = 25;
	private Renderer rendererForMat;
	private Material[] shipMats;
	private MeshFilter shipMesh;
	private float progress;
	public float startDelay;
	public Material warpMat;
	public Material standardMat;
	private bool finished = false;
	private bool shipSpawnFinished = false;
	private bool fadeInFinished = false;
	public GameObject holographicMeshGO;
	public Renderer hologramRenderer;
	public GameObject nonHoloMeshGO;

	void Awake()
	{
		rendererForMat = nonHoloMeshGO.GetComponent<Renderer>();
		hologramRenderer = holographicMeshGO.GetComponent<Renderer>();
		shipMats = rendererForMat.materials;
		
		shipMesh = nonHoloMeshGO.GetComponent<MeshFilter>();
		maxY = shipMesh.mesh.bounds.max.y;
		minY = shipMesh.mesh.bounds.min.y;
		progress = Time.time;
		rendererForMat.enabled = false;
		hologramRenderer.enabled = true;

	}

	void ChangeMainMaterial(Material mat)
	{
		shipMats[0] = mat;
		rendererForMat.materials = shipMats;
		currentMat = rendererForMat.materials[0];
	}

	void Start()
	{
		ChangeMainMaterial(warpMat);
		currentMat.SetColor("_ConstructColor", Color.cyan);

		holoMat = hologramRenderer.materials[0];
		holoMat.SetFloat("_Fade", 0.05f);
	}

	//Overwhelmingly inefficient and pooly coded, can't be assed to fix now since it 'works'
	void Update()
	{
		if (Time.time < startDelay || finished) return;
		if (rendererForMat.enabled == false)
			rendererForMat.enabled = true;
		if (hologramRenderer.enabled == false)
			hologramRenderer.enabled = true;

		float percentDone = progress / duration;

		if (!shipSpawnFinished)
		{
			//Apply scan-in effect by setting shader's ConstructY to lerp value
			float y = Mathf.Lerp(minY, maxY, percentDone);
			currentMat.SetFloat("_ConstructY", y);
		}

		if (!fadeInFinished && percentDone > 1.0f)
		{
			//manage hologram's alpha via lerp like above
			float f = Mathf.Lerp(0.05f, 1.0f, percentDone -1 );
			holoMat.SetFloat("_Fade", f);
		}
		
		progress += Time.deltaTime;

		//Scan process finished
		if (percentDone > 1 && !shipSpawnFinished)
		{
			//progress = 0;
			
			//ChangeMainMaterial(standardMat);
			//hologramRenderer.enabled = true;
			shipSpawnFinished = true;
			//fadeInFinished = true;
		}

		//hologram fade in finished
		if (percentDone >= 1.5) //hell yeah 150% done baby
		{
			fadeInFinished = true;
			//swap from scan material to the "good" material while hologram hides the ugly
			ChangeMainMaterial(standardMat);
		}
		
		if (fadeInFinished && !finished) //done swapping mats, fade out hologram
		{
			float f = Mathf.Lerp(1.0f, 0.0f, progress );
			holoMat.SetFloat("_Fade", f);

			if (f <= 0.001)
			{
				finished = true;
				holoMat.SetFloat("_Fade", 0);
				//done w/ hologram at this point, could delete the whole thing if you wanted
				hologramRenderer.enabled = false;
			}
		}
	}

}
