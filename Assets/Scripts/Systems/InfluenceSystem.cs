// /** 
//  * InfluenceSystem.cs
//  * Will Hart
//  * 20161109
// */

namespace GameGHJ.Systems
{
    #region Dependencies

    using System;
    using System.Linq;
    using UnityEngine;

    #endregion

    /// <summary>
    ///     Updates influence maps once every second
    /// </summary>
    public class InfluenceSystem : ZenBehaviour, IOnUpdate
    {
        [SerializeField] private float _updatePeriod = 1f; // update each AI every 10 seconds
        private float _nextUpdateTime = -1;

        public override Type ObjectType => typeof(InfluenceSystem);
        public override int ExecutionPriority => 0;

        public void OnUpdate()
        {
            if (Time.time < _nextUpdateTime) return;
            _nextUpdateTime = Time.time + _updatePeriod;

            var influences = ZenBehaviourManager.Instance.Get<InfluenceComp>(ComponentTypes.InfluenceComp).ToList();
            var strategicAis =
                ZenBehaviourManager.Instance.Get<StrategicAiStateComp>(ComponentTypes.StrategicAiStateComp);

            foreach (var ai in strategicAis)
            {
                ai.DecisionSpace?.Update(influences);
            }
        }
    }
}