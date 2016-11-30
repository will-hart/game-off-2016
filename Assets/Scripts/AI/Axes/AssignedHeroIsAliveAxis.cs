// /** 
//  * HasNearbyTargetsAxis.cs
//  * Will Hart
//  * 20161103
// */

namespace GameGHJ.AI.Axes
{
    #region Dependencies

    using System.Linq;
    using System.Runtime.Remoting.Messaging;
    using GameGHJ.AI.Core;
    using UnityEngine;

    #endregion

    public class AssignedHeroIsAliveAxis : IAxis<TacticalAiStateComp>
    {
        public float Score(AiContext<TacticalAiStateComp> context)
        {
            var hero = context.GetComponent<CreepComp>().AssignedHero;
            if (hero == null) return 0;
            
            return Functions.OneIfFalse(hero.Owner.GetComponent<HealthComp>().isDead);
        }

        public string Name => "Assigned Hero Is Alive";
        public string Description => "Returns 1 if the assigned hero is alive, otherwise returns 0";
    }
}