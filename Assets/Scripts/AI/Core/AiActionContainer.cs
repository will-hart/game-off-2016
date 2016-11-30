// /** 
//  * AiPlanner.cs
//  * Will Hart
//  * 20161103
// */

//#define AI_DEBUG

namespace GameGHJ.AI.Core
{
    #region Dependencies

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

//#if UNITY_EDITOR
    using System.Collections.ObjectModel;
//#endif

    using Actions;
    using Bundles;
    using Unitilities.Tuples;
    using Random = UnityEngine.Random;

    #endregion

    /// <summary>
    /// This class represents the AI for a single entity. It is reponsible for holding 
    /// current actions and managing bundles of actions that can be added and 
    /// removed from the entity's AI planning system
    /// </summary>
    public class AiActionContainer<T> where T : AbstractAiStateComp
    {
        private const float AiPlanPeriod = 1f;

        private readonly List<AbstractAiAction<T>> _actions = new List<AbstractAiAction<T>>();

        /// <summary>
        /// Constructs an AI without any actions
        /// </summary>
        public AiActionContainer()
        {
        }

        /// <summary>
        /// Default constructor, creates an AI with the given list of actions
        /// </summary>
        /// <param name="actions"></param>
        public AiActionContainer(IEnumerable<AbstractAiAction<T>> actions)
        {
            _actions.AddRange(actions);
            _actions.Sort((a, b) => b.Priority.CompareTo(a.Priority));
        }

//#if UNITY_EDITOR
        public ReadOnlyCollection<AbstractAiAction<T>> ActionBundle => _actions.AsReadOnly();
//#endif

        /// <summary>
        /// Gets a single active action with the highest priority, typically used for tactical AI
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public AbstractAiAction<T> GetActiveAction(AiContext<T> context)
        {
#if AI_DEBUG
            Debug.Log("Running AI planning");
#endif
            var best = 0f;
            AbstractAiAction<T> bestAction = null;

            foreach (var action in _actions)
            {
                if (best > action.Priority)
                {
#if AI_DEBUG
                    Debug.Log("Breaking out of planning action - priority too low");
#endif
                    break; // cannot possibly beat the score
                }

                var newScore = action.GetScore(context);
#if AI_DEBUG
                Debug.Log($"Calculated score {newScore} for {action.GetType()}");
#endif

                if (newScore < best) continue;

                best = newScore;
                bestAction = action;
            }

            context.State.NextAiPlanTime = Time.time
                                           + AiPlanPeriod - 0.5f + Random.value;

#if AI_DEBUG
            Debug.Log(
                bestAction == null
                    ? "No action selected - bestAction is null"
                    : $"New action selected by AI Planner {bestAction.GetType()} with score {best}");
#endif

            return bestAction;
        }

        /// <summary>
        /// Gets an enumerable of possible actions, sorted by priority. Typically used for strategic AI.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public IEnumerable<AbstractAiAction<StrategicAiStateComp>> GetAvailableActions(AiContext<StrategicAiStateComp> context)
        {
            return context.State.ActionContainer.ActionBundle
                .Select(act => new Tuple<AbstractAiAction<StrategicAiStateComp>, float>(act, act.GetScore(context)))
                .OrderBy(tup => -tup.second)
                .Select(tup => tup.first)
                .ToList();
        }

        /// <summary>
        /// Adds a bundle of AI actions to the ActionContainer
        /// </summary>
        /// <param name="bundle"></param>
        public void AddBundle(AiActionBundle<T> bundle)
        {
            _actions.AddRange(bundle.Actions);
            _actions.Sort((a, b) => b.Priority.CompareTo(a.Priority));
        }

        /// <summary>
        /// Clears all actions from the ActionContainer
        /// </summary>
        private void ClearAllActions()
        {
            _actions.Clear();
        }

        /// <summary>
        /// Replaces all the existing actions with the given action bundle
        /// </summary>
        /// <param name="bundle"></param>
        public void ReplaceActions(AiActionBundle<T> bundle)
        {
            ClearAllActions();
            AddBundle(bundle);
        }
    }
}