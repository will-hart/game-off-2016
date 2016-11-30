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

    public class InvertAxis<T> : IAxis<T> where T : AbstractAiStateComp
    {
        private readonly IAxis<T> _axis;

        public InvertAxis(IAxis<T> axis)
        {
            _axis = axis;
        }

        public float Score(AiContext<T> context)
        {
            return Mathf.Clamp01(1 - _axis.Score(context));
        }

        public override string ToString()
        {
            return $"{Name} {_axis}";
        }

        public string Name => "Invert";
        public string Description => "Returns 1 minus the value of the enclosed axis";
    }
}