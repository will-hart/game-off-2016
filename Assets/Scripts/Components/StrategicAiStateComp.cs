using System;
using System.Collections.Generic;
using GameGHJ.AI.Actions;
using GameGHJ.AI.Core;
using GameGHJ.AI.Influence;
using GameGHJ.AI.Strategy;
using GameGHJ.Common.ZenECS;

public class StrategicAiStateComp : AbstractAiStateComp
{
    public int FriendlyTeamId;
    public int EnemyTeamId;
    public StrategicPostures Posture;
    
    [NonSerialized] public StrategicDecisionSpace DecisionSpace;
    [NonSerialized] public AiActionContainer<StrategicAiStateComp> ActionContainer;
    [NonSerialized] public AbstractAiAction<StrategicAiStateComp> Action;
    [NonSerialized] public AiActionContainer<StrategicAiStateComp> PostureActionContainer;
    [NonSerialized] public AbstractAiAction<StrategicAiStateComp> PostureAction;
    [NonSerialized] public InfluenceMapBalance InfluenceBalance;


    public List<AbstractAiAction<StrategicAiStateComp>> ActiveActions;

    public List<ConstructableTypes> BuildOrder = new List<ConstructableTypes>();
    public List<string> UpgradeOrder = new List<string>();

    public StrategicAiStateComp() : base()
    {
    }

    public override ComponentTypes ComponentType => ComponentTypes.StrategicAiStateComp;
}