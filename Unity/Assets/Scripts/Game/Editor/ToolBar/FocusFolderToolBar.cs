using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;

namespace Game.Editor
{
    [InitializeOnLoad]
    public static class FocusFolderToolBar
    {
        private static readonly GUIContent s_UIResFocusGUIContent;
        private static readonly GUIContent s_UIModelCodeFocusGUIContent;
        private static readonly GUIContent s_UIHofixCodeFocusGUIContent;
        
        static FocusFolderToolBar()
        {
            s_UIResFocusGUIContent = new GUIContent("UI-Res", "Focus UI Res Folder.");
            s_UIModelCodeFocusGUIContent = new GUIContent("ETUI-Model", "Focus UI Model Code Folder.");
            s_UIHofixCodeFocusGUIContent = new GUIContent("ETUI-Hotfix", "Focus UI Hotfix Code Folder.");
            ToolbarExtender.AddLeftToolbarGUI(-1, OnToolbarGUI);
        }

        static void OnToolbarGUI()
        {
            if (GUILayout.Button(s_UIResFocusGUIContent))
            {
                EditorUtility.FocusProjectWindow();
                Object obj = AssetDatabase.LoadAssetAtPath<Object>("Assets/Res/UI/UIForm");
                Selection.activeObject = obj;
            }
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
        }
    }
}
