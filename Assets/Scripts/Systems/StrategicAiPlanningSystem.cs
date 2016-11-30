// /** 
//  * AiPlanner.cs
//  * Will Hart
//  * 20161103
// */


// #define AI_DEBUG

namespace GameGHJ.Systems
{
    #region Dependencies

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    
    using GameGHJ.AI.Actions;
    using GameGHJ.AI.Bundles;
    using GameGHJ.AI.Core;
    using GameGHJ.AI.Strategy;

    #endregion

    public class StrategicAiPlanningSystem : ZenBehaviour, IOnUpdate
    {
        private float _nextUpdateTime;
        [SerializeField] private float _updatePeriod = 5; 
        /// <summary>
        ///     Build context and periodically plan current actions
        ///     TODO is this currently being run every frame? 
        /// </summary>
        public void OnUpdate()
        {
            if (Time.time < _nextUpdateTime) return;
            _nextUpdateTime = Time.time + _updatePeriod;

#if AI_DEBUG
            var planners = ZenBehaviourManager.Instance.Get<StrategicAiStateComp>(ComponentTypes.StrategicAiStateComp).ToList();
            Debug.Log($"Planning strategic actions for {planners.Count} entities");
#else
            // leave result as IEnumerable if we aren't debugging      
            var planners = ZenBehaviourManager.Instance.Get<StrategicAiStateComp>(ComponentTypes.StrategicAiStateComp);
#endif

            foreach (var planner in planners)
            {
                if (planner.ActionContainer == null)
                {
                    planner.ActionContainer = new AiActionContainer<StrategicAiStateComp>();
                    planner.ActionContainer.AddBundle(new StrategicAiBundle());
                }

                if (planner.DecisionSpace == null)
                {
                    Debug.LogWarning("Found empty decision space. Can this branch be removed?");
                    planner.DecisionSpace = new StrategicDecisionSpace(
                        planner.FriendlyTeamId,
                        planner.EnemyTeamId,
                        200,
                        200);
                }
                
                var context = new AiContext<StrategicAiStateComp>(planner.Owner);

                if (planner.ActiveActions == null) planner.ActiveActions = new List<AbstractAiAction<StrategicAiStateComp>>();

                // run the actions and remove them if complete
                var completedActions = new List<AbstractAiAction<StrategicAiStateComp>>();

                foreach (var action in planner.ActiveActions)
                {
#if AI_DEBUG
                    Debug.Log($"STRAT updating action {action} for planner {planner.Owner.Wrapper}");
#endif 
                    action.Update(context);

                    // check if the action is complete
                    if (!action.IsComplete) continue;

                    // clean up
#if AI_DEBUG
                    Debug.Log($"STRAT exiting completed action {action} for planner {planner.Owner.Wrapper}");
#endif 
                    action.OnExit(context);
                    completedActions.Add(action);
                }

                foreach (var action in completedActions)
                {
#if AI_DEBUG
                    Debug.Log($"STRAT removing completed action {action} for planner {planner.Owner.Wrapper}");
#endif 
                    planner.ActiveActions.Remove(action);
                }
                
                // get all currently available actions
                var availableActions = planner.ActionContainer.GetAvailableActions(context);
                
                foreach (var action in availableActions)
                {
                    if (action.GetScore(context) <= 0.5f ||
                        planner.ActiveActions.Contains(action)) continue;
#if AI_DEBUG
                    Debug.Log($"STRAT entering action {action} for planner {planner.Owner.Wrapper}");
#endif 
                    planner.ActiveActions.Add(action);
                    action.OnEnter(context);
                }
            }
        }

        public override int ExecutionPriority => 101;
        public override Type ObjectType => typeof(StrategicAiPlanningSystem);
    }
}