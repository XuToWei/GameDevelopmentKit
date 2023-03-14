using ToolbarExtension;
using UnityEngine;

namespace ET.Editor
{
    sealed class CodeCreatorToolBar_ET
    {
        private static readonly GUIContent s_OpenETCodeCreatorTool = new GUIContent("ETCodeCreator", "Open ET Code Creator Tool.");

        [Toolbar(OnGUISide.Left, 0)]
        static void OnToolbarGUI()
        {
            if (GUILayout.Button(s_OpenETCodeCreatorTool))
            {
                CodeCreatorEditor.ShowWindow();
            }
        }
    }
}
