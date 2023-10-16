#if UNITY_HOTFIX
using Cysharp.Threading.Tasks;
using ToolbarExtension;
using UnityEditor;
using UnityEngine;

namespace ET.Editor
{
    sealed class CompileDllToolBar_ET
    {
        private static readonly GUIContent s_BuildReloadHotfixButtonGUIContent = new GUIContent("ETReload", "Compile And Reload ET.Hotfix Dll When Playing.");
        private static readonly GUIContent s_BuildHotfixModelButtonGUIContent = new GUIContent("ETCompile", "Compile All ET Dll.");
        private static bool s_IsReloading = false;

        [Toolbar(OnGUISide.Left, 0)]
        static void OnToolbarGUI()
        {
            EditorGUI.BeginDisabledGroup(!Application.isPlaying || s_IsReloading);
            {
                if (GUILayout.Button(s_BuildReloadHotfixButtonGUIContent))
                {
                    BuildAssemblyTool.Build();
                    Debug.Log("compile success!");

                    if (s_IsReloading)
                        return;
                    s_IsReloading = true;

                    async UniTaskVoid ReloadAsync()
                    {
                        try
                        {
                            await CodeLoaderComponent.Instance.ReloadAsync();
                            Debug.Log("reload hotfix success!");
                        }
                        finally
                        {
                            s_IsReloading = false;
                        }
                    }

                    ReloadAsync().Forget();
                }
            }
            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button(s_BuildHotfixModelButtonGUIContent))
            {
                BuildAssemblyTool.Build();
                Debug.Log("compile success!");
            }
        }
    }
}
#endif