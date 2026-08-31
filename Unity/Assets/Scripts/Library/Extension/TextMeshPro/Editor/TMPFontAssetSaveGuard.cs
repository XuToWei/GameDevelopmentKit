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
        private static readonly List<string> s_AllowedAssetPaths = new List<string>();

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

            s_AllowedAssetPaths.Clear();
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

                s_AllowedAssetPaths.Add(path);
            }

            return hasBlockedAsset ? s_AllowedAssetPaths.ToArray() : paths;
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
