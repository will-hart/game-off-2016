using System;
using UnityEngine;

public class PositionComp : ComponentECS
{
    //PH, we'll want to inject this from the Entity
    [NonSerialized]public Transform transform;
	[NonSerialized]
	public Vector3 position;
	[NonSerialized]
	public Quaternion rotation;
    
    public override ComponentTypes ComponentType => ComponentTypes.PositionComp;

	
}