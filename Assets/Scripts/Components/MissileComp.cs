// /** 
//  * MissileComp.cs
//  * Will Hart
//  * 20161104
// */

#region Dependencies

using System;
using Assets.Scripts.Weapons;
using FullSerializer;
using GameGHJ.Common.ZenECS;
using UnityEngine;

#endregion

public class MissileComp : ComponentECS, IProjectileFirable
{

	public ProjectileTypes ProjectileType;
	
	[NonSerialized] public IProjectileFirable projectile;
    [NonSerialized] public WeaponComp Weapon;

    public MissileComp() : base()
    {
	   
    }

	public override void Initialise()
	{
	}

	public void InitializeGameObject()
	{
		if (ProjectileType == ProjectileTypes.Laser)
		{
			projectile = Owner.Wrapper.gameObject.AddComponent<LightSaber_Launcher>();
		}
		else if (ProjectileType == ProjectileTypes.LineRender)
			projectile = Owner.Wrapper.gameObject.AddComponent<LineRenderedProjectile>();
	}

	public override ComponentTypes ComponentType => ComponentTypes.MissileComp;
	public void FireProjectile(CombatComp attacker, Transform target)
	{
		projectile.FireProjectile(attacker, target);
	}
}

public enum ProjectileTypes
{
	Laser,
	LineRender
}