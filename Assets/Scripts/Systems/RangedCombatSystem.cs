namespace GameGHJ.Systems
{
	#region Dependencies

	using System;
	using System.Linq;
	using Unitilities;
	using UnityEngine;

	#endregion

	public class RangedCombatSystem : ZenBehaviour, IOnUpdate
	{
		public override Type ObjectType => typeof(RangedCombatSystem);

		/// <summary>
		/// Loop through all living combat components and apply melee damage
		/// </summary>
		public void OnUpdate()
		{
			var combatComps = ZenBehaviourManager.Instance.Get<CombatComp>(ComponentTypes.CombatComp).ToList();
			combatComps.Shuffle();

			foreach (var combat in combatComps)
			{
				if (combat.selectedWeapon == null ||
					!combat.selectedWeapon.UnitAttackTimeReady(Time.deltaTime) ||
					combat.TargetedEnemy == null) continue;

			    var attacked = true;

			    if (combat.selectedWeapon.AttackType == AttackType.Ranged)
			        attacked = FireProjectile(combat, combat.TargetedEnemy.transform);

			    if (!attacked) continue;

			    PlayAttackNoise(combat);

                // if a projectile was fired, we can calculate damage
                var enemyHealth = combat.TargetedEnemy.Owner.GetComponent<HealthComp>();
			    var damage = CalculateDamage(combat.selectedWeapon, enemyHealth);
			    enemyHealth.currentHealth -= damage;
			}
		}

		private static int CalculateDamage(WeaponComp weapon, HealthComp enemy)
		{
			var maxDamage = Mathf.Max(0, weapon.baseDamage - enemy.armorValue) + weapon.pierceDamage;
			return Mathf.CeilToInt(UnityEngine.Random.Range(0.5f, 1f) * maxDamage);
		}

        /// <summary>
        /// Fires a projectile if feasible to do so
        /// </summary>
        /// <param name="attacker">That attacking component</param>
        /// <param name="target">The target transform</param>
        /// <returns>true if a projectile could be fired, false otherwise</returns>
		private static bool FireProjectile(CombatComp attacker, Transform target)
		{
			var mc = attacker.Owner.GetComponent<MissileComp>();
			var tf = attacker.Owner.GetComponent<PositionComp>();
            //var pc = mc.projectile.InstantiateFromPool<ProjectileController>(tf.position + new Vector3(0, 1, 0), tf.rotation);

		    if (tf == null || target == null) return false;
		    var range = attacker.selectedWeapon.attackRange*attacker.selectedWeapon.attackRange;
		    if ((tf.position - target.position).sqrMagnitude > range) return false;

			mc.FireProjectile(attacker, target);

		    return true;
		}

	    private static void PlayAttackNoise(CombatComp attacker)
	    {
            if (attacker.Owner.HasComponent(ComponentTypes.AudioSourceComp))
            {
                attacker.Owner.GetComponent<AudioSourceComp>().TriggerSfx(SfxTypes.GunFire);
            }
        }

		/// <summary>
		/// Make sure bullets don't immediately collide with their firing entity
		/// </summary>
		/// <param name="firer"></param>
		/// <param name="projectile"></param>
		private void IgnoreProjectileCollisions(CombatComp firer, ProjectileController projectile)
		{
			Collider pc = projectile.GetComponentDownward<Collider>();
			foreach (var col in firer.Owner.Wrapper.GetComponentsDownward<Collider>())
			{
				Physics.IgnoreCollision(pc, col);
			}
		}

		public override int ExecutionPriority => 0;
	}
}

// My thinking is you have a bunch of implementations with a common interface 
// then an enum on the weapon comp tells the system which one to use.Then 
// just store a dict of enum to instance.