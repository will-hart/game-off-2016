using GameGHJ.Common.ZenECS;

public class HealthComp : ComponentECS
{
    public float maxHealth;
    public float currentHealth;
    public float regenRate;
    public int armorValue;

    public bool isDead => currentHealth <= 0;

    public HealthComp() : base()
    {
    }

   public override ComponentTypes ComponentType => ComponentTypes.HealthComp;
}