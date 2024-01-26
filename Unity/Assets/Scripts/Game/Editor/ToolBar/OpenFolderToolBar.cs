using ToolbarExtension;
using UnityEngine;

namespace Game.Editor
{
    sealed class OpenFolderToolBar
    {
        private static readonly GUIContent s_OpenExcelButtonGUIContent = new GUIContent("Open-Excel", "Open Excel Folder!");
        private static readonly GUIContent s_OpenProtoButtonGUIContent = new GUIContent("Open-Proto", "Open Proto Folder!");
        private static readonly GUIContent s_OpenBuildButtonGUIContent = new GUIContent("Open-Build", "Open Build Folder!");
        
        [Toolbar(OnGUISide.Right, -1)]
        private static void OnToolbarGUI()
        {
            if (GUILayout.Button(s_OpenExcelButtonGUIContent))
            {
                OpenFolderTool.OpenExcelPath();
            }
            if (GUILayout.Button(s_OpenProtoButtonGUIContent))
            {
                OpenFolderTool.OpenProtoPath();
            }
            if (GUILayout.Button(s_OpenBuildButtonGUIContent))
            {
                OpenFolderTool.OpenBuildPath();
            }
        }
    }
}
