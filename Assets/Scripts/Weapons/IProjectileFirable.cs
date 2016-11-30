using UnityEngine;

namespace Assets.Scripts.Weapons
{
	public interface IProjectileFirable
	{
		void FireProjectile(CombatComp attacker, Transform target);
	}
}