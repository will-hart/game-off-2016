// /** 
//  * CreepComp.cs
//  * Will Hart
//  * 20161104
// */

#region Dependencies

using System;
using GameGHJ.Common.ZenECS;
using UnityEngine;

#endregion

public class CreepComp : ComponentECS
{
    public int CreepId;
	public PositionComp AssignedHero;
    public float ConstructionCost;

    [NonSerialized] public GameObject CreepObject;

    public CreepComp() : base()
    {
    }

    

    public override ComponentTypes ComponentType => ComponentTypes.CreepComp;
}