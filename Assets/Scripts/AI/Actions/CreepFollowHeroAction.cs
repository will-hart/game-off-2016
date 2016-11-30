// /** 
//  * CreepNavigateToHero.cs
//  * Will Hart
//  * 20161103
// */

namespace GameGHJ.AI.Actions
{
    #region Dependencies

    using System;
    using System.Collections.Generic;

    using Axes;
    using GameGHJ.AI.Core;
    using UniRx;
    using UnityEngine;

    #endregion

    public class CreepFollowHeroAction : AbstractNavigationAction
    {
        private const float PathfindingFrequency = 0.7f;
        private float _nextPathfindingTime;
        private bool _canRaycast = true;

        public CreepFollowHeroAction() : base(new List<IAxis<TacticalAiStateComp>>
        {
            new HasAssignedHeroAxis(),
            new AssignedHeroIsAliveAxis(),
            new IsNearHeroAxis()
        })
        {
        }

        public override int Priority => 1;

        public override void OnEnter(AiContext<TacticalAiStateComp> context)
        {
            base.OnEnter(context);
            context.State.Waypoints?.Clear();
            context.State.CurrentWaypoint = 0;
            _nextPathfindingTime = Time.time;
        }

        public override void Update(AiContext<TacticalAiStateComp> context)
        {
            // attempt to raycast to the hero, if we can raycast then just move directly to them
            // otherwise we'll have to pass over to pathfinding
            var pos = context.GetComponent<PositionComp>();
            var hero = context.GetComponent<CreepComp>().AssignedHero.Owner;
            var heroPos = hero.GetComponent<PositionComp>();
            var heroSpeed = hero.GetComponent<MovementComp>().currentMoveSpeed;

            // if the hero isn't moving, don't bother about raycasting, etc
            if (Math.Abs(heroSpeed) < float.Epsilon)
            {
                base.Update(context);
                return;
            }

            RaycastHit hit;
            var didHit = Physics.Linecast(pos.position, heroPos.position, out hit);
            _canRaycast = !didHit || hit.collider.gameObject.GetComponentInParent<EntityWrapper>() == hero.Wrapper;

            Debug.DrawLine(pos.position, heroPos.position, didHit ? Color.green : Color.cyan);

            if (!_canRaycast || context.State.Waypoints == null)
            {
                base.Update(context);
                return;
            }

            if (context.State.Waypoints.Count == 1)
            {
                context.State.Waypoints[0] = heroPos.position;
            }
            else
            {
                context.State.Waypoints.Clear();
                context.State.Waypoints.Add(heroPos.position);
            }

            base.Update(context);
        }

        /// <summary>
        /// Only run pathfinding if we can't raycast to the hero and it is time
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override bool ShouldRunPathfinding(AiContext<TacticalAiStateComp> context)
        {
            if (_canRaycast || Time.time < _nextPathfindingTime) return false;
            _nextPathfindingTime = Time.time + PathfindingFrequency;
            return true;
        }

        /// <summary>
        /// Move at the hero's speed, or the creep's movement speed, whichever is lower.
        /// </summary>
        /// <param name="hasWaypoints"></param>
        /// <param name="move"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override float UpdateSpeed(bool hasWaypoints, MovementComp move, AiContext<TacticalAiStateComp> context)
        {
            var hero = context.GetComponent<CreepComp>().AssignedHero.Owner;
            var heroMovement = hero.GetComponent<MovementComp>();
            return Mathf.Min(heroMovement.currentMoveSpeed, move.moveSpeed);
        }
    }
}