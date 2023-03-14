#if UNITY_HOTFIX
using ToolbarExtension;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace ET.Editor
{
    sealed class CompileDllToolBar_ET
    {
        private const CodeOptimization s_CodeOptimization = CodeOptimization.Debug;
        
        private static readonly GUIContent s_BuildReloadHotfixButtonGUIContent = new GUIContent("Reload ET.Hotfix", "Compile And Reload ET.Hotfix Dll When Playing.");
        private static readonly GUIContent s_BuildHotfixModelButtonGUIContent = new GUIContent("Compile All ET", "Compile All ET Dll.");
        private static bool s_IsReloading = false;

        [Toolbar(OnGUISide.Left, 0)]
        static void OnToolbarGUI()
        {
            EditorGUI.BeginDisabledGroup(!Application.isPlaying || s_IsReloading);
            {
                if (GUILayout.Button(s_BuildReloadHotfixButtonGUIContent))
                {
                    BuildAssemblyHelper.BuildHotfix(s_CodeOptimization, Define.CodeMode);
                    ShowNotification("Build Hotfix Success!");

                    if (s_IsReloading)
                        return;
                    s_IsReloading = true;

                    async void ReloadAsync()
                    {
                        try
                        {
                            await CodeLoaderComponent.Instance.LoadHotfixAsync();
                            EventSystem.Instance.Load();
                        }
                        finally
                        {
                            s_IsReloading = false;
                        }
                    }

                    ReloadAsync();
                }
            }
            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button(s_BuildHotfixModelButtonGUIContent))
            {
                BuildAssemblyHelper.BuildModel(s_CodeOptimization, Define.CodeMode);
                BuildAssemblyHelper.BuildHotfix(s_CodeOptimization, Define.CodeMode);
                ShowNotification("Build Model And Hotfix Success!");
            }
        }

        private static void ShowNotification(string msg)
        {
            Debug.Log(msg);
            EditorWindow game = EditorWindow.GetWindow(typeof(EditorWindow).Assembly.GetType("UnityEditor.GameView"));
            if (game != null) game.ShowNotification(new GUIContent(msg));
        }
    }
}
#endif