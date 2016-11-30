// /** 
//  * AbstractAiAction.cs
//  * Will Hart
//  * 20161103
// */

// #define AI_DEBUG

namespace GameGHJ.AI.Actions
{
    #region Dependencies

    using System;
    using System.Collections.Generic;
#if UNITY_EDITOR
    using System.Collections.ObjectModel;
#endif
    using UnityEngine;

    using Axes;
    using Core;

    #endregion

    public abstract class AbstractAiAction<T> where T : AbstractAiStateComp
    {
        private readonly List<IAxis<T>> _axes = new List<IAxis<T>>();
        private readonly int _numAxes;
        private float _endTime;
        private bool _isStarted;
        private float _percentComplete;

        protected float TimedDuration = -1;

        protected AbstractAiAction(IEnumerable<IAxis<T>> axes)
        {
            _axes.AddRange(axes);
            _numAxes = _axes.Count;
        }

        public bool IsComplete { get; protected set; }
        public abstract int Priority { get; }

        public float PercentComplete => IsTimed ? _percentComplete : 1;

        public bool IsTimed => TimedDuration > 0;

#if UNITY_EDITOR
        public ReadOnlyCollection<IAxis<T>> Axes => _axes.AsReadOnly();
#endif

        public virtual float GetScore(AiContext<T> context)
        {
            if (_numAxes == 0) return 0;

            var score = 1f;

            foreach (var ax in _axes)
            {
                var axScore = ax.Score(context);

                if (Math.Abs(axScore) < float.Epsilon)
                {
#if AI_DEBUG
                    Debug.Log("Axes " + ax.GetType() + " returned 0, aborting scoring");
#endif
                    return 0;
                }

                score *= axScore;

#if AI_DEBUG
                Debug.Log("Axis score " + axScore + ", running score " + score);
#endif
            }

            score = CompensateScore(_numAxes, score)*Priority;
            return score;
        }

        public virtual void OnExit(AiContext<T> context)
        {
        }

        public virtual void OnEnter(AiContext<T> context)
        {
        }

        public virtual void Update(AiContext<T> context)
        {
            if (!_isStarted)
            {
                StartTimedMission(TimedDuration);
            }

            var t = Time.time;

            _percentComplete = 1 - (_endTime - t)/TimedDuration;
            IsComplete = t > _endTime;
        }

        private void StartTimedMission(float duration)
        {
            _isStarted = true;
            _endTime = Time.time + (duration > 0 ? duration : 1e6f);
        }

        /// <summary>
        ///     Compensates scores with more axes, see
        ///     http://www.gdcvault.com/play/1021848/Building-a-Better-Centaur-AI
        ///     "Compensation Factor"
        /// </summary>
        /// <param name="numInputs"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        private static float CompensateScore(int numInputs, float score)
        {
            if (score >= 1) return score;

            var modFactor = 1 - 1/(float) numInputs;
            var makeup = (1 - score)*modFactor;
            return score + makeup*score;
        }
    }
}