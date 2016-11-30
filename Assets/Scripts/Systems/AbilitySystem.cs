// /** 
//  * AbilitySystem.cs
//  * Will Hart
//  * 20161109
// */

namespace GameGHJ.Systems
{
    #region Dependencies

    using System;
    using System.Linq;
    using UnityEngine;

    #endregion

    /// <summary>
    ///     Updates influence maps once every second
    /// </summary>
    public class AbilitySystem : ZenBehaviour, IOnUpdate
    {
        public override Type ObjectType => typeof(InfluenceSystem);
        public override int ExecutionPriority => 0;

        public void OnUpdate()
        {
            var abilities = ZenBehaviourManager.Instance.Get<AbilityComp>(ComponentTypes.AbilityComp).ToList();
            
            foreach (var ability in abilities)
            {
                if (ability.IsUnlocked && ability.IsEquipped && !ability.IsApplied) ApplyAbility(ability);
                if (ability.IsApplied && !ability.IsEquipped) RemoveAbility(ability);
            }
        }

        private static void RemoveAbility(AbilityComp ability)
        {
            foreach (var buff in ability.Buffs)
            {
                ApplyBuff(ability, buff.BuffType, 1/buff.BuffValue);
            }
        }

        private static void ApplyAbility(AbilityComp ability)
        {
            foreach (var buff in ability.Buffs)
            {
                ApplyBuff(ability, buff.BuffType, buff.BuffValue);
            }
        }

        private static void ApplyBuff(ComponentECS target, AbilityBuffType type, float delta)
        {
            switch (type)
            {
                case AbilityBuffType.CombatMeleeDamage:
                    target.Owner.GetComponent<CombatComp>().MeleeDamageMultiplier *= delta;
                    break;

                case AbilityBuffType.CombatRangedDamage:
                    target.Owner.GetComponent<CombatComp>().RangedDamageMultiplier *= delta;
                    break;

                case AbilityBuffType.CreepProductionSpeed:
                    target.Owner.GetComponent<CreepProductionComp>().ProductionPeriod *= delta;
                    break;

                case AbilityBuffType.HealthMaxHealth:
                    target.Owner.GetComponent<HealthComp>().maxHealth *= delta;
                    break;

                case AbilityBuffType.HealthRegenRate:
                    target.Owner.GetComponent<HealthComp>().regenRate *= delta;
                    break;

                case AbilityBuffType.MovementSpeed:
                    target.Owner.GetComponent<MovementComp>().moveSpeed *= delta;
                    break;

                case AbilityBuffType.ResourceProductionPeriod:
                    target.Owner.GetComponent<ResourceProductionComp>().ProductionPeriod *= delta;
                    break;

                case AbilityBuffType.HeroCreepCap:
                    target.Owner.GetComponent<HeroComp>().MaxCreeps += Mathf.CeilToInt(delta);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }

    public enum AbilityBuffType
    {
        CombatMeleeDamage = 0,
        CombatRangedDamage = 1,
        CreepProductionSpeed = 10,
        HealthMaxHealth = 20,
        HealthRegenRate = 21,
        HeroCreepCap = 30,
        MovementSpeed = 40,
        ResourceProductionPeriod = 50
    }
}