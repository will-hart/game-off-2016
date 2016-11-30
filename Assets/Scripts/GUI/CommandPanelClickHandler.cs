#pragma warning disable 0414, 0219, 649, 169, 618, 1570
using System;
using GameGHJ.Systems;
using UnityEngine;

public class CommandPanelClickHandler : ZenBehaviour, IOnAwake
{
	public override int ExecutionPriority => 0;
	public override Type ObjectType => typeof(CommandPanelClickHandler);

	public void OnAwake()
	{

	}

	public void SetStatePosition()
	{
		Debug.Log($"set state position");
		PlayerControlSystem.Instance.SetState(CursorSelectStates.PositionPick);
	}

	public void SetStateTargeting()
	{
		Debug.Log($"set state targeting");
		PlayerControlSystem.Instance.SetState(CursorSelectStates.TargetPick);
	}

	public void SetStateNormal()
	{
		Debug.Log($"set state normal");
		PlayerControlSystem.Instance.SetState(CursorSelectStates.Normal);
	}
}
