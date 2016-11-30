// /** 
//  * AiPlanner.cs
//  * Will Hart
//  * 20161103
// */


namespace GameGHJ.Systems
{
    #region Dependencies

    using System;
    using GameGHJ.Common.Behaviours;
    using UnityEngine;

    #endregion

    public class TacticalAiMovementSystem : ZenBehaviour, IOnUpdate
    {
        /// <summary>
        ///     Search for tactical AI who have a navigation target and start their pathfinding
        /// </summary>
        public void OnUpdate()
        {
            var tacAis = ZenBehaviourManager.Instance.Get<TacticalAiStateComp>(ComponentTypes.TacticalAiStateComp);

            foreach (var ai in tacAis)
            {
                // check if we are currently path finding or just navigating
                if (ai.IsFindingPath || !ai.NavigationTargetUpdated) continue;

                // find a new path if we need to
                ai.Owner.Wrapper.gameObject.GetComponent<AiPathFindingController>().GetPath(ai);
                ai.IsFindingPath = true;
                ai.NavigationTargetUpdated = false;
            }
        }

        public override int ExecutionPriority => 100;
        public override Type ObjectType => typeof(TacticalAiPlanningSystem);
    }
}