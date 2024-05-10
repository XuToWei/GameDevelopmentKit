using Cysharp.Threading.Tasks;
using ToolbarExtension;
using UnityEditor;
using UnityEngine;

namespace ET.Editor
{
    sealed class ReloadExcelToolBar
    {
        private static readonly GUIContent s_ExportReloadButtonGUIConent = new GUIContent("ReloadExcel", "Reload (No Export) All Excel!");
        private static bool s_IsReloading = false;

        [Toolbar(OnGUISide.Right, 98)]
        private static void OnToolbarGUI()
        {
            EditorGUI.BeginDisabledGroup(!Application.isPlaying || s_IsReloading);
            {
                if (GUILayout.Button(s_ExportReloadButtonGUIConent))
                {
                    if (s_IsReloading)
                        return;
                    s_IsReloading = true;

                    async UniTaskVoid ReloadAsync()
                    {
                        try
                        {
                            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                            await ConfigComponent.Instance.ReloadAllAsync();
                            Debug.Log("Export And Reload All Excel!");
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
        }
    }
}
