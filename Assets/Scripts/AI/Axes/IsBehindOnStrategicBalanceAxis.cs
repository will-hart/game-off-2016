
// /** 
//  * IsBehindOnStrategicBalanceAxis.cs
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

    public class IsBehindOnStrategicBalanceAxis : IAxis<StrategicAiStateComp>
    {
        private float _offset;

        public IsBehindOnStrategicBalanceAxis(float offset = 0)
        {
            _offset = offset;
        }

        public float Score(AiContext<StrategicAiStateComp> context)
        {
            var bal = context.State.InfluenceBalance;

            if (bal.PositiveCount == 0) return 0;

            var ratio = bal.NegativeCount / (float)bal.PositiveCount + _offset;
            return Functions.Quadratic(ratio);
        }

        public string Name => "Is Behind On Strategic Balance";
        public string Description => "Returns 1 if the friendly side is behind on strategic balance, < 1 if ahead";
    }
}