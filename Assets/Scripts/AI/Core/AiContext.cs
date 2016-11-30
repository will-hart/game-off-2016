// /** 
//  * AiContext.cs
//  * Will Hart
//  * 20161103
// */

namespace GameGHJ.AI.Core
{
    using System;

    /// <summary>
    ///     A class that contains information suitable for the AI context
    /// </summary>
    public class AiContext<T> where T : AbstractAiStateComp
    {
        private readonly Entity _entity;

        public AiContext(Entity entity)
        {
            _entity = entity;
            State = _entity.GetComponent<T>();
        }

        /// <summary>
        ///     Gets a reference to the world state
        /// </summary>
        public T State { get; }

        /// <summary>
        ///     Gets a component from the attached entity
        ///     A convenience method to avoid calling .Owner repeatedly
        /// </summary>
        /// <typeparam name="TU"></typeparam>
        /// <returns></returns>
        public TU GetComponent<TU>() where TU : ComponentECS
        {
            return _entity.GetComponent<TU>();
        }
    }
}