
// /** 
//  * EnemyIsCloseToBaseAxis.cs
//  * Will Hart
//  * 20161119
// */

namespace GameGHJ.AI.Axes
{
    #region Dependencies

    using GameGHJ.AI.Influence;
    using GameGHJ.AI.Core;
    using System.Linq;
    using UnityEngine;

    #endregion

    public class EnemyIsCloseToBaseAxis : IAxis<StrategicAiStateComp>
    {
        private readonly float _dangerDistance;

        public EnemyIsCloseToBaseAxis(float dangerDistance = 10)
        {
            _dangerDistance = dangerDistance * dangerDistance;
        } 

        public float Score(AiContext<StrategicAiStateComp> context)
        {
            var hqPos = context.GetComponent<PositionComp>().position;
            var minDist = ZenBehaviourManager.Instance.Get<HeroComp>(ComponentTypes.HeroComp)
                .Where(h => h.Owner.GetComponent<UnitPropertiesComp>().teamID != context.State.FriendlyTeamId)
                .Select(h => h.Owner.GetComponent<PositionComp>())
                .DefaultIfEmpty(new PositionComp { position = new Vector3(-1e6f, 0, -1e6f) })
                .Min(h => (hqPos - h.position).sqrMagnitude);
            
            var ratio = _dangerDistance / minDist;
            return Mathf.Clamp01(ratio); // (its already squared)
        }

        public string Name => "Enemy Is Close To Base";
        public string Description => "Returns 1 if the nearest enemy influence is within a distance of base";
    }
}