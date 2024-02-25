using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace ThunderFireUITool
{
    public class PrefabTabsData
    {
        [SerializeField]
        private List<string> m_tabs;

        public static List<string> Tabs
        {
            get
            {
                var instance = JsonAssetManager.GetAssets<PrefabTabsData>();
                if (instance == null)
                {
                    instance = JsonAssetManager.CreateAssets<PrefabTabsData>(ThunderFireUIToolConfig.PrefabTabsPath);
                    instance.m_tabs = new List<string>();
                }

                if (instance.m_tabs != null)
                {
                    return instance.m_tabs;
                }
                else
                {
                    return new List<string>();
                }
            }
        }

        [MenuItem(ThunderFireUIToolConfig.Menu_CreateAssets + "/" + ThunderFireUIToolConfig.PrefabTabs + "/PrefabTabsData", false, -43)]
        public static void Create()
        {
            JsonAssetManager.CreateAssets<PrefabTabsData>(ThunderFireUIToolConfig.PrefabTabsPath);
        }

        public static void SyncTab(List<string> list)
        {
            var instance = JsonAssetManager.GetAssets<PrefabTabsData>();
            if (instance == null)
            {
                instance = JsonAssetManager.CreateAssets<PrefabTabsData>(ThunderFireUIToolConfig.PrefabTabsPath);
            }
            if (instance.m_tabs == null)
            {
                instance.m_tabs = new List<string>();
            }
            instance.m_tabs.Clear();
            foreach (string s in list)
            {
                instance.m_tabs.Add(s);
            }
            JsonAssetManager.SaveAssets(instance);
        }
    }
}
