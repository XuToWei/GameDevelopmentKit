using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;

namespace ET.Editor
{
    [InitializeOnLoad]
    sealed class CodeCreatorToolBar_ET
    {
        private static readonly GUIContent s_OpenETCodeCreatorTool;
        
        static CodeCreatorToolBar_ET()
        {
            s_OpenETCodeCreatorTool = new GUIContent("ETCodeCreator", "Open ET Code Creator Tool.");
            ToolbarExtender.AddLeftToolbarGUI(0, OnToolbarGUI);
        }

        static void OnToolbarGUI()
        {
            if (GUILayout.Button(s_OpenETCodeCreatorTool))
            {
                CodeCreatorEditor.ShowWindow();
            }
        }
    }
}
