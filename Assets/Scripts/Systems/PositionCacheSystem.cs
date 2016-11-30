// /** 
//  * BuildingProductionSystem.cs
//  * Will Hart
//  * 20161104
// */

namespace GameGHJ.Systems
{
    #region Dependencies

    using System;
    using UnityEngine;

    #endregion

    /// <summary>
    /// Periodically produces creeps from production buildings
    /// </summary>
    public class PositionCacheSystem : ZenBehaviour, IOnUpdate
    {
        public override Type ObjectType  => typeof(PositionCacheSystem);
        public override int ExecutionPriority => -100;

        public void OnUpdate()
        {
            var positions = ZenBehaviourManager.Instance.Get<PositionComp>(ComponentTypes.PositionComp);

            foreach (var position in positions)
            {
                position.position = position.transform.position;
                position.rotation = position.transform.rotation;
            }
        }
    }
}