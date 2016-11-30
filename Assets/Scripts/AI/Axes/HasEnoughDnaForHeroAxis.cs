// /** 
//  * HasEnoughDnaForHeroAxis.cs
//  * Will Hart
//  * 20161127
// */

namespace GameGHJ.AI.Axes
{
    #region Dependencies

    using GameGHJ.AI.Core;

    #endregion

    public class HasEnoughDnaForHeroAxis : IAxis<StrategicAiStateComp>
    {
        public float Score(AiContext<StrategicAiStateComp> context)
        {
            return Functions.OneIfTrue(context.GetComponent<SidePropertiesComp>().Dna > 100);
        }

        public string Name => "Has Enough DNA For Building";
        public string Description => "Returns close to 1 when the next building can be afforded, close to 0 otherwise";
    }
}