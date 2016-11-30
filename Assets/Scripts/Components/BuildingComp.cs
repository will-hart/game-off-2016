using GameGHJ.Common.ZenECS;

public class BuildingComp : ComponentECS
{
    public int ConstructionCost;
    public float ConstructionTime;
    public float ConstructionPercentage;
	public BuildingType buildingType;

    public BuildingComp() : base()
    {
    }

    public override ComponentTypes ComponentType => ComponentTypes.BuildingComp;
}