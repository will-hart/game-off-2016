#pragma warning disable 0414, 0219, 649, 169, 1570
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public enum Tags : long
{
	None = 0,
	Player = 1 << 0,
	NPC = 1 << 1,
	Eclipse = 1 << 2,
	Foreground = 1 << 3,
	Middleground = 1 << 4,
	Background = 1 << 5,
	Weapons = 1 << 6,
	Lighting = 1 << 7,
	Laser = 1 << 8,
	Missile = 1 << 9,
	Enemy = 1 << 10,
	Ally = 1 << 11,
	Station = 1 << 12,
	Planet = 1 << 13,
	Star = 1 << 14,
	PlayerRangeTrigger = 1 << 15,
	DisableDistanceTrigger = 1 << 16,
	Ship = 1 << 17
}

public static class ExtensionMethodsZen
{
	public static T GetComponentDownward<T>(this UnityEngine.Component gameObject)
		where T : UnityEngine.Component
	{
		T comp = gameObject.GetComponent<T>();
		if (comp == null)
			comp = gameObject.GetComponentInChildren<T>();

		return comp;
	}

	public static T GetComponentDownward<T>(this GameObject gameObject)
		where T : UnityEngine.Component
	{
		T comp = gameObject.GetComponent<T>();
		if (comp == null)
			comp = gameObject.GetComponentInChildren<T>();

		return comp;
	}

	public static T GetComponentDownThenUp<T>(this UnityEngine.Component gameObject)
		where T : UnityEngine.Component
	{
		T comp = gameObject.GetComponentDownward<T>();
		if (comp == null)
			comp = gameObject.GetComponentUpward<T>();

		return comp;
	}

	public static T GetComponentDownThenUp<T>(this GameObject gameObject)
		where T : UnityEngine.Component
	{
		T comp = gameObject.GetComponentDownward<T>();
		if (comp == null)
			comp = gameObject.GetComponentUpward<T>();

		return comp;
	}

	public static T GetComponentUpward<T>(this UnityEngine.Component gameObject)
		where T : UnityEngine.Component
	{
		T result = gameObject.GetComponent<T>();
		if (result != null) return result;

		for (Transform t = gameObject.transform; t != null; t = t.parent)
		{
			result = t.GetComponent<T>();
			if (result != null)
				return result;
		}

		return null;
	}
	
	public static T GetComponentUpward<T>(this GameObject gameObject)
		where T : UnityEngine.Component
	{
		T result = gameObject.GetComponent<T>();
		if (result != null) return result;

		for (Transform t = gameObject.transform; t != null; t = t.parent)
		{
			result = t.GetComponent<T>();
			if (result != null)
				return result;
		}

		return null;
	}

	public static T[] GetComponentsInParentsAsArray<T>(this UnityEngine.Component gameObject)
		where T : UnityEngine.Component
	{
		List<T> results = new List<T>();
		for (Transform t = gameObject.transform; t != null; t = t.parent)
		{
			T result = t.GetComponent<T>();
			if (result != null)
				results.Add(result);
		}

		return results.ToArray();
	}

	public static List<T> GetComponentsInParents<T>(this UnityEngine.Component gameObject)
		where T : UnityEngine.Component
	{
		List<T> results = new List<T>();
		for (Transform t = gameObject.transform; t != null; t = t.parent)
		{
			T result = t.GetComponent<T>();
			if (result != null)
				results.Add(result);
		}

		return results;
	}

	public static List<T> GetComponentsDownward<T>(this UnityEngine.Component gameObject)
		where T : UnityEngine.Component
	{
		List<T> results = new List<T>();
		results.AddRange( gameObject.GetComponents<T>().ToList() );
		results.AddRange(gameObject.GetComponentsInChildren<T>().ToList());

		return results;
	}

	public static T[] GetComponentsDownwardAsArray<T>(this UnityEngine.Component gameObject)
		where T : UnityEngine.Component
	{
		List<T> results = new List<T>();
		results.AddRange(gameObject.GetComponents<T>().ToList());
		results.AddRange(gameObject.GetComponentsInChildren<T>().ToList());

		return results.ToArray();
	}

	public static void SetAllActiveRecursively(this GameObject rootObject, bool active)
	{
		rootObject.SetActive(active);

		foreach (Transform childTransform in rootObject.transform)
		{
			SetAllActiveRecursively(childTransform.gameObject, active);
		}
	}

	public static void SetChildrenLayerRecursively(this GameObject rootObject)
	{
		int newLayer = rootObject.layer;
		foreach (Transform t in rootObject.transform)
			t.gameObject.layer = newLayer;
	}


	public static T[] RemoveAt<T>(this T[] source, int index)
	{
		T[] dest = new T[source.Length - 1];
		int i = 0, j = 0;

		while (i < source.Length)
		{
			if (i != index)
			{
				dest[j] = source[i];
				j++;
			}
			i++;
		}

		return dest;
	}

	/// <summary>
	/// Gets or add a component. Usage example:
	/// BoxCollider boxCollider = transform.GetOrAddComponent<BoxCollider>();
	/// </summary>
	public static T GetOrAddComponent<T>(this UnityEngine.Component child) where T : UnityEngine.Component
	{
		T result = child.GetComponent<T>();
		if (result == null)
		{
			result = child.gameObject.AddComponent<T>();
		}
		return result;
		
	}

	public static RaycastHit2D[] FilterObjects(this RaycastHit2D[] hits, params GameObject[] objsToFilter)
	{
		List<RaycastHit2D> filtered = new List<RaycastHit2D>(hits.Length);
		for (int j = 0; j < hits.Length; j++)
		{
			bool shouldFilter = false;
			for (int k = 0; k < objsToFilter.Length; k++)
			{
				if (hits[j] && hits[j].transform.gameObject == objsToFilter[k]) 
				{
					shouldFilter = true;
				}
			}

			if (!shouldFilter) filtered.Add(hits[j]);
		}

		return filtered.ToArray();
	}

	public static bool HasFlag(this Enum e, Enum flag)
	{
		return (Convert.ToInt64(e) & Convert.ToInt64(flag)) != 0;
	}

}

