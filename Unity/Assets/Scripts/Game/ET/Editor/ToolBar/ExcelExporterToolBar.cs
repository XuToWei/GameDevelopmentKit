using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;

namespace ET.Editor
{
    [InitializeOnLoad]
    sealed class ExcelExporterToolBar
    {
        private static GUIContent s_ExportButtonGUIContent;
        private static GUIContent s_ExportReloadButtonGUIConent;
        private static bool s_IsReloading;
        
        static ExcelExporterToolBar()
        {
            s_ExportButtonGUIContent = new GUIContent("ExportExcel", "Export All Excel!");
            s_ExportReloadButtonGUIConent = new GUIContent("ReloadExcel", "Export And Reload All Excel!");
            ToolbarExtender.AddLeftToolbarGUI(0, OnToolbarGUI);
        }

        private static void OnToolbarGUI()
        {
            if (GUILayout.Button(s_ExportButtonGUIContent))
            {
                ToolsEditor.ExcelExporter();
            }
            EditorGUI.BeginDisabledGroup(!Application.isPlaying || s_IsReloading);
            {
                if (GUILayout.Button(s_ExportReloadButtonGUIConent))
                {
                    if (s_IsReloading)
                        return;
                    s_IsReloading = true;
                    
                    ToolsEditor.ExcelExporter();
                    
                    async void ReloadAsync()
                    {
                        // try
                        // {
                        //     foreach (var VARIABLE in COLLECTION)
                        //     {
                        //         
                        //     }
                        //     await ConfigComponent.Instance.LoadOneAsync();
                        //     EventSystem.Instance.Load();
                        // }
                        // finally
                        // {
                        //     s_IsReloading = false;
                        // }
                    }
                    
                    ShowNotification("Export And Reload All Excel!");
                }
            }
        }

        private static void ShowNotification(string msg)
        {
            EditorWindow game = EditorWindow.GetWindow(typeof(EditorWindow).Assembly.GetType("UnityEditor.GameView"));
            if (game != null) game.ShowNotification(new GUIContent(msg));
        }
    }
}
