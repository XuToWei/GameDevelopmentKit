using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;

namespace Game.Editor
{
    [InitializeOnLoad]
    sealed class OpenFolderToolBar
    {
        private static readonly GUIContent s_OpenExcelButtonGUIConent;
        private static readonly GUIContent s_OpenProtoButtonGUIConent;
        private static readonly GUIContent s_OpenBuildButtonGUIConent;

        static OpenFolderToolBar()
        {
            s_OpenExcelButtonGUIConent = new GUIContent("Open-Excel", "Open Excel Folder!");
            s_OpenProtoButtonGUIConent = new GUIContent("Open-Proto", "Open Proto Folder!");
            s_OpenBuildButtonGUIConent = new GUIContent("Open-Build", "Open Build Folder!");
            ToolbarExtender.AddRightToolbarGUI(0, OnToolbarGUI);
        }

        private static void OnToolbarGUI()
        {
            if (GUILayout.Button(s_OpenExcelButtonGUIConent))
            {
                OpenFolderTool.OpenExcelPath();
            }
            if (GUILayout.Button(s_OpenProtoButtonGUIConent))
            {
                OpenFolderTool.OpenProtoPath();
            }
            if (GUILayout.Button(s_OpenBuildButtonGUIConent))
            {
                OpenFolderTool.OpenBuildPath();
            }
        }
    }
}
