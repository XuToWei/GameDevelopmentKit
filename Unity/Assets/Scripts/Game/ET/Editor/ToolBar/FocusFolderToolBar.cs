using ToolbarExtension;
using UnityEditor;
using UnityEngine;

namespace ET.Editor
{
    public static class ETFocusFolderToolBar
    {
        private static readonly GUIContent s_UIModelCodeFocusGUIContent = new GUIContent("ETUI-Model", "Focus UI Model Code Folder.");
        private static readonly GUIContent s_UIHofixCodeFocusGUIContent = new GUIContent("ETUI-Hotfix", "Focus UI Hotfix Code Folder.");

        [Toolbar(OnGUISide.Left, -2)]
        static void OnToolbarGUI()
        {
            if (GUILayout.Button(s_UIModelCodeFocusGUIContent))
            {
                EditorUtility.FocusProjectWindow();
                var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>("Assets/Scripts/Game/ET/Code/ModelView/Client/Game/UI");
                Selection.activeObject = obj;
            }
            if (GUILayout.Button(s_UIHofixCodeFocusGUIContent))
            {
                EditorUtility.FocusProjectWindow();
                var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>("Assets/Scripts/Game/ET/Code/HotfixView/Client/Game/UI");
                Selection.activeObject = obj;
            }
        }
    }
}
