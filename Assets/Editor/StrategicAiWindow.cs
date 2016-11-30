// /** 
//  * TacticalAiWindow.cs
//  * Will Hart
//  * 20161115
// */

namespace GameGHJ.Editor
{
    #region Dependencies

    using System.Collections.Generic;
    using System.Linq;
    using GameGHJ.AI.Core;
    using GameGHJ.Systems;
    using UnityEditor;
    using UnityEngine;
    using AI.Influence;
    using GameGHJ.AI.Actions;
    using Unitilities.Tuples;
    #endregion

    public class StrategicAiWindow : AbstractAiWindow
    {
        private readonly Dictionary<string, bool> _foldoutData = new Dictionary<string, bool>();
        private Vector2 _scrollPos;

        private Dictionary<string, StrategicAiStateComp> _sides;
        private int _selectedSide;

        [MenuItem("Window/AI/Strategic Ai Viewer")]
        private static void Init()
        {
            var window = (StrategicAiWindow) GetWindow(typeof(StrategicAiWindow));
            window.titleContent = new GUIContent("Strategic AI");
            window.Show();
        }

        private void OnGUI()
        {
            if (!Application.isPlaying)
            {
                DrawEditorView();
            }
            else
            {
                _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

                if (_sides == null || _sides.Count <= 0)
                {
                    _sides = ZenBehaviourManager.Instance.Get<StrategicAiStateComp>(ComponentTypes.StrategicAiStateComp)
                        .ToDictionary(s => s.Owner.Wrapper.name, s => s);
                    _selectedSide = 0;
                }

                if (_sides.Count <= 0) 
                {
                    DrawNoSelectionView();
                    EditorGUILayout.EndScrollView();
                    return;
                }

                DrawSideSelectionPopup(_sides.Keys);

                var selectedSide = _sides[_sides.Keys.ElementAt(_selectedSide)];

                DrawSideDebugButtons(selectedSide);
                DrawResourceData(selectedSide.FriendlyTeamId);
                DrawSideData(selectedSide);
                DrawAiInfo(selectedSide);
                DrawAiPlanners(selectedSide);

                EditorGUILayout.EndScrollView();
            }
        }

        private void DrawSideDebugButtons(StrategicAiStateComp selectedSide)
        {
            if (GUILayout.Button("Set DNA to 0"))
            {
                selectedSide.Owner.GetComponent<SidePropertiesComp>().Dna = 0;
            }

            if (GUILayout.Button("Add 100 DNA"))
            {
                selectedSide.Owner.GetComponent<SidePropertiesComp>().Dna += 100;
            }

            if (GUILayout.Button("Set Special DNA to 0"))
            {
                selectedSide.Owner.GetComponent<SidePropertiesComp>().SpecialDna = 0;
            }

            if (GUILayout.Button("Add 100 Special DNA"))
            {
                selectedSide.Owner.GetComponent<SidePropertiesComp>().SpecialDna += 100;
            }

            if (GUILayout.Button("Neutralise All Resource Points"))
            {
                CapAllResourcePoints(0, false);
            }

            if (GUILayout.Button("Enemy Cap All Resource Points"))
            {
                CapAllResourcePoints(selectedSide.EnemyTeamId);
            }

            if (GUILayout.Button("Friendly Cap All Resource Points"))
            {
                CapAllResourcePoints(selectedSide.FriendlyTeamId);
            }
        }

        private void CapAllResourcePoints(int sideId, bool isCapped = true)
        {
            foreach (
                var resource in
                ZenBehaviourManager.Instance.Get<ResourceProductionComp>(ComponentTypes.ResourceProductionComp))
            {
                resource.IsCapped = isCapped;
                resource.CapPercent = isCapped ? 1 : 0;
                resource.OwningSideId = sideId;
            }
        }

        private void DrawSideData(StrategicAiStateComp selectedSide)
        {
            var sideData = selectedSide.Owner.GetComponent<SidePropertiesComp>();

            GUILayout.Label("Side Info", EditorStyles.boldLabel);
            DrawLabel("DNA", $"{sideData.Dna}");
            DrawLabel("Special DNA", $"{sideData.SpecialDna}");
        }

        private void DrawResourceData(int friendlyId)
        {
            var rps = ZenBehaviourManager.Instance.Get<ResourceProductionComp>(ComponentTypes.ResourceProductionComp)
                .DefaultIfEmpty(null)
                .Aggregate(new Tuple3I(0, 0, 0), (acc, item) =>
                {
                    if (item.IsCapped)
                    {
                        if (item.OwningSideId == friendlyId) acc.first++;
                        else acc.third++;
                    }
                    else
                    {
                        acc.second++;
                    }

                    return acc;
                });

            GUILayout.Label("Resource Points", EditorStyles.boldLabel);
            DrawLabel("Owned RPs", $"{rps.first}");
            DrawLabel("Neutral RPs", $"{rps.second}");
            DrawLabel("Enemy RPs", $"{rps.third}");
        }

        private static void DrawNoSelectionView()
        {
            GUILayout.Label("There are no strategic AI available", EditorStyles.boldLabel);
        }

        private static void DrawEditorView()
        {
            GUILayout.Label("AI can only be viewed in play mode", EditorStyles.boldLabel);
        }

        private void DrawSideSelectionPopup(IEnumerable<string> sides)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Side", EditorStyles.boldLabel);
            _selectedSide = EditorGUILayout.Popup(_selectedSide, sides.ToArray());
            EditorGUILayout.EndHorizontal();
        }

        private void DrawAiInfo(StrategicAiStateComp side)
        {
            GUILayout.Label("Balance", EditorStyles.boldLabel);

            var bal = side.DecisionSpace.Influence.GetBalance();
            DrawLabel("Zero", $"{bal.ZeroCount}");
            DrawDoubleLabel("Positive", $"{bal.PositiveCount}", "=>", $"{bal.PositiveSum}");
            DrawDoubleLabel("Negative", $"{bal.NegativeCount}", "=>", $"{bal.NegativeSum}");
            DrawDoubleLabel("Balance", $"{bal.PositiveCount / (float)bal.NegativeCount}", "=>", $"{bal.PositiveSum / (float)bal.NegativeSum}");

            GUILayout.Label("AI Details", EditorStyles.boldLabel);

            DrawLabel("Friendly ID", side.FriendlyTeamId.ToString());
            DrawLabel("Friendly ID", side.EnemyTeamId.ToString());
            DrawLabel("Posture", side.Posture.ToString());
            DrawLabel("Build Order", side.BuildOrder.xJoin());
            DrawLabel("Upgrade Order", side.UpgradeOrder.xJoin());
        }

        private void DrawAiPlanners(StrategicAiStateComp side)
        {
            var context = new AiContext<StrategicAiStateComp>(side.Owner);
            DrawAiPlanner("Posture", context, side.PostureAction, side.PostureActionContainer);
            DrawAiPlanner("Logistics", context, side.Action, side.ActionContainer);
        }

        private void DrawAiPlanner(
            string systemName,
            AiContext<StrategicAiStateComp> context,
            AbstractAiAction<StrategicAiStateComp> action,
            AiActionContainer<StrategicAiStateComp> container)
        {
            DrawLabel($"{systemName} Action", action?.ToString().Replace("GameGHJ.AI.Actions.", ""));
            if (action != null && action.IsTimed)
                DrawLabel("Complete", $"{action.PercentComplete * 100:0.0%}");

            GUILayout.Label($"{systemName} AI Scores", EditorStyles.boldLabel);

            if (container?.ActionBundle == null) return;

            foreach (var act in container.ActionBundle)
            {
                var actionName = act.ToString().Replace("GameGHJ.AI.Actions.", "");

                var actKey = $"{systemName}{actionName}";
                if (!_foldoutData.ContainsKey(actKey))
                {
                    _foldoutData.Add(actKey, false);
                }

                var score = act.GetScore(context);
                _foldoutData[actKey] = EditorGUILayout.Foldout(
                    _foldoutData[actKey],
                    $"[{score:0.0000}] {actionName}");

                if (!_foldoutData[actKey]) continue;

                foreach (var ax in act.Axes)
                {
                    var axScore = ax.Score(context);
                    DrawLabel(
                        "   --> " + ax.ToString().Replace("GameGHJ.AI.Axes.", ""),
                        $"{axScore:0.0000}",
                        100,
                        false);
                }
            }
        }
    }
}