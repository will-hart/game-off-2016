// /** 
//  * SidePropertiesComp.cs
//  * Will Hart
//  * 20161104
// */

#region Dependencies

using System;
using System.Collections.Generic;
using GameGHJ.Common.ZenECS;

#endregion

public class SidePropertiesComp : ComponentECS
{
    public bool IsPlayerControlled;
    public int SideId;
    public int Dna;
    public int SpecialDna;

    public int TowerCount;
    public int HeroLimit;
    
    public List<string> UnlockedUpgrades = new List<string>();

    [NonSerialized] public List<HeroComp> LivingHeroes = new List<HeroComp>();

    public SidePropertiesComp() : base()
    {
    }

   public override ComponentTypes ComponentType => ComponentTypes.SidePropertiesComp;
}