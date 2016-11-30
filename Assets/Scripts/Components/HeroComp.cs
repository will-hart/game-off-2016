// /** 
//  * HeroComp.cs
//  * Will Hart
//  * 20161104
// */

#region Dependencies

using System.Collections.Generic;
using GameGHJ.Common.ZenECS;

#endregion

public class HeroComp : ComponentECS
{
    public string Name;
    public List<AbilityComp> Abilities;

    public int MaxCreeps;
    public int DeployedCreeps;

    public HeroComp() : base()
    {
    }

    

    public override ComponentTypes ComponentType => ComponentTypes.HeroComp;
}