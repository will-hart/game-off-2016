using System;
using System.Collections.Generic;
using GameGHJ.Common.ZenECS;

public class UnityPrefabComp : ComponentECS
{
	public UnityPrefabType prefabType;
	
	public UnityPrefabComp() : base()
	{
	}

	
	public override ComponentTypes ComponentType => ComponentTypes.UnityPrefabComp;
}