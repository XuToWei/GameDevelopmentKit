using FastScriptReload.Editor;
using ToolbarExtension;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    public static class FastScriptReloadToolBar
    {
        private static readonly GUIContent s_ReloadButtonGUIConent = new GUIContent("↻", "FastScriptReload Trigger Reload For Changed Files!");

        [Toolbar(OnGUISide.Left, 99)]
        private static void OnToolbarGUI()
        {
            EditorGUI.BeginDisabledGroup(!Application.isPlaying);
            {
                if (GUILayout.Button(s_ReloadButtonGUIConent))
                {
                    FastScriptReloadManager.Instance.TriggerReloadForChangedFiles();
                }
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}