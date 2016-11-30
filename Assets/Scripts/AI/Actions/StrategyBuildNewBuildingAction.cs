// /** 
//  * CreepAttackNearbyTagets.cs
//  * Will Hart
//  * 20161103
// */

namespace GameGHJ.AI.Actions
{
    #region Dependencies

    using System.Collections.Generic;
    using System.Linq;
    using GameGHJ.AI.Axes;
    using GameGHJ.AI.Core;
    using GameGHJ.AI.Influence;
    using Pathfinding;
    using UnityEngine;

    #endregion

    public class StrategyBuildNewBuildingAction : AbstractAiAction<StrategicAiStateComp>
    {
        private BuildingComp _buildingComp;

        public StrategyBuildNewBuildingAction() : base(new List<IAxis<StrategicAiStateComp>>
        {
            new HasABuildOrderAxis(),
            new HasEnoughDnaForBuildingAxis()
        })
        {
        }

        public override int Priority => 1;

        public override void OnEnter(AiContext<StrategicAiStateComp> context)
        {
            base.OnEnter(context);
            
            var side = context.GetComponent<SidePropertiesComp>();
            var pos = context.GetComponent<PositionComp>().position;


            // get the building type frome the build order
            var buildingType = context.State.BuildOrder[0];
            var vulnPeaks = context.State.DecisionSpace.Vulnerability.GetPeaks();
            var safePlaces = buildingType == ConstructableTypes.Tower
                ? GetTowerPlacements(context, vulnPeaks)
                : GetBuildingPlacements(context, vulnPeaks);

            if (!safePlaces.Any())
            {
                Debug.Log("Unable to place building, no safe places");
                IsComplete = true;
                return;
            }
            
            var buildingPos = safePlaces.First();
            Debug.Log($"Building {buildingType} at {buildingPos}");
            context.State.BuildOrder.RemoveAt(0);

            // spawn a building game object
            var go = BuildingConstructionController.BeginAiBuildProcess(buildingType, buildingPos);
            go.GetComponent<Rigidbody>().position = buildingPos;
            _buildingComp = go.GetComponent<EntityWrapper>().entity.GetComponent<BuildingComp>();
            _buildingComp.Owner.GetComponent<UnitPropertiesComp>().teamID = context.State.FriendlyTeamId;

            
            // deduct building cost
            side.Dna -= _buildingComp.ConstructionCost;
            TimedDuration = _buildingComp.ConstructionTime + 2f;
        }

        public override void Update(AiContext<StrategicAiStateComp> context)
        {
            base.Update(context);

            if (IsComplete) return;

            if (_buildingComp.ConstructionPercentage >= 1)
            {
                IsComplete = true;
            }
        }

        /// <summary>
        /// Gets a list of vector3 locations for towers, sorted by preference
        /// Locations have been checked to make sure they are walkable and are in world space
        /// 
        /// NOTE - currently only returns a single point
        /// </summary>
        /// <param name="context"></param>
        /// <param name="vulnPeaks"></param>
        /// <returns></returns>
        private static List<Vector3> GetTowerPlacements(AiContext<StrategicAiStateComp> context, PeakResults vulnPeaks)
        {
            return vulnPeaks.Negative.Count == 0
                ? new List<Vector3>()
                : GetSuitableConstructionPosition(context, vulnPeaks.Negative.First(), 20, 40);
        }

        /// <summary>
        /// Gets a list of vector3 locations for buildings, sorted by preference
        /// Locations have been checked to make sure they are walkable and are in world space
        /// 
        /// NOTE - currently only returns a single point
        /// </summary>
        /// <param name="context"></param>
        /// <param name="vulnPeaks"></param>
        /// <returns></returns>
        private static List<Vector3> GetBuildingPlacements(AiContext<StrategicAiStateComp> context, PeakResults vulnPeaks)
        {
            return vulnPeaks.Positive.Count == 0
                ? new List<Vector3>()
                : GetSuitableConstructionPosition(context, vulnPeaks.Positive.First());
        }

        /// <summary>
        /// Returns a walkable position within a certain distance of the influence position. 
        /// The position is world pos
        /// </summary>
        /// <param name="context"></param>
        /// <param name="influencePos"></param>
        /// <param name="minRange"></param>
        /// <param name="maxRange"></param>
        /// <returns></returns>
        private static List<Vector3> GetSuitableConstructionPosition(
                AiContext<StrategicAiStateComp> context,
                Vector2 influencePos,
                float minRange = 10,
                float maxRange = 30)
        {
            var hqPos = context.GetComponent<PositionComp>().position;
            var pos3 = context.State.DecisionSpace.Influence.ConvertInfluenceToWorld((int)influencePos.x, (int)influencePos.y);
            pos3 = (pos3 - hqPos).normalized*Random.Range(minRange, maxRange) + hqPos;

            var walkablePos =
                (Vector3)
                AstarPath.active.GetNearest(pos3, new NNConstraint {constrainDistance = true, walkable = true})
                    .node.position;
            return new List<Vector3> { walkablePos };
        }
    }
}