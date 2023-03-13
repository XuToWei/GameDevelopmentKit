using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;

namespace ET.Editor
{
    [InitializeOnLoad]
    sealed class ReloadExcelToolBar
    {
        private static readonly GUIContent s_ExportReloadButtonGUIConent;
        private static bool s_IsReloading;
        
        static ReloadExcelToolBar()
        {
            s_ExportReloadButtonGUIConent = new GUIContent("ReloadExcel", "Reload (No Export) All Excel!");
            s_IsReloading = false;
            ToolbarExtender.AddRightToolbarGUI(98, OnToolbarGUI);
        }

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
