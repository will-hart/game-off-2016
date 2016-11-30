// /** 
//  * AbstractAiWindow.cs
//  * Will Hart
//  * 20161115
// */

namespace GameGHJ.Editor
{
    #region Dependencies

    using UnityEditor;
    using UnityEngine;

    #endregion

    public abstract class AbstractAiWindow : EditorWindow
    {
        private float _nextUpdate = 1;

        protected virtual void Update()
        {
            _nextUpdate -= Time.deltaTime;
            if (_nextUpdate > 0) return;
            Repaint();
            _nextUpdate = 1;
        }

        protected void DrawDoubleLabel(string label, string value1, string separator, string value2)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, EditorStyles.miniBoldLabel);
            GUILayout.Box(value1, GUILayout.Width(100));
            GUILayout.Label(separator, GUILayout.Width(50));
            GUILayout.Box(value2, GUILayout.Width(100));
            GUILayout.EndHorizontal();
        }

        protected void DrawLabel(string label, string value, int width = 200, bool isBox = true)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, EditorStyles.miniBoldLabel);
            if (isBox)
            {
                GUILayout.Box(value, GUILayout.Width(width));
            }
            else
            {
                GUILayout.Label(value, GUILayout.Width(width));
            }
            GUILayout.EndHorizontal();
        }
    }
}