// /** 
//  * AiActionBundle.cs
//  * Will Hart
//  * 20161103
// */

namespace GameGHJ.AI.Bundles
{
    #region Dependencies

    using System.Collections.Generic;
    using Actions;

    #endregion

    public abstract class AiActionBundle<T> where T : AbstractAiStateComp
    {
        private readonly List<AbstractAiAction<T>> _actions = new List<AbstractAiAction<T>>();

        protected AiActionBundle(IEnumerable<AbstractAiAction<T>> actions)
        {
            _actions.AddRange(actions);
            _actions.Sort((a, b) => b.Priority.CompareTo(a.Priority));
        }

        public IEnumerable<AbstractAiAction<T>> Actions => _actions;
    }
}