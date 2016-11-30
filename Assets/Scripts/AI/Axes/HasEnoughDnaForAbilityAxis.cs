// /** 
//  * HasEnoughDnaForAbilityAxis.cs
//  * Will Hart
//  * 20161127
// */

namespace GameGHJ.AI.Axes
{
    #region Dependencies

    using System.Linq;
    using GameGHJ.AI.Core;

    #endregion

    public class HasEnoughDnaForAbilityAxis : IAxis<StrategicAiStateComp>
    {
        public float Score(AiContext<StrategicAiStateComp> context)
        {
            // TODO get building cost somehow - not really feasible given the entity loading method
            var nextUpgrade = context.State.UpgradeOrder.Count > 0 ? context.State.UpgradeOrder.First() : null;
            if (nextUpgrade == null) return 0;

            var upgradeData = ZenBehaviourManager.Instance.GetComponent<EntityFactory>().GetTemplate(nextUpgrade);
            var cost = (float) upgradeData
                .AsDictionary["_components"]
                .AsDictionary["AbilityComp"]
                .AsDictionary["UpgradeCost"]
                .AsDouble;

            var side = context.GetComponent<SidePropertiesComp>();
            return Functions.Sextic(side.Dna/(1.5f*cost));
        }

        public string Name => "Has Enough DNA For Ability";
        public string Description => "Returns (Dna/(2*cost)) ^ 6, [0..1]";
    }
}