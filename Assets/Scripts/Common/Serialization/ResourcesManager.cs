#pragma warning disable 0414, 0219, 649, 169, 618, 1570
using System;
using System.Collections.Generic;
using UnityEngine;

public static class ResourcesManager
{
	public static Dictionary<ProjectileTypes, GameObject> ProjectileStorage;

	public static Dictionary<UnityPrefabType, GameObject> PrefabStorage;

	public static Dictionary<CreepType, string> CreepTypeMapping;

	//NGUI UIAtlas is string based, using this purely for safety & centralized location
	public static Dictionary<GuiIcons, string> GuiIconMapping;

	public static Dictionary<MaterialTypes, Material> MaterialsMapping;

	public static Dictionary<LaserTypes, GameObject> LaserTypesMapping;

	public static Dictionary<BuildingType, string> BuildingTypeMapping;

	static ResourcesManager()
	{
		ProjectileStorage = new Dictionary<ProjectileTypes, GameObject>
		{ 
			//{ProjectileTypes.Bullet, Resources.Load("Prefabs/Projectiles/Bullet") as GameObject},
			//{ProjectileTypes.Missile, Resources.Load("Prefabs/Projectiles/Missile") as GameObject} 
		};

		PrefabStorage = new Dictionary<UnityPrefabType, GameObject>
		{
			{UnityPrefabType.MeleeCreepPrefab, Resources.Load("Prefabs/Units/MeleeCreepPrefab") as GameObject },
			{UnityPrefabType.RangedCreepPrefab, Resources.Load("Prefabs/Units/RangedCreepPrefab") as GameObject },
			{UnityPrefabType.MeleeHeroPrefab, Resources.Load("Prefabs/Units/MeleeHeroPrefab") as GameObject },
			{UnityPrefabType.RangedHeroPrefab, Resources.Load("Prefabs/Units/RangedHeroPrefab") as GameObject },
			{UnityPrefabType.BuildingCortexPrefab, Resources.Load("Prefabs/Buildings/CortexPrefab") as GameObject },
            {UnityPrefabType.BuildingBaseTowerPrefab, Resources.Load("Prefabs/Buildings/ConstructionTowerPrefab") as GameObject },
			{UnityPrefabType.BuildingBaseFactoryPrefab, Resources.Load("Prefabs/Buildings/ConstructionFactoryPrefab") as GameObject },
			{UnityPrefabType.BuildingMeleeCreepPrefab, Resources.Load("Prefabs/Buildings/BuildingMeleeCreepPrefab") as GameObject },
            {UnityPrefabType.BuildingRangedCreepPrefab, Resources.Load("Prefabs/Buildings/BuildingRangedCreepPrefab") as GameObject },
            {UnityPrefabType.BuildingResourcePointPrefab, Resources.Load("Prefabs/Buildings/BuildingResourcePointPrefab") as GameObject },
			{UnityPrefabType.ConstructionFactoryPrefab, Resources.Load("Prefabs/Buildings/ConstructionFactoryPrefab") as GameObject },
			{UnityPrefabType.ConstructionTowerPrefab, Resources.Load("Prefabs/Buildings/ConstructionTowerPrefab") as GameObject },
			{UnityPrefabType.BuildingLaserTowerPrefab, Resources.Load("Prefabs/Buildings/LaserTowerPrefab") as GameObject },
			{UnityPrefabType.BuildingMissileTowerPrefab, Resources.Load("Prefabs/Buildings/MissileTowerPrefab") as GameObject }
        };

		CreepTypeMapping = new Dictionary<CreepType, string>
		{
			{CreepType.MeleeCreep1, "MeleeCreep1" },
			{CreepType.MeleeCreep2, "MeleeCreep2" },
			{CreepType.RangedCreep1, "RangedCreep1" },
			{CreepType.RangedCreep2, "RangedCreep2" }
		};

		GuiIconMapping = new Dictionary<GuiIcons, string>
		{
			{GuiIcons.HeroMeleeIcon, "HeroMeleeIcon"},
			{GuiIcons.HeroRangedIcon, "HeroRangedIcon" },
			{GuiIcons.CreepMeleeIcon, "CreepMeleeIcon"},
			{GuiIcons.CreepRangedIcon, "CreepRangedIcon"},
			{GuiIcons.BuildingRangedIcon, "BuildingRangedIcon"},
			{GuiIcons.BuildingMeleeIcon, "BuildingMeleeIcon" },
			{GuiIcons.TowerBaseIcon, "BuildTowerIcon" },
			{GuiIcons.TowerLaserIcon, "TowerLaserIcon" },
			{GuiIcons.TowerMissileIcon, "TowerMissileIcon" }
		};

		MaterialsMapping = new Dictionary<MaterialTypes, Material>
		{
			{MaterialTypes.Arc, Resources.Load("Materials/Weapons/Lasers/Arc", typeof(Material)) as Material },
			{MaterialTypes.ThinArc, Resources.Load("Materials/Weapons/Lasers/ThinArc", typeof(Material)) as Material }
		};

		LaserTypesMapping = new Dictionary<LaserTypes, GameObject>
		{
			{LaserTypes.LightningBig, Resources.Load("Prefabs/Projectiles/Rays/LightningRay_big") as GameObject},
			{LaserTypes.Fire, Resources.Load("Prefabs/Projectiles/Rays/FireRay_spiralZen") as GameObject},
			{LaserTypes.Plasma, Resources.Load("Prefabs/Projectiles/Rays/PlasmaRay") as GameObject}
		};

		BuildingTypeMapping = new Dictionary<BuildingType, string>
		{
			{BuildingType.RangedCreepBuilding, "BuildingRangedCreep1" },
			{BuildingType.LaserTower, "BuildingTowerLaser" }
		};
	}
}

public enum UnityPrefabType
{
	BuildingBaseFactoryPrefab,
	BuildingMeleeCreepPrefab,
    BuildingRangedCreepPrefab,
    BuildingBaseTowerPrefab,
	BuildingLaserTowerPrefab,
	BuildingMissileTowerPrefab,
	BuildingCortexPrefab,
    BuildingResourcePointPrefab,

	ConstructionFactoryPrefab,
	ConstructionTowerPrefab,
	
	MeleeCreepPrefab,
	RangedCreepPrefab,
	MeleeHeroPrefab,
    RangedHeroPrefab
}

public enum CreepType
{
	RangedCreep1,
	RangedCreep2,
	MeleeCreep1,
	MeleeCreep2
}

public enum BuildingType
{
	Cortex,
	RangedCreepBuilding,
	MeleeCreepBuilding,
	LaserTower, //Placeholder ideas
	MissileTower,
	BaseFactory,
	BaseTower
}

public enum GuiIcons
{
	HeroMeleeIcon,
	HeroRangedIcon,
	CreepMeleeIcon,
	CreepRangedIcon,
	FactoryBaseIcon,
	BuildingRangedIcon,
	BuildingMeleeIcon,
	TowerBaseIcon,
	TowerLaserIcon,
	TowerMissileIcon,
	ArmorAbilityIcon,
	AttachHeroIcon,
	AttackIcon,
	BuildFactoryIcon,
	BuildHeroIcon
}

public enum MaterialTypes
{
	Arc,
	ThinArc
}

public enum LaserTypes
{
	LightningBig,
	Fire,
	Plasma
}

[Flags]
public enum BuildingUpgradeTypes
{
	RangedCreepProduction = 1 << 0,
	MeleeCreepProduction = 1 << 1,
	LaserTower = 1 << 2,
	MissileTower = 1 << 3
}