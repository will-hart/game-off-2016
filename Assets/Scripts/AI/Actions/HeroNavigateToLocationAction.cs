// /** 
//  * CreepNavigateToHero.cs
//  * Will Hart
//  * 20161103
// */

namespace GameGHJ.AI.Actions
{
    #region Dependencies

    using System.Collections.Generic;

    using Axes;
    using GameGHJ.AI.Core;

    #endregion

    /// <summary>
    /// Navigates the hero to a given location using the AI pathfinding system.
    /// 
    /// The navigation target is set by strategic AI or the player, not handled within tactical AI
    /// </summary>
    public class HeroNavigateToLocationAction : AbstractNavigationAction
    {
        public HeroNavigateToLocationAction() : base(new List<IAxis<TacticalAiStateComp>>
        {
            new IsFarFromNavigationTargetAxis()
        })
        {
        }

        protected override bool ShouldRunPathfinding(AiContext<TacticalAiStateComp> context) => false;
        public override int Priority => 1;
    }
}