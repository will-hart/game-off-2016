// /** 
//  * IAxis.cs
//  * Will Hart
//  * 20161103
// */

namespace GameGHJ.AI.Axes
{
    #region Dependencies

    using Core;

    #endregion

    public interface IAxis<T> where T : AbstractAiStateComp
    {
        string Name { get; }
        string Description { get; }
        float Score(AiContext<T> context);
    }
}