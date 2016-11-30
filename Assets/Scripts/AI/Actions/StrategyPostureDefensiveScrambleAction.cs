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

    public class StrategyPostureDefensiveScrambleAction : AbstractAiAction<StrategicAiStateComp>
    {
        public StrategyPostureDefensiveScrambleAction() : base(new List<IAxis<StrategicAiStateComp>>
        {
            new EnemyIsCloseToBaseAxis()
        })
        {
        }

        public override void OnEnter(AiContext<StrategicAiStateComp> context)
        {
            base.OnEnter(context);
            context.State.Posture = Strategy.StrategicPostures.Scramble;

            Update(context);
        }

        public override void Update(AiContext<StrategicAiStateComp> context)
        {
            base.Update(context);

            // get peaks
            var allPeaks = context.State.DecisionSpace.Vulnerability.GetPeaks().Positive; // positive peaks are vulnerable areas
            var hqPos3 = context.State.Owner.GetComponent<PositionComp>().position;
            var hqPos = new Vector2(hqPos3.x, hqPos3.z);
            var side = context.State.Owner.GetComponent<SidePropertiesComp>();
            var heroCount = side.LivingHeroes.Count;
            var currentVulnerabilityIdx = 0;

            // sort the peaks by proximity to base
            allPeaks.Sort((a, b) => (a - hqPos).sqrMagnitude.CompareTo((b - hqPos).sqrMagnitude));

            // TODO split heroes sensibly based on good combinations of types
            // TODO also allocate heroes based on proximity rather than randomly
            if (heroCount == 0) return;

            for (var i = 0; i < heroCount; i += 2)
            {
                var heroes = side.LivingHeroes.Skip(i * 2).Take(2);
                var vuln = i == 0 
                    ? hqPos 
                    : currentVulnerabilityIdx >= allPeaks.Count ? hqPos : allPeaks[currentVulnerabilityIdx];

                foreach (var heroComp in heroes)
                {
                    // TODO prevent heroes from moving too far from home
                    var tacAi = heroComp.Owner.GetComponent<TacticalAiStateComp>();
                    tacAi.NavigationTarget = new Vector3(vuln.x, 0, vuln.y);
                    tacAi.NavigationTargetUpdated = true;
                    tacAi.Stance = TacticalAiStateComp.TacticalAiStance.Defensive;
                }

                if (i > 0) ++currentVulnerabilityIdx;
            }
        }

        public override int Priority => 2;
    }
}