#if UNITY_HOTFIX
using ToolbarExtension;
using UnityEngine;

namespace Game.Hot.Editor
{
    internal sealed class CompileDllToolBar_UGFHotfix
    {
        private static readonly GUIContent s_ButtonGUIContent = new GUIContent("HotCompile", "Compile GameHot Dll.");

        [Toolbar(OnGUISide.Left, 50)]
        static void OnToolbarGUI()
        {
            if (GUILayout.Button(s_ButtonGUIContent))
            {
                BuildAssemblyTool.Build();
            }
        }
    }
}
#endif