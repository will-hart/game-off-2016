// /** 
//  * TacticalAiWindow.cs
//  * Will Hart
//  * 20161115
// */

namespace GameGHJ.Editor
{
    #region Dependencies

    using System.Collections.Generic;
    using GameGHJ.AI.Core;
    using GameGHJ.Systems;
    using UnityEditor;
    using UnityEngine;

    #endregion

    public class TacticalAiWindow : AbstractAiWindow
    {
        private readonly Dictionary<string, bool> _foldoutData = new Dictionary<string, bool>();
        private Vector2 _scrollPos;

        [MenuItem("Window/AI/Tactical Ai Viewer")]
        private static void Init()
        {
            var window = (TacticalAiWindow) GetWindow(typeof(TacticalAiWindow));
            window.titleContent = new GUIContent("Tactical AI");
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
                var selected = PlayerControlSystem.Instance.selectedObjects;

                if ((selected == null) ||
                    (selected.Count == 0))
                {
                    DrawNoSelectionView();
                }
                else
                {
                    DrawSelectionView(selected[0]);
                }
            }
        }

        private void DrawSelectionView(ComponentECS selected)
        {
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            var creep = selected.Owner;
            DrawCreepInfo(selected.Owner);
            if (creep.HasComponent(ComponentTypes.TacticalAiStateComp)) DrawAiInfo(creep);
            EditorGUILayout.EndScrollView();
        }

        private void DrawCreepInfo(Entity selected)
        {
            GUILayout.Label("Creep Data", EditorStyles.boldLabel);
            var health = selected.GetComponent<HealthComp>();
            DrawDoubleLabel("Health", $"{health.currentHealth}", " / ", $"{health.maxHealth}");
        }

        private static void DrawNoSelectionView()
        {
            GUILayout.Label("Select an Entity to continue", EditorStyles.boldLabel);
        }

        private static void DrawEditorView()
        {
            GUILayout.Label("AI can only be viewed in play mode", EditorStyles.boldLabel);
        }

        private void DrawAiInfo(Entity creep)
        {
            GUILayout.Label("AI State", EditorStyles.boldLabel);

            var data = creep.GetComponent<TacticalAiStateComp>();
            DrawLabel("Navigation Target", data.NavigationTarget.ToString());
            DrawLabel("Current Action", data.Action.ToString().Replace("GameGHJ.AI.Actions.", ""));
            if (data.Action.IsTimed) DrawLabel("Complete", $"{data.Action.PercentComplete*100:0.0%}");

            GUILayout.Label("AI Scores", EditorStyles.boldLabel);

            var context = new AiContext<TacticalAiStateComp>(creep);

            if (data.ActionContainer?.ActionBundle == null) return;

            foreach (var action in data.ActionContainer.ActionBundle)
            {
                var actionName = action.ToString().Replace("GameGHJ.AI.Actions.", "");

                if (!_foldoutData.ContainsKey(actionName))
                {
                    _foldoutData.Add(actionName, false);
                }

                var score = action.GetScore(context);
                _foldoutData[actionName] = EditorGUILayout.Foldout(
                    _foldoutData[actionName],
                    $"[{score:0.0000}] {actionName}");

                if (!_foldoutData[actionName]) continue;
                foreach (var ax in action.Axes)
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