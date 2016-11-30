#pragma warning disable 0414, 0219, 649, 169, 618, 1570
using System;
using System.Collections;
using System.Collections.Generic;
using MEC;
using GameGHJ.Systems;
using UnityEngine;

public class AbilityPanelClickHandler : ZenBehaviour, IOnStart
{
	public override int ExecutionPriority => 0;
	public void OnStart()
	{
		Timing.Instance.TimeBetweenSlowUpdateCalls = 1f;
	}

	public override Type ObjectType => typeof(AbilityPanelClickHandler);

	public void TriggerStrAbility()
	{
		Debug.Log("Triggering str ability");
		var hero = PlayerControlSystem.Instance.selectedObjects[0];
		var cc = hero.Owner.GetComponent<CombatComp>();
		cc.MeleeDamageMultiplier = 2.0f;
		cc.RangedDamageMultiplier = 2.0f;
		Timing.RunCoroutine(RemoveStrBuff(cc), Segment.SlowUpdate);
	}

	public void TriggerArmorAbility()
	{
		Debug.Log("Triggering armor ability");
		var hero = PlayerControlSystem.Instance.selectedObjects[0];
		var hc = hero.Owner.GetComponent<HealthComp>();
		hc.armorValue *= 2;
		Timing.RunCoroutine(RemoveArmorBuff(hc), Segment.SlowUpdate);
	}

	IEnumerator<float> RemoveStrBuff(CombatComp cc)
	{
		yield return Timing.WaitForSeconds(10.0f);
		cc.MeleeDamageMultiplier = 1.0f;
		cc.RangedDamageMultiplier = 1.0f;
	}

	IEnumerator<float> RemoveArmorBuff(HealthComp hc)
	{
		yield return Timing.WaitForSeconds(15.0f);
		hc.armorValue /= 2;
	}
}
