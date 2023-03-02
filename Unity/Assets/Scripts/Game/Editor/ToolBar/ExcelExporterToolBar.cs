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
            }
        }
    }
}
