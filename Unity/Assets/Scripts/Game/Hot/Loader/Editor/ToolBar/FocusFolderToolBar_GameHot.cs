using ToolbarExtension;
using UnityEditor;
using UnityEngine;

namespace Game.Hot.Editor
{
    public static class FocusFolderToolBar_GameHot
    {
        private static readonly GUIContent s_HotRuntimeFocusGUIContent = new GUIContent("Hot-Runtime", "Focus Hot Code Runtime Folder.");
        private static readonly GUIContent s_HotUIFocusGUIContent = new GUIContent("Hot-UI", "Focus Hot Code UI Folder.");

        [Toolbar(OnGUISide.Left, -2)]
        static void OnToolbarGUI()
        {
            if (GUILayout.Button(s_HotRuntimeFocusGUIContent))
            {
                EditorUtility.FocusProjectWindow();
                Object obj = AssetDatabase.LoadAssetAtPath<Object>("Assets/Scripts/Game/Hot/Code");
                Selection.activeObject = obj;
            }
            
            if (GUILayout.Button(s_HotUIFocusGUIContent))
            {
                EditorUtility.FocusProjectWindow();
                Object obj = AssetDatabase.LoadAssetAtPath<Object>("Assets/Scripts/Game/Hot/Code/UI");
                Selection.activeObject = obj;
            }
        }
    }
}