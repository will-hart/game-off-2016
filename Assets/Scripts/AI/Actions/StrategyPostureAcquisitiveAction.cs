// /** 
//  * StrategyPostureAcquisitiveAction.cs
//  * Will Hart
//  * 20161103
// */

namespace GameGHJ.AI.Actions
{
    #region Dependencies

    using System.Collections.Generic;
    using System.Linq;
    using GameGHJ.AI.Axes;
    using GameGHJ.AI.Bundles;
    using GameGHJ.AI.Core;
    using GameGHJ.AI.Influence;
    using UnityEngine;
    using Strategy;
    #endregion

    public class StrategyPostureAcquisitiveAction : AbstractAiAction<StrategicAiStateComp>
    {

        public StrategyPostureAcquisitiveAction() : base(new List<IAxis<StrategicAiStateComp>>
        {
            new HasAvailableResourcePointsAxis(),
            new EmptyResourcePointsAreSafeAxis()
        })
        {
        }

        public override void OnEnter(AiContext<StrategicAiStateComp> context)
        {
            base.OnEnter(context);
            context.State.Posture = Strategy.StrategicPostures.Acquisitive;

            Update(context);
        }

        public override void Update(AiContext<StrategicAiStateComp> context)
        {
            base.Update(context);

            var side = context.GetComponent<SidePropertiesComp>();
            var closePoints = ResourcePointUtility.GetResourcePoints(side);
            var heroCount = side.LivingHeroes.Count;

            if (heroCount > 0 && heroCount <= 2)
            {
                AllocateAvailablePoint(closePoints, side.LivingHeroes);
            }
            else if (heroCount == 3)
            {
                // TODO pick a more appropriate scout rather than the first hero
                side.LivingHeroes.First().Owner.GetComponent<TacticalAiStateComp>().Stance =
                    TacticalAiStateComp.TacticalAiStance.Scouting;

                var caps = side.LivingHeroes.Skip(1);
                AllocateAvailablePoint(closePoints, caps);
            }
            else if (heroCount > 3)
            {
                var skipped = false;

                if (heroCount % 2 == 1)
                {
                    skipped = true;
                    side.LivingHeroes.First().Owner.GetComponent<TacticalAiStateComp>().Stance =
                        TacticalAiStateComp.TacticalAiStance.Scouting;
                }

                for (var i = skipped ? 1 : 0; i < heroCount - 1; i += 2)
                {
                    var caps = side.LivingHeroes.Skip(i * 2).Take(2);
                    AllocateAvailablePoint(closePoints, caps);
                }
            }
        }

        /// <summary>
        /// Sends the list of heroes passed in to the closes navigation point in the list of points passed in
        /// </summary>
        /// <param name="closePoints"></param>
        /// <param name="heroes"></param>
        private static void AllocateAvailablePoint(IList<ResourceProductionComp> closePoints, IEnumerable<HeroComp> heroes)
        {
            if (closePoints.Count <= 0) return;

            var targetPos = closePoints.First().Owner.GetComponent<PositionComp>().position;
            SetNavigationTargets(heroes, targetPos);
            closePoints.RemoveAt(0);
        }

        /// <summary>
        /// Sets the navigation targets for the given hero components
        /// </summary>
        /// <param name="heroes"></param>
        /// <param name="targetPos"></param>
        private static void SetNavigationTargets(IEnumerable<HeroComp> heroes, Vector3 targetPos)
        {
            foreach (var hero in heroes)
            {
                var ai = hero.Owner.GetComponent<TacticalAiStateComp>();
                ai.NavigationTarget = targetPos;
                ai.NavigationTargetUpdated = true;
            }
        }

        public override int Priority => 1;
    }
}