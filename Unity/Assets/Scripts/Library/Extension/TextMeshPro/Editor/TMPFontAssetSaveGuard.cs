using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace TMPro.Extension.Editor
{
    [InitializeOnLoad]
    internal sealed class TMPFontAssetSaveGuard : AssetModificationProcessor
    {
        private static readonly HashSet<string> s_ReportedAssetPaths = new HashSet<string>();

        static TMPFontAssetSaveGuard()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        public static string[] OnWillSaveAssets(string[] paths)
        {
            if (!TMPFontAssetPlayModeLock.IsActive)
            {
                return paths;
            }

            List<string> allowedPaths = new List<string>(paths.Length);
            bool hasBlockedAsset = false;
            for (int i = 0; i < paths.Length; i++)
            {
                string path = paths[i];
                Type mainAssetType = AssetDatabase.GetMainAssetTypeAtPath(path);
                if (mainAssetType != null && typeof(TMP_FontAsset).IsAssignableFrom(mainAssetType))
                {
                    hasBlockedAsset = true;
                    if (s_ReportedAssetPaths.Add(path))
                    {
                        Debug.LogWarning($"运行模式下禁止保存 TMP Font Asset：{path}");
                    }

                    continue;
                }

                allowedPaths.Add(path);
            }

            return hasBlockedAsset ? allowedPaths.ToArray() : paths;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredEditMode)
            {
                s_ReportedAssetPaths.Clear();
            }
        }
    }
}
