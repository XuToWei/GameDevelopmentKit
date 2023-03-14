using ToolbarExtension;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    public static class FocusFolderToolBar
    {
        private static readonly GUIContent s_UIResFocusGUIContent = new GUIContent("UI-Res", "Focus UI Res Folder.");
#if UNITY_ET
        private static readonly GUIContent s_UIModelCodeFocusGUIContent = new GUIContent("ETUI-Model", "Focus UI Model Code Folder.");
        private static readonly GUIContent s_UIHofixCodeFocusGUIContent = new GUIContent("ETUI-Hotfix", "Focus UI Hotfix Code Folder.");
#endif

        [Toolbar(OnGUISide.Left, -1)]
        static void OnToolbarGUI()
        {
            if (GUILayout.Button(s_UIResFocusGUIContent))
            {
                EditorUtility.FocusProjectWindow();
                Object obj = AssetDatabase.LoadAssetAtPath<Object>("Assets/Res/UI/UIForm");
                Selection.activeObject = obj;
            }
#if UNITY_ET
            else if (GUILayout.Button(s_UIModelCodeFocusGUIContent))
            {
                EditorUtility.FocusProjectWindow();
                Object obj = AssetDatabase.LoadAssetAtPath<Object>("Assets/Scripts/Game/ET/Code/ModelView/Client/Game/UI");
                Selection.activeObject = obj;
            }
            else if (GUILayout.Button(s_UIHofixCodeFocusGUIContent))
            {
                EditorUtility.FocusProjectWindow();
                Object obj = AssetDatabase.LoadAssetAtPath<Object>("Assets/Scripts/Game/ET/Code/HotfixView/Client/Game/UI");
                Selection.activeObject = obj;
            }
#endif
        }
    }
}
