using System.Collections.Generic;
using GameGHJ.Systems;

public class AbilityComp : ComponentECS
{
    public float CooldownTime;
    public int UpgradeCost;
    public bool IsUnlocked;
    public bool IsApplied;
    public bool IsEquipped;
    public string Description;
    public List<AbilityBuff> Buffs = new List<AbilityBuff>();

    public override ComponentTypes ComponentType => ComponentTypes.AbilityComp;

}

public struct AbilityBuff
{
    public AbilityBuffType BuffType;
    public float BuffValue;
    public string Description;
}