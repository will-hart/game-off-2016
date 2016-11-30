#pragma warning disable 0414, 0219, 649, 169, 618, 1570
using UnityEngine;
using System.Collections;

public class ObjectPoolID : MonoBehaviour
{
	public Transform MyParentTransform;
	public ObjectPool.Pool Pool { get; set; }
	public bool Free => isFree;
	bool isFree { get; set; }
	public int thisID => GetInstanceID();

	public int prefabID
	{
		get { if (MyParentTransform == null) return 0; return MyParentTransform.gameObject.GetInstanceID(); }
	}

	public void SetFree(bool state)
	{
		isFree = state;
	}

	public bool GetFree()
	{
		return isFree;
	}
}