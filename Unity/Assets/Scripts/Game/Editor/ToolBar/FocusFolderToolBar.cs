using ToolbarExtension;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    public static class FocusFolderToolBar
    {
        private static readonly GUIContent s_UIResFocusGUIContent = new GUIContent("UI-Res", "Focus UI Res Folder.");

        [Toolbar(OnGUISide.Left, -1)]
        static void OnToolbarGUI()
        {
            if (GUILayout.Button(s_UIResFocusGUIContent))
            {
                EditorUtility.FocusProjectWindow();
                Object obj = AssetDatabase.LoadAssetAtPath<Object>("Assets/Res/UI/UIForm");
                Selection.activeObject = obj;
            }
        }
    }
}
