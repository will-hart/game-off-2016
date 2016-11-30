// /** 
//  * InvertAxis.cs
//  * Will Hart
//  * 20161110
// */

namespace GameGHJ.AI.Axes
{
    #region Dependencies

    using GameGHJ.AI.Core;
    using UnityEngine;

    #endregion

    public class MaxOfAxis<T> : IAxis<T> where T : AbstractAiStateComp
    {
        private readonly IAxis<T> _axis1;
        private readonly IAxis<T> _axis2;

        public MaxOfAxis(IAxis<T> axis1, IAxis<T> axis2)
        {
            _axis1 = axis1;
            _axis2 = axis2;
        }

        public float Score(AiContext<T> context)
        {
            var a = _axis1.Score(context);
            var b = _axis2.Score(context);
            return Mathf.Clamp01(Mathf.Max(a, b));
        }

        public override string ToString() => Name;
        public string Name => $"Max of {_axis1} and {_axis2}";
        public string Description => "Returns the larger score of the two enclosed axes";
    }
}