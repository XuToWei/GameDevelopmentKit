using FastScriptReload.Editor;
using ToolbarExtension;
using UnityEngine;

namespace Game.Editor
{
    public static class FastScriptReloadTool
    {
        private static readonly GUIContent s_ReloadButtonGUIConent = new GUIContent("↻", "FastScriptReload Trigger Reload For Changed Files!");

        [Toolbar(OnGUISide.Left, 99)]
        private static void OnToolbarGUI()
        {
            if (GUILayout.Button(s_ReloadButtonGUIConent))
            {
                FastScriptReloadManager.Instance.TriggerReloadForChangedFiles();
            }
        }
    }
}