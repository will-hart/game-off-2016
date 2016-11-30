using UnityEngine;
using System.Collections;
using AdvancedInspector;

public class BuildingUpgradeComp : ComponentECS
{
	[Inspect] [Enum(true)] public BuildingUpgradeTypes possibleBuildingTypes;

	public override ComponentTypes ComponentType => ComponentTypes.BuildingUpgradeComp;
}
