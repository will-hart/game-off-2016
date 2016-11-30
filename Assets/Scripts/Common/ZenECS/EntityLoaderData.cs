// /** 
//  * EntityLoaderData.cs
//  * Will Hart
//  * 20161105
// */

namespace GameGHJ.Common.ZenECS
{
    #region Dependencies

    using System;
    using System.Collections.Generic;
    using UnityEngine;

    #endregion

    [Serializable]
    public class EntityLoaderData
    {
        public int Id;

        public int Creep_CreepId;

        public int CreepProduction_CreepId;
        public float CreepProduction_ProductionPeriod = 10f;

        public int Health_ArmorValue = 5;
        public float Health_CurrentHealth = 10f;
        public float Health_MaxHealth = 10f;
        public float Health_RegenRate = 0.1f;

        public int Hero_HeroId;
        public string Hero_Name;

        public float Influence_InfluenceFalloffRate = 0.1f;
        public int Influence_InfluenceStrength = 1;
        public float Influence_MaxInfluenceRange = 10f;

        public MovementType Movement_MovementType = MovementType.Ground;

        public float Movement_MoveSpeed = 5f;

        public Vector3 Position_Position = Vector3.zero;
        public Quaternion Position_Rotation = Quaternion.identity;

        public float ResourceProduction_ProductionPeriod = 1;
        public int ResourceProduction_ProductionQuantity = 1;
        public bool SideProperties_IsPlayerControlled = false;

        public int SideProperties_SideId;

        public int UnitProperties_SideId;
        
        public DamageMode WeaponDamageMode = DamageMode.Single;
        public float Weapon_AttackRange = 5;
        public float Weapon_AttackSpeed = 5;
        public int Weapon_BaseDamage = 5;
        public int Weapon_PierceDamage = 5;
        public bool Weapon_CanHitAirUnits = false;

        public List<ComponentTypes> Components;

        public EntityLoaderData(List<ComponentTypes> components)
        {
            Components = components;
        }
    }
}