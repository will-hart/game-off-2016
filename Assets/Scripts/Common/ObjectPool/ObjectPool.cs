#pragma warning disable 0414, 0219, 649, 169, 618, 1570
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using AdvancedInspector;
using MEC;

public static class ObjectPoolExtensions
{
	public static GameObject InstantiateFromPool(this GameObject prefab, Vector3 position, Quaternion rotation)
	{
		return ObjectPool.Instantiate(prefab, position, rotation);
	}

	public static GameObject InstantiateFromPool(this GameObject prefab)
	{
		return ObjectPool.Instantiate(prefab, Vector3.zero, Quaternion.identity);
	}

	public static T InstantiateFromPool<T>(this GameObject prefab)
		where T : class
	{
		T tComp = ObjectPool.Instantiate(prefab).GetComponent<T>();
		if (tComp == null) Debug.LogError("Object of type " + typeof(T).Name + " is not contained in prefab");
		return tComp;
	}

	public static T InstantiateFromPool<T>(this GameObject prefab, Vector3 position, Quaternion rotation)
		where T : class
	{
		T tComp = ObjectPool.Instantiate(prefab, position, rotation).GetComponent<T>();
		if (tComp == null) Debug.LogError("Object of type " + typeof(T).Name + " is not contained in prefab");
		return tComp;
	}

	public static GameObject InstantiateFromPool(this GameObject prefab, Vector3 position, Quaternion rotation, GameObject activeParent, GameObject inactiveParent)
	{
		return ObjectPool.Instantiate(prefab, position, rotation, activeParent, inactiveParent);
	}

	public static T InstantiateFromPool<T>(this GameObject prefab, Vector3 position, Quaternion rotation, GameObject activeParent, GameObject inactiveParent)
		where T : class
	{
		return ObjectPool.Instantiate(prefab, position, rotation, activeParent, inactiveParent) as T;
	}

	public static void Release(this GameObject objthis)
	{
		ObjectPool.Release(objthis);
	}

	//public static void Release<T>(this T objThis) where T: UnityEngine.Component
	//{
	//	ObjectPool.Release(objThis.gameObject);
	//}

	public static void ReleaseDelayed(this GameObject objthis, float delayTime)
	{
		ObjectPool.DelayedRelease(objthis, delayTime);
	}
}

public class ObjectPool : MonoBehaviour
{
	public GameObject ActiveParentDefault, InactiveParentDefault;
	private static ObjectPool _instance;
	public static ObjectPool instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType<ObjectPool>();
				if (_instance == null)
				{
					GameObject go = new GameObject("ObjectPool");
					_instance = go.AddComponent<ObjectPool>();

				}
			}
			return _instance;
		}
	}

	public List<Pool> customPools = new List<Pool>();

	public List<Pool> runtimePools = new List<Pool>();//Read only, just displays pools created on runtime without prior setup."

	Dictionary<GameObject, Pool> pool = new Dictionary<GameObject, Pool>();
	
	public static GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation, GameObject activeParent = null, GameObject inactiveParent = null)

	{
		return instance._Instantiate(prefab, position, rotation, activeParent, inactiveParent);
	}

	GameObject _Instantiate(GameObject prefab, Vector3 position, Quaternion rotation, GameObject activeParent = null, GameObject inactiveParent = null)
	{
		if (pool.ContainsKey(prefab))
		{
			Pool current = pool[prefab];

			//GameObject keywordActiveGameObject = new GameObject(current.Keyword);
			//keywordActiveGameObject.transform.SetParent(activeParent.transform);
			//current.activeParentGO = keywordActiveGameObject;
			//
			//GameObject keywordInactiveGameObject = new GameObject(current.Keyword);
			//keywordInactiveGameObject.transform.SetParent(inactiveParent.transform);
			//current.inactiveParentGO = keywordInactiveGameObject;

			//if (current.activeParentGO == null)


			return current.Request(position, rotation);
		}
		else // Create new runtime pool
		{
			Pool newpool = new Pool();

			

			//if (activeParent) newpool.activeParentGO = activeParent;
			//else newpool.activeParentGO = ActiveParentDefault;
			//if (inactiveParent) newpool.inactiveParentGO = inactiveParent;
			//else newpool.inactiveParentGO = InactiveParentDefault;

			GameObject keywordActiveGameObject = new GameObject(prefab.name);
			keywordActiveGameObject.transform.SetParent( (activeParent != null) ? activeParent.transform : ActiveParentDefault.transform  );
			newpool.activeParentGO = keywordActiveGameObject;

			GameObject keywordInactiveGameObject = new GameObject(prefab.name);
			keywordInactiveGameObject.transform.SetParent((inactiveParent != null) ? inactiveParent.transform : InactiveParentDefault.transform);
			newpool.inactiveParentGO = keywordInactiveGameObject;

			runtimePools.Add(newpool);
			pool.Add(prefab, newpool);
			newpool.MaxObjectsWarning = 1000;
			newpool.prefab = prefab;
			return newpool.Request(position, rotation);
		}

	}

	void Awake()
	{
		if (ActiveParentDefault == null) ActiveParentDefault = GameObject.Find("Active");
		if (InactiveParentDefault == null) InactiveParentDefault = GameObject.Find("Inactive");
		for (int i = 0; i < customPools.Count; i++)
		{
			if (customPools[i].prefab == null)
			{
				Debug.LogError("Custom object pool exists without prefab assigned to it.");
				continue;
			}
			//Set keyword if blank
			if (customPools[i].Keyword.Length == 0) customPools[i].Keyword = customPools[i].prefab.name;
			//Set custom pool's parents to the default if they don't exist
			bool addSortObject = false;
			if (customPools[i].activeParentGO == null)
			{
				customPools[i].activeParentGO = ActiveParentDefault;
				addSortObject = true;
				GameObject keywordActiveGameObject = new GameObject(customPools[i].Keyword);
				keywordActiveGameObject.transform.SetParent(customPools[i].activeParentGO.transform);
				customPools[i].activeParentGO = keywordActiveGameObject;
			}
			if (customPools[i].inactiveParentGO == null)
			{
				customPools[i].inactiveParentGO = InactiveParentDefault;
				addSortObject = true;
				GameObject keywordInactiveGameObject = new GameObject(customPools[i].Keyword);
				keywordInactiveGameObject.transform.SetParent(customPools[i].inactiveParentGO.transform);
				customPools[i].inactiveParentGO = keywordInactiveGameObject;
			}

			if (!pool.ContainsKey(customPools[i].prefab))
			{
				pool.Add(customPools[i].prefab, customPools[i]);
			}
			else
			{
				Debug.LogError("Trying to add object that is already registered by object pooler.");
			}
		}
	}

	void Start()
	{
		for (int i = 0; i < customPools.Count; i++)
		{
			customPools[i].PreloadInstances();
		}
	}

	public static void Release(GameObject obj)
	{
		instance._release(obj);
	}

	void _release(GameObject obj)
	{
		ObjectPoolID id = obj.GetComponent<ObjectPoolID>();
		if (id.Free)
		{
			return;
		}

		//Culling
		// Trigger culling if the feature is ON and the size  of the 
		//   overall pool is over the Cull Above threashold.
		//   This is triggered here because Despawn has to occur before
		//   it is worth culling anyway, and it is run fairly often.
		if (!id.Pool.cullingActive &&   // Cheap & Singleton. Only trigger once!
			id.Pool.cullDespawned &&    // Is the feature even on? Cheap too.
			id.Pool.CountTotal > id.Pool.cullAboveCount)   // Criteria met?
		{
			id.Pool.cullingActive = true;
			//StartCoroutine(id.Pool.CullDespawned());
			Timing.RunCoroutine(id.Pool.CullDespawned(), Segment.SlowUpdate);
		}

		id.Pool.Release(id);
	}

	public static void DelayedRelease(GameObject obj, float delayTime)
	{
		instance._delayedRelease(obj, delayTime);
	}

	private void _delayedRelease(GameObject obj, float delayTime)
	{
		float handle = 0;
		//Timing.RunCoroutine(_delayedReleaseCoroutine(obj, delayTime));
		Timing.CallDelayed<GameObject>(obj, delayTime,
			x =>
			{
				Timing.RunCoroutine(_delayedReleaseCoroutine(x));
			});
	}

	IEnumerator<float> _delayedReleaseCoroutine(GameObject obj)
	{
		//yield return new WaitForSeconds(delayTime);
		_release(obj);
		yield return 0f;
	}

	[Serializable]
	public class Pool //: MonoBehaviour
	{
		public override string ToString()
		{
			if (prefab == null)
				return "Empty Pool";
			return prefab.ToString();
		}
		public GameObject prefab;
		public GameObject activeParentGO;
		public GameObject inactiveParentGO;
		public string Keyword;
		public int MaxObjectsWarning = 10000;
		public uint preloadCount = 0;
		public bool cullDespawned = false;
		public uint cullAboveCount = 100;
		public float cullWaitDelay = 60;
		public int cullMaxPerPass = 1000;

		[ReadOnly]
		public bool cullingActive = false;

		[ReadOnly]
		public int CountFree;
		[ReadOnly]
		public int CountInUse;
		[ReadOnly]
		public int CountTotal => CountFree + CountInUse;
		List<ObjectPoolID> free = new List<ObjectPoolID>();
		List<ObjectPoolID> inUse = new List<ObjectPoolID>();

		GameObject temp;

		public void PreloadInstances()
		{
			if (Keyword.Length == 0) Keyword = prefab.gameObject.name;
			if (preloadCount <= 0)
			{
				return;
			}

			ObjectPoolID obj;

			for (int i = 0; i < preloadCount; i++)
			{
				temp = (GameObject)GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity);
				temp.name += CountTotal;
				obj = temp.AddComponent<ObjectPoolID>();
				free.Add(obj);
				CountFree = free.Count;

				obj.Pool = this;
				if (activeParentGO)
					obj.MyParentTransform = activeParentGO.transform;
				else
					obj.MyParentTransform = prefab.transform.parent;

				obj.transform.SetParent(obj.MyParentTransform);
				if (CountTotal > MaxObjectsWarning)
				{
					//Debug.LogError("ObjectPool: More than max objects spawned. --- " + prefab.name + " Max obj set to: " + MaxObjectsWarning + " and the pool already has: " + CountTotal);
				}

				if (!inactiveParentGO)
					obj.transform.SetParent(instance.transform);
				else
					obj.transform.SetParent(inactiveParentGO.transform);
				obj.SetFree(true);
				obj.gameObject.SetActive(false);

			}
		}

		public GameObject Request(Vector3 position, Quaternion rotation)
		{
			ObjectPoolID obj;
			if (CountFree <= 0)
			{
				temp = (GameObject)GameObject.Instantiate(prefab, position, rotation);
				temp.name += CountTotal;
				obj = temp.AddComponent<ObjectPoolID>();
				inUse.Add(obj);
				CountInUse = inUse.Count;

				obj.Pool = this;
				if (activeParentGO)
					obj.MyParentTransform = activeParentGO.transform;
				else
					obj.MyParentTransform = prefab.transform.parent;

				obj.transform.SetParent(obj.MyParentTransform);
				if (CountTotal > MaxObjectsWarning)
				{
					//Debug.LogError("ObjectPool: More than max objects spawned. --- " + prefab.name + " Max obj set to: " + MaxObjectsWarning + " and the pool already has: " + CountTotal);
				}
				obj.SetFree(false);
			}
			else
			{
				obj = free[free.Count - 1];

				free.RemoveAt(free.Count-1);
				
				inUse.Add(obj);
				obj.transform.SetParent(obj.MyParentTransform);

				obj.gameObject.transform.position = position;
				obj.gameObject.transform.rotation = rotation;
				//obj.gameObject.SetActive(true);

				CountFree = free.Count;
				CountInUse = inUse.Count;
				temp = obj.gameObject;
				temp.SetActive(true);
				obj.SetFree(false);
			}

			temp.GetComponent<IPoolInit>()?.InitFromPool();

			return temp;
		}

		public void Release(ObjectPoolID obj)
		{
			obj.GetComponent<IPoolRelease>()?.DeactivateBeforeRelease();

			inUse.Remove(obj);
			CountInUse = inUse.Count;
			free.Add(obj);

			CountFree = free.Count;

			if (!inactiveParentGO)
				obj.transform.SetParent(instance.transform);
			else
				obj.transform.SetParent(inactiveParentGO.transform);

			obj.SetFree(true);
			obj.gameObject.SetActive(false);

		}

		/// <summary>
		/// Waits for 'cullDelay' in seconds and culls the 'despawned' list if 
		/// above 'cullingAbove' amount. 
		/// 
		/// Triggered by DespawnInstance()
		/// </summary>
		public IEnumerator<float> CullDespawned()
		{
			// First time always pause, then check to see if the condition is
			//   still true before attempting to cull.
			yield return Timing.WaitForSeconds(this.cullWaitDelay);

			while (this.CountTotal > this.cullAboveCount)
			{
				// Attempt to delete an amount == this.cullMaxPerPass
				for (int i = 0; i < this.cullMaxPerPass; i++)
				{
					// Break if this.cullMaxPerPass would go past this.cullAbove
					if (this.CountTotal <= this.cullAboveCount)
						break;  // The while loop will stop as well independently

					// Destroy the last item in the list
					if (this.CountFree > 0)
					{
						ObjectPoolID inst = this.free[0];
						free.RemoveAt(0);
						Destroy(inst.gameObject);
						this.CountFree = free.Count;

					}

				}

				// Check again later
				yield return Timing.WaitForSeconds(this.cullWaitDelay);
			}

			// Reset the singleton so the feature can be used again if needed.
			this.cullingActive = false;
			yield return 0.0f;
		}
	}
}
