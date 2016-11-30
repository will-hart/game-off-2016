using System.Collections.Generic;
using UnityEngine;

public static class LayerManager
{
	private static int _terrainMask = -50;

	public static int TerrainMask
	{
		get
		{
			if (_terrainMask < 0)
				_terrainMask = LayerMask.GetMask("Terrain");
			return _terrainMask;
		}
	}

	private static int _selectableMask = -50;

	public static int SelectableMask
	{
		get
		{
			if (_selectableMask < 0)
				_selectableMask = LayerMask.GetMask("Selectable", "SelectableObstacles");
			return _selectableMask;
		}
	}

	private static int _unitMask = -50;

	public static int UnitMask
	{
		get
		{
			if (_unitMask < 0)
				_unitMask = LayerMask.GetMask("Selectable");
			return _unitMask;
		}
	}

	private static int _buildingMask = -50;

	public static int BuildingMask
	{
		get
		{
			if (_buildingMask < 0)
				_buildingMask = LayerMask.GetMask("SelectableObstacles");
			return _buildingMask;
		}
	}

	public static Dictionary<LayerNames, string> LayerMapping = new Dictionary<LayerNames, string>
	{
		{LayerNames.Terrain, "Terrain" },
		{LayerNames.Selectable, "Selectable" },
		{LayerNames.TerrainObstacles, "TerrainObstacles" },
		{LayerNames.EnemiesInRange, "EnemiesInRange" },
		{LayerNames.SelectableObstacles, "SelectableObstacles" },
		{LayerNames.ConstructionPlacement, "ConstructionPlacement" }
	};

	public static int LayerFromName(LayerNames ln)
	{
		return LayerMask.NameToLayer(LayerManager.LayerMapping[ln]);
	}
}

public enum LayerNames
{
	Terrain,
	Selectable,
	TerrainObstacles,
	EnemiesInRange,
	SelectableObstacles,
	ConstructionPlacement
}