using System;
using System.Collections.Generic;
using Assets.Scripts.Weapons;
using MEC;
using UnityEngine;

public class LineRenderedProjectile : ZenBehaviour, IOnStart, IProjectileFirable
{
	public override int ExecutionPriority => 190;
	public override Type ObjectType => typeof(LineRenderedProjectile);

	LineRenderer line;

	public int laserDistance = 10;
	public float laserSpeed = 2;
	public int laserDamage = 10;
	public int laserForce = 1;
	public float laserForceMult = 0.1f;

	public void OnStart()
	{
		line = gameObject.AddComponent<LineRenderer>();
		var mat = ResourcesManager.MaterialsMapping[MaterialTypes.ThinArc];
		line.material = mat;
		line.enabled = false;
	}

	public void FireProjectile(CombatComp attacker, Transform target)
	{
		if (target == null) return;
		line.enabled = true;
		line.useWorldSpace = true;
		line.SetPosition(0, attacker.Owner.GetComponent<PositionComp>().position);
		line.SetPosition(1, target.position);

		Timing.KillCoroutines(DisableLaser());
		Timing.RunCoroutine(DisableLaser());
	}

	IEnumerator<float> DisableLaser()
	{
		yield return Timing.WaitForSeconds(0.5f);
		if (line) line.enabled = false;
	}

	public void OnDisable()
	{
		Timing.KillCoroutines(DisableLaser());
	}

	public override void OnDestroy()
	{
		Timing.KillCoroutines(DisableLaser());
		base.OnDestroy();
	}
}