using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace TMPro.Extension.Editor
{
    internal sealed class TMPFontAssetSaveGuard : AssetModificationProcessor
    {
        private static readonly HashSet<string> s_ReportedAssetPaths = new HashSet<string>();
        private static readonly List<string> s_AllowedAssetPaths = new List<string>();

        public static string[] OnWillSaveAssets(string[] paths)
        {
            s_AllowedAssetPaths.Clear();
            bool hasBlockedAsset = false;
            for (int i = 0; i < paths.Length; i++)
            {
                string path = paths[i];
                Type mainAssetType = AssetDatabase.GetMainAssetTypeAtPath(path);
                if (mainAssetType != null && typeof(TMP_FontAsset).IsAssignableFrom(mainAssetType))
                {
                    TMP_FontAsset fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
                    if (fontAsset == null || !TMPFontAssetEditLock.IsUnlocked(fontAsset))
                    {
                        hasBlockedAsset = true;
                        if (s_ReportedAssetPaths.Add(path))
                        {
                            Debug.LogWarning($"TMP Font Asset 已锁定，无法保存；请先在 Inspector 中解锁：{path}");
                        }

                        continue;
                    }

                    s_ReportedAssetPaths.Remove(path);
                }

                s_AllowedAssetPaths.Add(path);
            }

            return hasBlockedAsset ? s_AllowedAssetPaths.ToArray() : paths;
        }
    }
}
