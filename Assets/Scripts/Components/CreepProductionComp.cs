// /** 
//  * CreepProductionComp.cs
//  * Will Hart
//  * 20161104
// */

#region Dependencies

using GameGHJ.Common.ZenECS;
using UnityEngine;

#endregion

public class CreepProductionComp : ComponentECS
{
    public int CreepId;
	public CreepType CreepTypeToProduce;
    public float NextProductionTime;
    public float ProductionPeriod;
    public HeroComp AssignedHero;
    public Vector3 SpawnOffset;

    public override ComponentTypes ComponentType => ComponentTypes.CreepProductionComp;

   
}