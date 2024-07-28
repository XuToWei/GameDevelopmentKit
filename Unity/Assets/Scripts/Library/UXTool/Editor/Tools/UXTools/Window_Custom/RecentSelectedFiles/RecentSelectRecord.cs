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
                var recentSelected = JsonAssetManager.GetAssets<RecentFilesSetting>();
                var recentList = recentSelected.List;
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
                    //string path = AssetDatabase.GetAssetPath(obj);
                    string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(obj));
                    GameObject go = obj as GameObject;
                    if (go != null && PrefabUtility.IsPartOfPrefabInstance(go))
                    {
                        Object sourcePrefab = PrefabUtility.GetCorrespondingObjectFromSource(go);
                        guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(sourcePrefab));
                    }
                    if (recentList.Contains(guid))
                    {
                        recentSelected.Remove(guid);
                    }
                    if (guid != "" && !recentList.Contains(guid))
                    {
                        recentSelected.Add(guid);
                    }
                }
                while (recentList.Count > maxRecentSelectedFiles)
                {
                    recentSelected.Remove(recentList.Count - 1);
                }
            }

        }
    }
}
#endif