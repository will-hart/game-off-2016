
// /** 
//  * EmptyResourcePointsAreSafeAxis.cs
//  * Will Hart
//  * 20161119
// */

namespace GameGHJ.AI.Axes
{
    #region Dependencies

    using System.Linq;
    using GameGHJ.AI.Core;
    using UnityEngine;

    #endregion

    public class EmptyResourcePointsAreSafeAxis : IAxis<StrategicAiStateComp>
    {
        public float Score(AiContext<StrategicAiStateComp> context)
        {
            var avg = ZenBehaviourManager.Instance.Get<ResourceProductionComp>(ComponentTypes.ResourceProductionComp)
                .Where(rp => !rp.IsCapped)
                .Select(rp => context.State.DecisionSpace.Influence.GetValue(rp.Owner.GetComponent<PositionComp>().position))
                .DefaultIfEmpty(0)
                .Average();

            return Mathf.Clamp01((float)avg / 10 + 0.5f);
        }

        public string Name => "Empty Resource Points Are Safe";
        public string Description => "Returns 1 if all empty resource points are very safe or neutral, 0 if very unsafe";
    }
}