// /** 
//  * HasTargetAxis.cs
//  * Will Hart
//  * 20161112
// */

namespace GameGHJ.AI.Axes
{
    #region Dependencies

    using GameGHJ.AI.Core;

    #endregion

    public class HasTargetAxis : IAxis<TacticalAiStateComp>
    {
        public float Score(AiContext<TacticalAiStateComp> context)
        {
            return Functions.OneIfFalse(context.State.AttackTarget == null);
        }

        public string Name => "Has Target";
        public string Description => "Returns 1 if there is a designated target, otherwise returns 0";
    }
}