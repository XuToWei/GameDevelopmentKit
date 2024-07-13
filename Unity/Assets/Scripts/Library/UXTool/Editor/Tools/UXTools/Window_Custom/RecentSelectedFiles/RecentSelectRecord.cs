#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ThunderFireUITool
{
    [UXInitialize(100)]
    public class RecentSelectRecord
    {
        private static UXToolCommonData commonData;
        private static int maxRecentSelectedFiles;

        static RecentSelectRecord()
        {
            Selection.selectionChanged += UpdateRecentFiles;
        }


        public static void UpdateRecentFiles()
        {
            if (SwitchSetting.CheckValid(SwitchSetting.SwitchType.RecentlySelected))
            {
                var savePath = JsonAssetManager.GetAssets<RecentFilesSetting>();
                var recentFilePaths = savePath.Paths;
                commonData = AssetDatabase.LoadAssetAtPath<UXToolCommonData>(ThunderFireUIToolConfig.UXToolCommonDataPath);
                if (commonData != null)
                {
                    maxRecentSelectedFiles = commonData.MaxRecentSelectedFiles;
                }
                else
                {
                    maxRecentSelectedFiles = 15;
                }
                foreach (Object obj in Selection.objects)
                {
                    string path = AssetDatabase.GetAssetPath(obj);
                    GameObject go = obj as GameObject;
                    if (go != null && PrefabUtility.IsPartOfPrefabInstance(go))
                    {
                        Object sourcePrefab = PrefabUtility.GetCorrespondingObjectFromSource(go);
                        path = AssetDatabase.GetAssetPath(sourcePrefab);
                    }
                    if (recentFilePaths.Contains(path))
                    {
                        savePath.Remove(path);
                    }
                    if (path != "" && !recentFilePaths.Contains(path))
                    {
                        savePath.Add(path);
                    }
                }
                while (recentFilePaths.Count > maxRecentSelectedFiles)
                {
                    savePath.Remove(recentFilePaths.Count - 1);
                }
            }

        }
    }
}
#endif