// /** 
//  * ResourcePointUtility.cs
//  * Will Hart
//  * 20161105
// */

namespace GameGHJ.AI.Strategy
{
    #region Dependencies

    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    #endregion

    /// <summary>
    /// Contains utilities for querying resource points. Used by the strategic AI
    /// </summary>
    public static class ResourcePointUtility
    {
        /// <summary>
        ///     Gets a list of resource points in the sequence:
        ///     - all neutral points sorted by ascending distance from HQ, then
        ///     - all enemy points sorted by ascending distance from HQ, then
        ///     - all friendly points sorted by DESCENDING distance from HQ
        /// </summary>
        /// <param name="side"></param>
        /// <returns></returns>
        public static List<ResourceProductionComp> GetResourcePoints(SidePropertiesComp side)
        {
            var enemy = new List<ResourceProductionComp>();
            var friendly = new List<ResourceProductionComp>();
            var neutral = new List<ResourceProductionComp>();

            var resources =
                ZenBehaviourManager.Instance.Get<ResourceProductionComp>(ComponentTypes.ResourceProductionComp).ToList();

            if (resources.Count == 0) return neutral;

            var hqPos = side.Owner.GetComponent<PositionComp>().position;
            var friendlyId = side.SideId;

            foreach (var resource in resources)
            {
                if (resource.OwningSideId == 0 || !resource.IsCapped) neutral.Add(resource);
                else if (resource.OwningSideId == friendlyId) friendly.Add(resource);
                else enemy.Add(resource);
            }

            // sort the results by distance
            // NOTE the sorter could probably be optimised, maybe by calculating distance once per 
            //      resource point and storing that separately then manually building up the list?
            //      this should do for now as it isn't run very often
            neutral.Sort((a, b) => ComponentProximitySorter(a, b, hqPos));
            friendly.Sort((a, b) => ComponentProximitySorter(a, b, hqPos, true));
            enemy.Sort((a, b) => ComponentProximitySorter(a, b, hqPos));

            neutral.AddRange(enemy);
            neutral.AddRange(friendly);

            return neutral;
        }

        /// <summary>
        ///     Sorts resources by distance from the given HQ point
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="target"></param>
        /// <param name="farthest"></param>
        /// <returns></returns>
        public static int ComponentProximitySorter(
            ComponentECS a,
            ComponentECS b,
            Vector3 target,
            bool farthest = false)
        {
            var aPos = a.Owner.GetComponent<PositionComp>().position;
            var bPos = b.Owner.GetComponent<PositionComp>().position;

            return farthest
                ? (bPos - target).sqrMagnitude.CompareTo((aPos - target).sqrMagnitude)
                : (aPos - target).sqrMagnitude.CompareTo((bPos - target).sqrMagnitude);
        }
    }
}