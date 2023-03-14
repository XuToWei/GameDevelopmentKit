using ToolbarExtension;
using UnityEditor;
using UnityEngine;

namespace ET.Editor
{
    sealed class ReloadExcelToolBar
    {
        private static readonly GUIContent s_ExportReloadButtonGUIConent = new GUIContent("ReloadExcel", "Reload (No Export) All Excel!");
        private static bool s_IsReloading = false;

        [Toolbar(OnGUISide.Right, 98)]
        private static void OnToolbarGUI()
        {
            EditorGUI.BeginDisabledGroup(!Application.isPlaying || s_IsReloading);
            {
                if (GUILayout.Button(s_ExportReloadButtonGUIConent))
                {
                    if (s_IsReloading)
                        return;
                    s_IsReloading = true;

                    async void ReloadAsync()
                    {
                        try
                        {
                            await ConfigComponent.Instance.ReloadAllAsync();
                            EventSystem.Instance.Load();
                            ShowNotification("Export And Reload All Excel!");
                        }
                        finally
                        {
                            s_IsReloading = false;
                        }
                    }

                    ReloadAsync();
                }
            }
            EditorGUI.EndDisabledGroup();
        }

        private static void ShowNotification(string msg)
        {
            Debug.Log(msg);
            EditorWindow game = EditorWindow.GetWindow(typeof(EditorWindow).Assembly.GetType("UnityEditor.GameView"));
            if (game != null) game.ShowNotification(new GUIContent(msg));
        }
    }
}
