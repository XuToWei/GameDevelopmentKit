using ToolbarExtension;
using UnityEngine;

namespace Game.Editor
{
    sealed class OpenFolderToolBar
    {
        private static readonly GUIContent s_OpenExcelButtonGUIConent = new GUIContent("Open-Excel", "Open Excel Folder!");
        private static readonly GUIContent s_OpenProtoButtonGUIConent = new GUIContent("Open-Proto", "Open Proto Folder!");
        private static readonly GUIContent s_OpenBuildButtonGUIConent = new GUIContent("Open-Build", "Open Build Folder!");
        
        [Toolbar(OnGUISide.Right, -1)]
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
