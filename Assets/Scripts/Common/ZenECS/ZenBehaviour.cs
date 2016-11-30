#pragma warning disable 0414, 0219, 649, 169, 618, 1570
using System;
using UnityEngine;

public abstract class ZenBehaviour : MonoBehaviour, IZenBehaviour
{
	public abstract int ExecutionPriority { get; }
	public abstract Type ObjectType { get; }

	[HideInInspector]
	public virtual bool IsEnabled
	{
		get {return enabled;}
		set { enabled = value; }
	}

	private Guid _UniqueID;
	public Guid UniqueID => _UniqueID;
	
	public void Awake()
	{
		_UniqueID = Guid.NewGuid();
		
		//Moved to awake so it's caught properly
		IsEnabled = enabled;
		ZenBehaviourManager.Instance.RegisterZenBehaviour(this);
	}

	//BUILD: Remove this, wasted cycles just so you can debug in inspector
	public void Start()
	{
	}

	public virtual void OnDestroy()
	{
		if (ZenBehaviourManager.Instance != null)
			ZenBehaviourManager.Instance.DeregisterZenBehaviour(this);
	}


}
