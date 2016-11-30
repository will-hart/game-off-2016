// /** 
//  * AbstractAiStateComp.cs
//  * Will Hart
//  * 20161104
// */

using GameGHJ.Common.ZenECS;

public abstract class AbstractAiStateComp : ComponentECS
{
    public float NextAiPlanTime;

    public AbstractAiStateComp() : base()
    {
    }
}