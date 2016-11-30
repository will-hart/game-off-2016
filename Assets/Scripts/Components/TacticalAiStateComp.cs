using System;
using System.Collections.Generic;
using GameGHJ.AI.Actions;
using GameGHJ.AI.Bundles;
using GameGHJ.AI.Core;
using GameGHJ.Common.ZenECS;
using UnityEngine;

public class TacticalAiStateComp : AbstractAiStateComp
{
    public TacticalAiStance Stance;

    public Vector3 NavigationTarget;
    [NonSerialized] public bool NavigationTargetUpdated;
    
    [NonSerialized] public HealthComp AttackTarget;
	[NonSerialized] public bool AttackTargetUpdated;

    [NonSerialized] public bool IsFindingPath;
    [NonSerialized] public List<Vector3> Waypoints = new List<Vector3>();
    [NonSerialized] public int CurrentWaypoint;
    
    [NonSerialized] public AbstractAiAction<TacticalAiStateComp> Action;
    [NonSerialized] public AiActionContainer<TacticalAiStateComp> ActionContainer;
    
    public TacticalAiStateComp() : base()
    {
    }

    public override ComponentTypes ComponentType => ComponentTypes.TacticalAiStateComp;

    public enum TacticalAiStance
    {
        Offensive,
        Defensive,
        Scouting
    }

	public void SetNewAttackTarget(HealthComp newTarget)
	{
		AttackTarget = newTarget;
		AttackTargetUpdated = true;
	}

	public void SetNewMovementTarget(Vector3 newTarget)
	{
		AttackTarget = null;
		AttackTargetUpdated = true;
		NavigationTarget = newTarget;
		NavigationTargetUpdated = true;
	}
}