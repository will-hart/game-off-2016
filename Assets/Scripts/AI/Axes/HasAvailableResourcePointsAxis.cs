
// /** 
//  * HasAvailableResourcePointsAxis.cs
//  * Will Hart
//  * 20161103
// */

namespace GameGHJ.AI.Axes
{
    #region Dependencies

    using System.Linq;
    using GameGHJ.AI.Core;
    using UnityEngine;

    #endregion

    public class HasAvailableResourcePointsAxis : IAxis<StrategicAiStateComp>
    {
        public float Score(AiContext<StrategicAiStateComp> context)
        {
            var rps = ZenBehaviourManager.Instance.Get<ResourceProductionComp>(ComponentTypes.ResourceProductionComp)
                .Where(rp => !rp.IsCapped);

            return Mathf.Clamp01(rps.Count() * 0.25f);
        }

        public string Name => "Has Available Resource Points";
        public string Description => "Returns 0 if there are no free resource points, otherwise returns 0.25 per free resource point";
    }
}