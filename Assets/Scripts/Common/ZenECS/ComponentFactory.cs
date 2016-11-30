using System;
using System.Collections.Generic;
using System.ComponentModel;
using GameGHJ.Common.ZenECS;

/// FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF I Can't believe there is no less-ugly way to do this
public static class ComponentFactory
{
    public static readonly Dictionary<ComponentTypes, Type> ComponentLookup = new Dictionary<ComponentTypes, Type>
    {
        {ComponentTypes.AbilityComp, typeof(AbilityComp)},
        {ComponentTypes.AvailableWeaponsComp, typeof(AvailableWeaponsComp)},
        {ComponentTypes.BuildingComp, typeof(BuildingComp)},
        {ComponentTypes.CombatComp, typeof(CombatComp)},
        {ComponentTypes.HealthComp, typeof(HealthComp)},
        {ComponentTypes.MovementComp, typeof(MovementComp)},
        {ComponentTypes.PositionComp, typeof(PositionComp)},
        {ComponentTypes.UnitPropertiesComp, typeof(UnitPropertiesComp)},
        {ComponentTypes.WeaponComp, typeof(WeaponComp)},
        {ComponentTypes.HeroComp, typeof(HeroComp) },
        {ComponentTypes.CreepComp, typeof(CreepComp) },
        {ComponentTypes.CreepProductionComp, typeof(CreepProductionComp) },
        {ComponentTypes.InfluenceComp, typeof(InfluenceComp) },
        {ComponentTypes.MissileComp, typeof(MissileComp) },
        {ComponentTypes.ResourceProductionComp, typeof(ResourceProductionComp) },
        {ComponentTypes.SidePropertiesComp, typeof(SidePropertiesComp) },
        {ComponentTypes.StrategicAiStateComp, typeof(StrategicAiStateComp) },
        {ComponentTypes.TacticalAiStateComp, typeof(TacticalAiStateComp) },
        {ComponentTypes.SelectableUnitComp, typeof(SelectableUnitComp) },
		{ComponentTypes.UnityPrefabComp, typeof(UnityPrefabComp) },
        {ComponentTypes.AudioSourceComp, typeof(AudioSourceComp) },
        {ComponentTypes.TowerComp, typeof(TowerComp) },
		{ComponentTypes.BuildingUpgradeComp, typeof(BuildingUpgradeComp) }

    };

    public static ComponentECS Create(ComponentTypes type, Entity entity, EntityLoaderData data)
    {
        if (!ComponentLookup.ContainsKey(type)) return null;
        return (ComponentECS)Activator.CreateInstance(ComponentLookup[type], entity, data);
    }
}

public enum ComponentTypes
{
    AbilityComp = 0,
    AvailableWeaponsComp = 1,
    CombatComp = 2,
    HealthComp = 3,
    MovementComp = 4,
    PositionComp = 5,
    UnitPropertiesComp = 6,
    WeaponComp = 7,
    HeroComp = 8,
    CreepComp = 9,
    CreepProductionComp = 10,
    InfluenceComp = 11,
    MissileComp = 12,
    ResourceProductionComp = 13,
    SidePropertiesComp = 14,
    
    SelectableUnitComp = 16,
	StrategicAiStateComp = 17,
    TacticalAiStateComp = 18,
    BuildingComp = 19,
	UnityPrefabComp = 20,
	AudioSourceComp = 21,
    TowerComp = 22,
	BuildingUpgradeComp = 23
}