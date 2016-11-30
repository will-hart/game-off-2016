// /** 
//  * StrategicPosturePlanningSystem.cs
//  * Will Hart
//  * 20161104
// */

namespace GameGHJ.Systems
{
    #region Dependencies

    using System;
    using System.Collections.Generic;
    using GameGHJ.AI.Bundles;
    using GameGHJ.AI.Core;
    using UnityEngine;

    using GameGHJ.AI.Influence;
    using GameGHJ.AI.Strategy;
    using System.Linq;
    #endregion

    /// <summary>
    ///     A system to determine strategic posture for the AIs in the game.
    ///     This system will loop through all AIs it finds, with each AI having a refresh
    ///     rate defined by the UpdatePeriod constant.
    /// </summary>
    public class StrategicPosturePlanningSystem : ZenBehaviour, IOnUpdate
    {
        // TODO make this scale for the number of AIs, so each AI is processed every N seconds
        private const float UpdatePeriod = 5; 
        private float _nextUpdateTime = -1;

        private readonly List<StrategicAiStateComp> _ais = new List<StrategicAiStateComp>();
        private int _currentAiIndex;

        public override Type ObjectType => typeof(StrategicPosturePlanningSystem);

        public void OnUpdate()
        {
            if (_ais.Count == 0)
            {
                _ais.AddRange(ZenBehaviourManager.Instance.Get<StrategicAiStateComp>(ComponentTypes.StrategicAiStateComp));
            }

            // check if we have an AI and its time to update it
            if ((_ais.Count == 0) || (Time.time < _nextUpdateTime)) return;

            // check if influence maps are being processed, if they are, try again next frame
            if (UpdatePosture(_ais[_currentAiIndex]))
            {
                _nextUpdateTime = Time.time + UpdatePeriod;
                ++_currentAiIndex;
            }
            else
            {
                return;
            }

            if (_currentAiIndex != _ais.Count) return;

            // force requery of strategic AIs in case AIs have been added or removed
            _currentAiIndex = 0;
            _ais.Clear();
        }

        public override int ExecutionPriority => 0;

        /// <summary>
        ///     A very basic method for determining posture.
        ///     TODO the AI knows everything here, how do we bring limited vision in to the decision making?
        /// </summary>
        private static bool UpdatePosture(StrategicAiStateComp ai)
        {
            if (ai.DecisionSpace == null)
            {
                Debug.LogWarning("Unable to determine posture for strategic AI, no decision space available");
                return false;
            }

            var context = new AiContext<StrategicAiStateComp>(ai.Owner);

            if (ErrorInCreatingActionContainer(ai)) return true;

            // populate AI with current state
            ai.InfluenceBalance = ai.DecisionSpace.Influence.GetBalance();
            var side = context.GetComponent<SidePropertiesComp>();
            side.LivingHeroes = ZenBehaviourManager.Instance.Get<HeroComp>(ComponentTypes.HeroComp)
                .Where(hc => !hc.Owner.GetComponent<HealthComp>().isDead && hc.Owner.GetComponent<UnitPropertiesComp>().teamID == side.SideId)
                .ToList();

            if (ai.PostureAction != null)
            {
                ai.PostureAction.Update(context);

                // check if the action is complete
                if (ai.PostureAction.IsComplete)
                {
                    ai.PostureAction.OnExit(context);
                    ai.PostureAction = null;
                }
            }

            var newAction = ai.PostureActionContainer.GetActiveAction(context);
            if (newAction == ai.PostureAction) return true;

            ai.PostureAction?.OnExit(context);
            newAction.OnEnter(context);
            ai.PostureAction = newAction;

            return true;
        }

        private static bool ErrorInCreatingActionContainer(StrategicAiStateComp planner)
        {
            if (planner.PostureActionContainer != null) return false;
            planner.PostureActionContainer = new AiActionContainer<StrategicAiStateComp>();
            planner.PostureActionContainer.AddBundle(new StrategicAiDefaultPostureBundle());
            return false;
        }
    }
}