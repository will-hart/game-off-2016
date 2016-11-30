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

    public class StrategyPostureDefensiveAction : AbstractAiAction<StrategicAiStateComp>
    {
        public StrategyPostureDefensiveAction() : base(new List<IAxis<StrategicAiStateComp>>
        {
            new IsBehindOnStrategicBalanceAxis(-0.5f)
        })
        {
        }

        public override void OnEnter(AiContext<StrategicAiStateComp> context)
        {
            base.OnEnter(context);
            context.State.Posture = Strategy.StrategicPostures.Defensive;

            Update(context);
        }

        public override void Update(AiContext<StrategicAiStateComp> context)
        {
            base.Update(context);

            // get peaks
            var allPeaks = context.State.DecisionSpace.Vulnerability.GetPeaks().Positive; // positive peaks are vulnerable areas

            var hqWorldPos = context.GetComponent<PositionComp>().position;
            var hqPos3 = context.State.DecisionSpace.Influence.ConvertWorldToInfluence(hqWorldPos);
            var hqPos = new Vector2(hqPos3.first, hqPos3.second);

            var side = context.GetComponent<SidePropertiesComp>();
            var heroCount = side.LivingHeroes.Count;
            var currentVulnerabilityIdx = 0;

            // sort the peaks by proximity to base
            allPeaks.Sort((a, b) => (a - hqPos).sqrMagnitude.CompareTo((b - hqPos).sqrMagnitude));

            // TODO split heroes sensibly based on good combinations of types
            // TODO also allocate heroes based on proximity rather than randomly
            // TODO attempt to recapture vulnerable points with spare heroes
            if (heroCount == 0) return;

            for (var i = 0; i < heroCount; i += 2)
            {
                var heroes = side.LivingHeroes.Skip(i * 2).Take(2);
                var vuln = currentVulnerabilityIdx >= allPeaks.Count ? hqPos : allPeaks[currentVulnerabilityIdx];

                foreach (var heroComp in heroes)
                {
                    var vulnWorldPos = context.State.DecisionSpace.Influence.ConvertInfluenceToWorld((int)vuln.x, (int)vuln.y);
                    var vulnPos = hqWorldPos + 50 * (vulnWorldPos - hqWorldPos).normalized;
                    vulnPos.y = 0;

                    var tacAi = heroComp.Owner.GetComponent<TacticalAiStateComp>();
                    tacAi.NavigationTarget = vulnPos;
                    tacAi.NavigationTargetUpdated = true;
                    tacAi.Stance = TacticalAiStateComp.TacticalAiStance.Defensive;
                }

                ++currentVulnerabilityIdx;
            }
        }

        public override int Priority => 1;
    }
}