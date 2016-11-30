using System;
using System.Collections.Generic;
using FullInspector;
using GameGHJ.Common.ZenECS;

public class CombatComp : ComponentECS
{
    [NonSerialized, ShowInInspector] public WeaponComp selectedWeapon;
    [NonSerialized, ShowInInspector] public PositionComp TargetedEnemy;

    public float MeleeDamageMultiplier = 1;
    public float RangedDamageMultiplier = 1;

    public List<PositionComp> EnemiesInRange = new List<PositionComp>();
    
    public CombatComp() : base()
    {
    }
    
    public override ComponentTypes ComponentType => ComponentTypes.CombatComp;
}