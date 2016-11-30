// /** 
//  * BuildingProductionSystem.cs
//  * Will Hart
//  * 20161104
// */

namespace GameGHJ.Systems
{
    #region Dependencies

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Unitilities.Tuples;
    using UnityEngine;

    #endregion

    /// <summary>
    /// Periodically produces creeps from production buildings
    /// </summary>
    public class ResourceProductionSystem : ZenBehaviour, IOnUpdate
    {
        private float _nextOwnershipUpdate;
        [SerializeField] private float _ownershipUpdatePeriod = 0.5f;
        [SerializeField] private LayerMask _capLayerMask;

        public override Type ObjectType => typeof(BuildingProductionSystem);
        public override int ExecutionPriority => 0;

        public void OnUpdate()
        {
            var buildings = ZenBehaviourManager.Instance.Get<ResourceProductionComp>(ComponentTypes.ResourceProductionComp);
            
            // convert to dict prevent multiple "find" ops on sides array to find the component's side
            var sidesLookup =
                ZenBehaviourManager.Instance.Get<SidePropertiesComp>(ComponentTypes.SidePropertiesComp)
                    .ToDictionary(side => side.SideId);

            var updateOwnership = Time.time > _nextOwnershipUpdate;

            if (updateOwnership) _nextOwnershipUpdate = Time.time + _ownershipUpdatePeriod;

            foreach (var building in buildings)
            {
                if (updateOwnership) UpdateOwnership(sidesLookup, building);
                UpdateProduction(sidesLookup, building);
            }
        }

        /// <summary>
        /// Updates ownership of resource buildings
        /// </summary>
        /// <param name="sidesLookup"></param>
        /// <param name="building"></param>
        private void UpdateOwnership(IDictionary<int, SidePropertiesComp> sidesLookup, ResourceProductionComp building)
        {
            // circle cast to find nearby units
            var pos = building.Owner.GetComponent<PositionComp>().position;
            var nearbyHits = Physics.OverlapSphere(pos,
                building.CapDistance, _capLayerMask.value, QueryTriggerInteraction.Ignore);

            Debug.DrawLine(pos, pos + building.CapDistance * Vector3.right);
            Debug.DrawLine(pos, pos + building.CapDistance * Vector3.left);
            Debug.DrawLine(pos, pos + building.CapDistance * Vector3.forward);
            Debug.DrawLine(pos, pos + building.CapDistance * Vector3.back);

            // revert cap percent to 0 or capped if there are no colliders in range
            var capDelta = 0.025f;
            var target = 1;
            var currentOwningSide = building.OwningSideId;
            var anybodyCapping = nearbyHits.Length > 0;
            var topInfluenceSideId = 0;

            if (anybodyCapping)
            {
                topInfluenceSideId = nearbyHits
                    .Select(nh => nh.GetComponentInParent<EntityWrapper>()?.entity)
                    .Where(ent => ent != null && ent.HasComponent(ComponentTypes.InfluenceComp))
                    .Select(nh => new TupleI(nh.GetComponent<UnitPropertiesComp>().teamID, nh.GetComponent<InfluenceComp>().InfluenceStrength))
                    .DefaultIfEmpty(new TupleI(0, 0))
                    .Aggregate(new Dictionary<int, int>(),
                        (influences, tup) =>
                        {
                            if (!influences.ContainsKey(tup.first)) influences.Add(tup.first, 0);
                            influences[tup.first] += tup.second;
                            return influences;
                        })
                    .Aggregate((next, curr) => (next.Value > curr.Value) || (next.Value == curr.Value && next.Key == building.OwningSideId) ? next : curr).Key;

                if (topInfluenceSideId == 0)
                {
                    anybodyCapping = false;
                }
            }

            if (!anybodyCapping)
            {
                // just decay slowly towards capped / non-capped value
                capDelta = capDelta/2f;
                target = building.IsCapped ? 1 : 0;
            }
            else
            {
                if (topInfluenceSideId != currentOwningSide)
                {
                    target = 0;
                }
            }

            // Update the building cap value
            building.CapPercent = Mathf.Clamp01(Mathf.MoveTowards(building.CapPercent, target, capDelta));

            // Handle uncapped buildings
            if (building.CapPercent <= float.Epsilon)
            {
                building.CapPercent = 0;
                building.IsCapped = false;
                building.OwningSideId = topInfluenceSideId;
            }
            // handle capped buildings
            else if (building.CapPercent >= 1 && !building.IsCapped)
            {
                building.IsCapped = true;
                building.CapPercent = 1;
            }
        }

        /// <summary>
        /// Produce from owned buildings
        /// </summary>
        /// <param name="sidesLookup"></param>
        /// <param name="building"></param>
        private static void UpdateProduction(IDictionary<int, SidePropertiesComp> sidesLookup, ResourceProductionComp building)
        {
            var now = Time.time;
            if (building.NextProductionTime > now) return;
            building.NextProductionTime = now + building.ProductionPeriod;

            if (!building.IsCapped) return;

            var sideId = building.OwningSideId;
            sidesLookup[sideId].Dna += building.ProductionQuantity;
        }
    }
}