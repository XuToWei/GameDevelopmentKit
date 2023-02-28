using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;

namespace Game.Editor
{
    [InitializeOnLoad]
    sealed class ExcelExporterToolBar
    {
        private static GUIContent s_ExportButtonGUIConent;

        static ExcelExporterToolBar()
        {
            s_ExportButtonGUIConent = new GUIContent("ExportExcel", "Export All Excel!");
            ToolbarExtender.AddRightToolbarGUI(99, OnToolbarGUI);
        }

        private static void OnToolbarGUI()
        {
            if (GUILayout.Button(s_ExportButtonGUIConent))
            {
                ToolEditor.ExcelExporter();
                ShowNotification("Export All Excel!");
            }
        }

        private static void ShowNotification(string msg)
        {
            EditorWindow game = EditorWindow.GetWindow(typeof(EditorWindow).Assembly.GetType("UnityEditor.GameView"));
            if (game != null) game.ShowNotification(new GUIContent(msg));
        }
    }
}
