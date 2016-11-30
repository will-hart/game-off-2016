// /** 
//  * IsNotNearNavigationTargetAxis.cs
//  * Will Hart
//  * 20161103
// */

namespace GameGHJ.AI.Axes
{
    #region Dependencies

    using System.Linq;
    using UnityEngine;

    using Core;

    #endregion

    public class IsOutnumberedAxis : IAxis<TacticalAiStateComp>
    {
        private readonly float _offset;

        public IsOutnumberedAxis(float offset = 0.5f)
        {
            _offset = offset;
        }

        public float Score(AiContext<TacticalAiStateComp> context)
        {
            var combat = context.GetComponent<CombatComp>();
            var enemiesInRange = combat.EnemiesInRange.Count;

            if (enemiesInRange == 0) return 0;

            var firstEnemy = combat.EnemiesInRange
                .FirstOrDefault(pos => pos.Owner.HasComponent(ComponentTypes.CombatComp));
            if (firstEnemy == null) return 0;

            var friendliesInRange = firstEnemy.Owner.GetComponent<CombatComp>().EnemiesInRange.Count;

            var ratio = Mathf.Clamp01(enemiesInRange/(float)friendliesInRange - _offset);
            return Functions.ReverseLogistic(ratio);
        }

        public string Name => "Is Outnumbered";

        public string Description
            => "Returns [0-1], 1 is outnumbered, 0 not outnumbered, 0.5 is balanced (by default, unless a different offset is provided in the constructor)";
    }
}