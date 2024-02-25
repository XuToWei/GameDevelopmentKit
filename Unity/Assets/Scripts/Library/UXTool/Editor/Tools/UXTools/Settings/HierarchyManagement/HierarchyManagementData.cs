using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ThunderFireUITool
{
    [Serializable]
    public class PrefabHierarchyData
    {
        public string Name;
        public string Channel;
        public int Level;
        public string Guid;
    }

    public class HierarchyManagementData
    {
        public List<PrefabHierarchyData> dataList = new List<PrefabHierarchyData>();

        [MenuItem(ThunderFireUIToolConfig.Menu_CreateAssets + "/" + ThunderFireUIToolConfig.HierarchyManage + "/HierarchyManagementData", false, -49)]
        public static void Create()
        {
            JsonAssetManager.CreateAssets<HierarchyManagementData>(
                ThunderFireUIToolConfig.HierarchyManagementDataPath);
        }

        public void Save()
        {
            if (HierarchyManagementEvent.isDemo)
            {
                JsonAssetManager.SaveAssetsAtPath(this, ThunderFireUIToolConfig.HierarchyManagementDataPath_Sample);
            }
            else
            {
                JsonAssetManager.SaveAssets(this);
            }
        }
    }
}