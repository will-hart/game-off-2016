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
    using GameGHJ.AI.Bundles;
    using GameGHJ.AI.Core;
    using GameGHJ.AI.Influence;
    using UnityEngine;

    #endregion

    public class StrategyPostureAggressiveAction : AbstractAiAction<StrategicAiStateComp>
    {

        public StrategyPostureAggressiveAction() : base(new List<IAxis<StrategicAiStateComp>>
        {
            new MaxOfAxis<StrategicAiStateComp>(new HeroCountNearMaximumAxis(), new HeroCountIsEquivalentAxis()),
            new HeroPopulationMaxedOutAxis(),
            new IsAheadOnStrategicBalanceAxis()
        })
        {
        }

        public override void OnEnter(AiContext<StrategicAiStateComp> context)
        {
            base.OnEnter(context);
            context.State.Posture = Strategy.StrategicPostures.Aggressive;

            Update(context);
        }

        public override void Update(AiContext<StrategicAiStateComp> context)
        {
            base.Update(context);
            // get peaks
            var skippedHeroes = 0;
            var allPeaks = context.State.DecisionSpace.Vulnerability.GetPeaks();
            var safePeaks = allPeaks.Negative;
            var vulnerablePeaks = allPeaks.Positive;
            var hqPos3 = context.State.Owner.GetComponent<PositionComp>().position;
            var hqPos = new Vector2(hqPos3.x, hqPos3.z);
            var side = context.State.Owner.GetComponent<SidePropertiesComp>();
            var heroCount = side.LivingHeroes.Count;

            // sort the peaks by proximity to base
            vulnerablePeaks.Sort((a, b) => (a - hqPos).sqrMagnitude.CompareTo((b - hqPos).sqrMagnitude));
            // TODO this finds safe areas furthest from friendly HQ. Is there a better way?
            safePeaks.Sort((a, b) => (b - hqPos).sqrMagnitude.CompareTo((a - hqPos).sqrMagnitude));

            // TODO split heroes sensibly based on good combinations of types
            // TODO also allocate heroes based on proximity rather than randomly
            if (heroCount == 0) return;
            if (heroCount > 3)
            {
                side.LivingHeroes[0].Owner.GetComponent<TacticalAiStateComp>().Stance =
                    TacticalAiStateComp.TacticalAiStance.Scouting;
                skippedHeroes++;

                if (vulnerablePeaks.Count > 0)
                {
                    var safetyHero = side.LivingHeroes[1];
                    var shTacAi = safetyHero.Owner.GetComponent<TacticalAiStateComp>();
                    shTacAi.NavigationTarget = vulnerablePeaks[0];
                    shTacAi.NavigationTargetUpdated = true;
                    shTacAi.Stance = TacticalAiStateComp.TacticalAiStance.Defensive;
                    ++skippedHeroes;
                }
            }

            if (safePeaks.Count <= 0)
            {
                Debug.LogWarning("Unable to plan aggressive movement - no safe peaks found");
                return;
            }

            var vuln = safePeaks[0];
            foreach (var heroComp in side.LivingHeroes.Skip(skippedHeroes))
            {
                var tacAi = heroComp.Owner.GetComponent<TacticalAiStateComp>();
                tacAi.NavigationTarget = new Vector3(vuln.x, 0, vuln.y);
                tacAi.NavigationTargetUpdated = true;
                tacAi.Stance = TacticalAiStateComp.TacticalAiStance.Defensive;
            }
        }

        public override int Priority => 1;
    }
}