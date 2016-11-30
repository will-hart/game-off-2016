
// /** 
//  * IsAheadOnStrategicBalanceAxis.cs
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

    public class IsAheadOnStrategicBalanceAxis : IAxis<StrategicAiStateComp>
    {
        private float _offset;

        public IsAheadOnStrategicBalanceAxis(float offset = 0)
        {
            _offset = offset;
        }

        public float Score(AiContext<StrategicAiStateComp> context)
        {
            var bal = context.State.InfluenceBalance;

            if (bal.NegativeCount == 0) return 0;

            var ratio = bal.PositiveCount / (float) bal.NegativeCount - _offset;
            return Functions.Quadratic(ratio);
        }

        public string Name => "Is Ahead On Strategic Balance";
        public string Description => "Returns 1 if the friendly side is ahead on strategic balance, < 1 if behind";
    }
}