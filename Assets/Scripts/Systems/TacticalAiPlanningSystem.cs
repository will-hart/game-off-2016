// /** 
//  * AiPlanner.cs
//  * Will Hart
//  * 20161103
// */

//#define AI_DEBUG

namespace GameGHJ.Systems
{
    #region Dependencies

    using System;

#if AI_DEBUG
    using System.Linq;
#endif

    using UnityEngine;
    
    using GameGHJ.AI.Bundles;
    using GameGHJ.AI.Core;

    #endregion

    public class TacticalAiPlanningSystem : ZenBehaviour, IOnUpdate
    {
        /// <summary>
        ///     Build context and periodically plan current action
        /// </summary>
        public void OnUpdate()
        {
#if AI_DEBUG
            var planners = ZenBehaviourManager.Instance.Get<TacticalAiStateComp>(ComponentTypes.TacticalAiStateComp).ToList();
            Debug.Log($"Planning actions for {planners.Count} entities");
#else
            // leave result as IEnumerable if we aren't debugging      
            var planners = ZenBehaviourManager.Instance.Get<TacticalAiStateComp>(ComponentTypes.TacticalAiStateComp);
#endif

            foreach (var planner in planners)
            {
                if (ErrorInCreatingActionContainer(planner)) continue;

                var context = new AiContext<TacticalAiStateComp>(planner.Owner);

                // run the action
                if (planner.Action != null)
                {
                    planner.Action.Update(context);

                    // check if the action is complete
                    if (planner.Action.IsComplete)
                    {
                        planner.Action.OnExit(context);
                        planner.Action = null;
                    }
                }

                // only replan the ai if required
                if ((planner.Action != null) && (Time.time <= planner.NextAiPlanTime)) continue;

                // do not interrupt timed actions, let them run to completion
                if ((planner.Action != null) && planner.Action.IsTimed) continue;

                // if the action has changed, exit the previous action
                var newAction = planner.ActionContainer.GetActiveAction(context);
                if (newAction == planner.Action) continue;

                planner.Action?.OnExit(context);
                newAction.OnEnter(context);
                planner.Action = newAction;
            }
        }

        private static bool ErrorInCreatingActionContainer(TacticalAiStateComp planner)
        {
            if (planner.ActionContainer != null) return false;

            planner.ActionContainer = new AiActionContainer<TacticalAiStateComp>();

            if (planner.Owner.HasComponent(ComponentTypes.CreepComp))
            {
                planner.ActionContainer.AddBundle(new CreepAiBundle());
            }
            else if (planner.Owner.HasComponent(ComponentTypes.HeroComp))
            {
                planner.ActionContainer.AddBundle(new HeroAiBundle());
            }
            else if (planner.Owner.HasComponent(ComponentTypes.TowerComp))
            {
                planner.ActionContainer.AddBundle(new TowerAiBundle());
            }
            else
            {
                Debug.LogWarning(
                    $"Unable to select an appropriate AI bundle for entity {planner.Owner.EntityName}, ignoring");
                return true;
            }

            return false;
        }

        public override int ExecutionPriority => 100;
        public override Type ObjectType => typeof(TacticalAiPlanningSystem);
    }
}