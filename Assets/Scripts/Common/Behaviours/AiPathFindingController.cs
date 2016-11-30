namespace GameGHJ.Common.Behaviours
{
    using System;
    using Pathfinding;
    using UnityEngine;

    public class AiPathFindingController : ZenBehaviour, IOnAwake
    {
        private Seeker _seeker;
        private TacticalAiStateComp _ai;

        public void GetPath(TacticalAiStateComp ai)
        {
            _ai = ai;
            _seeker.StartPath(ai.Owner.GetComponent<PositionComp>().position, ai.NavigationTarget, ReceivePath);
        }

        private void ReceivePath(Path p)
        {
            _ai.IsFindingPath = false;
            _ai.CurrentWaypoint = 0;
            _ai.Waypoints = p.vectorPath;
        }

        public void OnAwake()
        {
            _seeker = GetComponent<Seeker>();
        }
        
        public override int ExecutionPriority => 1;
        public override Type ObjectType => typeof(AiPathFindingController);
    }
}

