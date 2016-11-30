using GameGHJ.Common.ZenECS;

public class InfluenceComp : ComponentECS
{
    public int InfluenceStrength;
    public int ScoutVisionRange;

    // TBD

    public InfluenceComp() : base()
    {
    }

   

    public override ComponentTypes ComponentType => ComponentTypes.InfluenceComp;
}