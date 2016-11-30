// /** 
//  * ResourceProductionComp.cs
//  * Will Hart
//  * 20161104
// */

using GameGHJ.Common.ZenECS;

public class ResourceProductionComp : ComponentECS
{
    public float NextProductionTime;
    public float ProductionPeriod;
    public int ProductionQuantity;

    public int OwningSideId;
    public bool IsCapped;
    public float CapPercent;
    public float CapDistance;

    public ResourceProductionComp() : base()
    {
    }

    public override ComponentTypes ComponentType => ComponentTypes.ResourceProductionComp;
}