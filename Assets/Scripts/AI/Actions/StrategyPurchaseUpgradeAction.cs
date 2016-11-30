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
    using FullSerializer;
    using GameGHJ.AI.Axes;
    using GameGHJ.AI.Core;
    using GameGHJ.AI.Influence;
    using UnityEngine;

    #endregion

    public class StrategyPurchaseUpgradeAction : AbstractAiAction<StrategicAiStateComp>
    {
        private SidePropertiesComp _side;
        private string _upgrade;

        public StrategyPurchaseUpgradeAction() : base(new List<IAxis<StrategicAiStateComp>>
        {
            new HasAnUpgradeOrderAxis(),
            new HasEnoughDnaForAbilityAxis()
            // TODO do we need an axis to balance new buildings vs upgrades?
        })
        {
        }

        public override int Priority => 1;

        public override void OnEnter(AiContext<StrategicAiStateComp> context)
        {
            _side = context.GetComponent<SidePropertiesComp>();

            // get the building type frome the build order
            _upgrade = context.State.UpgradeOrder[0];
            context.State.UpgradeOrder.RemoveAt(0);

            // spawn a building game object
            // TODO need a way to get bulding stats before creating building
            var upgradeTemplate = ZenBehaviourManager.Instance.GetComponent<EntityFactory>().GetTemplate(_upgrade)
                .AsDictionary["_components"]
                .AsDictionary["AbilityComp"]
                .AsDictionary;

            // deduct building cost
            _side.Dna -= (int)upgradeTemplate["UpgradeCost"].AsInt64;
            TimedDuration = (float)upgradeTemplate["UpgradeTime"].AsDouble;
            
            base.OnEnter(context);
        }

        public override void OnExit(AiContext<StrategicAiStateComp> context)
        {
            base.OnExit(context);
            _side.UnlockedUpgrades.Add(_upgrade);
        }
    }
}