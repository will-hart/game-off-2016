// /** 
//  * HasNearbyTargetsAxis.cs
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

    public class HasLowTowerToTimeRatioAxis : IAxis<StrategicAiStateComp>
    {
        private const int SecondsPerTower = 120;

        public float Score(AiContext<StrategicAiStateComp> context)
        {
            var side = context.GetComponent<SidePropertiesComp>();
            var elapsed = Time.time;
            var towers = side.TowerCount;
            var ratio = towers/elapsed;
            
            return 1 - Functions.Quartic(ratio / SecondsPerTower);
        }

        public string Name => "Has Low Tower To Time";
        public string Description => "Is closer to 1 when there have been less than a tower every 2 minutes";
    }
}