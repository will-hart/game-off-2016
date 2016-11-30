using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Assets.Scripts.Weapons;
using Random = System.Random;

public class LightSaber_Launcher : ZenBehaviour, IOnAwake, IOnLateUpdate, IProjectileFirable
{
	public GameObject arcPrefab;
	public GameObject helperPrefab;
	public LaunchMethod launchMethod = LaunchMethod.forward_raycast;
	public float Distance = 100;
	public float PropagationSpeed = 10000;
	public LayerMask layers;
	public RayTransformBehaivour startBehaviour = RayTransformBehaivour.stick;
	public RayTransformBehaivour endBehaviour = RayTransformBehaivour.follow_raycast;


	public Transform globalParentTransform;

	public List<RayInfo> rays;


	protected List<RayInfo> destrArr;
	protected RaycastHit hit;

	//Zen
	private Random rng;
	protected bool hitDetected;
	protected RaycastHit detectedHit;

	public LaserData laserData;
	private int rayEndPointCount = 0;

	public List<RayInfo> Rays
	{
		get { return rays; }
	}

	public class RayInfo
	{
		public LightSaber_Arc arc;
		public Transform[] shape;
		public GameObject startObject;
		public GameObject endObject;
		public float distance;
		public int rayHash;
		public LaserData laserData;
		public Vector3 targetVec;
		public Vector3 targWorldPos;
		public Transform hitTargetTform;
		public Vector3 hitPoint;
	}

	public enum LaunchMethod
	{
		forward_raycast = 0,
		targeted_raycast = 1
	}

	public enum RayTransformBehaivour
	{
		immobile = 0,
		stick = 1,
		follow_raycast = 2
	}


	public void OnAwake()
	{
		rays = new List<RayInfo>();
		hit = new RaycastHit();
		destrArr = new List<RayInfo>();
		rng = new Random();


		launchMethod = LaunchMethod.targeted_raycast;
		startBehaviour = RayTransformBehaivour.stick;
		endBehaviour = RayTransformBehaivour.stick;

		arcPrefab = ResourcesManager.LaserTypesMapping[LaserTypes.LightningBig];
	}

	public void FireProjectile(CombatComp attacker, Transform target)
	{
		if (launchMethod == LaunchMethod.forward_raycast && startBehaviour == RayTransformBehaivour.follow_raycast)
		{
			Debug.LogError(
				"Launch method 'forward_raycast' and start behaviour 'follow_raycast' are incompatible. Change one of the settings.");
			return;
		}

		if (arcPrefab == null)
		{
			Debug.LogError("No arc prefab set.");
			return;
		}

		Transform start = transform;
		Transform end;
		GameObject tmpobj = new GameObject("rayEndPoint");

	    tmpobj.transform.position = target.position;
		

		//End position will be raycasted in any case
		end = tmpobj.transform;
		
		//Start position will depend on launch method
		tmpobj = new GameObject("rayStartPoint");
		start = tmpobj.transform;
		start.position = transform.position;

		start.parent = transform;
		start.rotation = transform.rotation;
		if (helperPrefab != null)
		{
			//tmpobj = (GameObject)Instantiate(helperPrefab);
			tmpobj = helperPrefab.InstantiateFromPool();
			tmpobj.transform.parent = start;
			tmpobj.transform.position = start.transform.position;
			tmpobj.transform.rotation = start.transform.rotation;
		}


		RayInfo rinfo = new RayInfo();
		//tmpobj = (GameObject)Instantiate(arcPrefab);
		tmpobj = arcPrefab.InstantiateFromPool();
		tmpobj.transform.parent = globalParentTransform;
		rinfo.arc = tmpobj.GetComponent<LightSaber_Arc>();
		//bool[] destrFlags = new bool[0];


		rinfo.shape = new Transform[2];
		rinfo.shape[0] = start;
		rinfo.shape[1] = end;
		rinfo.arc.shapeTransforms = rinfo.shape;
		//destrFlags = new bool[2];


		rinfo.arc.shapeTransforms = rinfo.shape;

		/*for(int i = 0; i <= destrFlags.Length-1; i++)
			destrFlags[i] = true;
		rinfo.arc.transformsDestructionFlags = destrFlags;*/


		rays.Add(rinfo);
	}


	// Update is called once per frame
	public void OnLateUpdate()
	{
		for (int x = 0; x < rays.Count; x++)
		{
			if (rays[x].arc == null)
			{
				destrArr.Add(rays[x]);
			}
		}

		CleanupSystem();
	}


	void CleanupSystem()
	{
		for (int x = destrArr.Count - 1; x >= 0; x--)
		{
			foreach (Transform tr in destrArr[x].shape)
			{
				if (tr)
					Destroy(tr.gameObject);
			}

			rays.RemoveAt(x);
		}
		if (destrArr.Count > 0)
			destrArr.Clear();
	}

	void DestroySystem()
	{
		for (int x = rays.Count - 1; x >= 0; x--)
		{
			foreach (Transform tr in rays[x].shape)
			{
				if (tr)
				Destroy(tr.gameObject);
			}
		}
		rays.Clear();
		destrArr.Clear();
	}

	public override int ExecutionPriority => 200;
	public override Type ObjectType => typeof(LightSaber_Launcher);
}