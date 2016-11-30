// /** 
//  * CreepNavigateToHero.cs
//  * Will Hart
//  * 20161103
// */

namespace GameGHJ.AI.Actions
{
    #region Dependencies

    using System.Collections.Generic;

    using Axes;
    using GameGHJ.AI.Core;
    using UnityEngine;

    #endregion

    /// <summary>
    /// Sets navigation targets at set intervals
    /// </summary>
    public abstract class AbstractNavigationAction : AbstractAiAction<TacticalAiStateComp>
    {
        protected GameObject _meshObject;

        protected AbstractNavigationAction(IEnumerable<IAxis<TacticalAiStateComp>> axes) : base(axes)
        {
        }

        public override void OnEnter(AiContext<TacticalAiStateComp> context)
        {
            base.OnEnter(context);
            _meshObject = context.State.Owner.Wrapper.gameObject.GetComponentInChildren<MeshRenderer>()?.gameObject;
        }

        public override void Update(AiContext<TacticalAiStateComp> context)
        {
            base.Update(context);

            var ai = context.State;
            var move = context.GetComponent<MovementComp>();
            var hasWaypoints = ai.Waypoints != null && ai.Waypoints.Count > 0;

            if (!hasWaypoints)
            {
                move.currentMoveSpeed = UpdateSpeed(false, move, context);
                IsComplete = true;
                return;
            }
            
            // check if we are at the end of the existing path
            if (ai.CurrentWaypoint >= ai.Waypoints.Count)
            {
                ai.CurrentWaypoint = 0;
                ai.Waypoints.Clear();
                return;
            }

            // navigate through waypoints
            var wp = ai.Waypoints[ai.CurrentWaypoint];
            var pos = context.GetComponent<PositionComp>().position;
            var dir = wp - pos;

            move.currentMoveSpeed = UpdateSpeed(true, move, context);

            var delta = dir.normalized * move.currentMoveSpeed * Time.deltaTime;
            delta.y = 0.01f; // make sure we don't bobble up and down or go through the floor

            var rb = ai.Owner.Wrapper.gameObject.GetComponent<Rigidbody>();
            rb.MovePosition(pos + delta);
            
            if (_meshObject != null && delta.sqrMagnitude > 0.1f)
            {
                var lookRot = 50 * delta;
                lookRot.y = 0;
                _meshObject.transform.rotation = Quaternion.Slerp(
                    _meshObject.transform.rotation,
                    Quaternion.LookRotation(lookRot, Vector3.up),
                    Time.deltaTime * 5);
            }

            if (dir.x * dir.x + dir.z * dir.z < 2.5)
            {
                ++ai.CurrentWaypoint;
            }

            if (ShouldRunPathfinding(context))
            {
                RunPathFinding(context);
            }
        }

        protected abstract bool ShouldRunPathfinding(AiContext<TacticalAiStateComp> context);

        protected virtual Vector3 GetPathFindingTarget(AiContext<TacticalAiStateComp> context)
        {
	        var cc = context.GetComponent<CreepComp>();
	        var ah = cc.AssignedHero;
	        var pos = ah.position;
            return context.GetComponent<CreepComp>().AssignedHero.position;
        }

        protected virtual void RunPathFinding(AiContext<TacticalAiStateComp> context)
        {
            // the path is handled by the TacticalAiMovementSystem, this just sets the destination
            context.State.NavigationTarget = GetPathFindingTarget(context);
            context.State.NavigationTargetUpdated = true;
        }

        protected virtual float UpdateSpeed(bool hasWaypoints, MovementComp move, AiContext<TacticalAiStateComp> context)
        {
            return hasWaypoints
                ? move.moveSpeed
                : 0;
        }
    }
}