// /** 
//  * HasEnoughDnaForBuildingUpgradeAxis.cs
//  * Will Hart
//  * 20161127
// */

namespace GameGHJ.AI.Axes
{
    #region Dependencies

    using GameGHJ.AI.Core;

    #endregion

    public class HasEnoughDnaForBuildingUpgradeAxis : IAxis<StrategicAiStateComp>
    {
        public float Score(AiContext<StrategicAiStateComp> context)
        {
            return Functions.OneIfTrue(context.GetComponent<SidePropertiesComp>().Dna > 50);
        }

        public string Name => "Has Enough DNA For Building Upgrade";
        public string Description => "Returns close to 1 when the next building upgrade can be afforded, close to 0 otherwise";
    }
}