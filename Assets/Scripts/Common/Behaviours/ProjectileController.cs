#pragma warning disable 0414, 0219, 649, 169, 618, 1570
using System;
using UnityEngine;

public class ProjectileController : ZenBehaviour, IOnUpdate
{
	public override int ExecutionPriority => 0;
	public override System.Type ObjectType => typeof(ProjectileController);

	public Transform target;

	private float TimeToLive = 5f;
	
	public void OnUpdate()
	{
		TimeToLive -= Time.deltaTime;
		if (target == null || TimeToLive < 0f)
		{
			this.gameObject.Release();
			return;
		}

		transform.LookAt(target);
		transform.Translate(transform.forward * 0.2f);
	}

	void OnCollisionEnter(Collision other)
	{
		gameObject.Release();
	}
}

