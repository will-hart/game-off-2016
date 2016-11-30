// /** 
//  * HasNearbyTargetsAxis.cs
//  * Will Hart
//  * 20161103
// */

namespace GameGHJ.AI.Axes
{
    #region Dependencies

    using System;
    using System.Linq;
    using GameGHJ.AI.Core;
    using UnityEngine;

    #endregion

    public class HasEnoughDnaForBuildingAxis : IAxis<StrategicAiStateComp>
    {
        public float Score(AiContext<StrategicAiStateComp> context)
        {
            if (context.State.BuildOrder.Count == 0) return 0;

            var nextBuildItem = context.State.BuildOrder.First();

            var buildingData =
                ZenBehaviourManager.Instance.GetComponent<EntityFactory>()
                    .GetTemplate(nextBuildItem == ConstructableTypes.Factory ? "BaseFactory" : "BaseTower");

            var cost = (float)buildingData
                .AsDictionary["_components"]
                .AsDictionary["BuildingComp"]
                .AsDictionary["ConstructionCost"]
                .AsInt64;

            var side = context.GetComponent<SidePropertiesComp>();
            if (Math.Abs(cost) < float.Epsilon || side.Dna < cost) return 0;

            return Functions.Logistic(side.Dna / cost - 0.55f);
        }

        public string Name => "Has Enough DNA For Building";
        public string Description => "Returns close to 1 when the next building can be afforded, close to 0 otherwise";
    }
}