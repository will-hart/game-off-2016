using GameGHJ.Common.ZenECS;

public class UnitPropertiesComp : ComponentECS
{
    public int teamID;
    
    public UnitPropertiesComp() : base()
    {
    }
    
    public override ComponentTypes ComponentType => ComponentTypes.UnitPropertiesComp;
}