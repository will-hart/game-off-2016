using GameGHJ.Common.ZenECS;

public class WeaponComp : ComponentECS
{
    public int baseDamage;
    public int pierceDamage;
    public float attackRange;
    public bool canHitAirUnits;
    public DamageMode DamageMode;
    public AttackType AttackType;
    public float attackSpeed;

	private float timeSinceLastAttack = 0f;

    public WeaponComp() : base()
    {
    }

	/// <summary>
	/// Manages whether a unit's attack speed cooldown is up or not
	/// TODO: This currently naively assumes if the method is called and an attack is possible
	/// that the attack will always occur and resets the timer var appropriately
	/// </summary>
	/// <param name="deltaTime">Pass in Time.deltaTime here to update the unit's time since last attack</param>
	/// <returns>true means attack is possible.</returns>
	public bool UnitAttackTimeReady(float deltaTime)
	{
		timeSinceLastAttack += deltaTime;
		if (timeSinceLastAttack >= attackSpeed)
		{
			//unit can attack
			timeSinceLastAttack = 0f;
			return true;
		}
		return false;
	}

    public override ComponentTypes ComponentType => ComponentTypes.WeaponComp;
}

public enum DamageMode
{
    Single,
    Area
}

public enum AttackType
{
    Melee,
    Ranged
}