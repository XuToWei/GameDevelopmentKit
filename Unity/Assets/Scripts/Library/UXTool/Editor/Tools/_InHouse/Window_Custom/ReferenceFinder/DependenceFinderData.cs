#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace ThunderFireUITool
{
    public class DependenceFinderData
    {
        public string Name = "";
        public string FilePath = "";
        public readonly List<string> Dependencies = new List<string>();

        public void CollectDependenciesInfo(string assetPath)
        {
            try
            {
                var basePath = Application.dataPath.Replace("/Assets", ""); // 磁盘符开头的项目路径
                var guidRegex = new Regex("guid: ([a-z0-9]{32})", RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
                var fileName = $"{basePath}/{assetPath}";
                if (!File.Exists(fileName)) return;

                var fileStr = PrefabYamlUtils.FilterNestPrefabInstanceContent(fileName);
                var guids = guidRegex.Matches(fileStr);

                Name = Path.GetFileNameWithoutExtension(assetPath);
                FilePath = assetPath;
                Dependencies.Clear();
                foreach (Match i in guids)
                {
                    if (Dependencies.Contains(i.Groups[1].Value.ToLower())) continue;
                    Dependencies.Add(i.Groups[1].Value.ToLower());
                }

            }
            catch (Exception e)
            {
                Debug.LogError(e);
                EditorUtility.ClearProgressBar();
            }
        }
    }
}
#endif