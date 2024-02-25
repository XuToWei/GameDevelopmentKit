#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ThunderFireUITool
{
    //最近打开Prefab列表
    [Serializable]
    public class PrefabOpenedSetting
    {
        public List<string> List = new List<string>();

        [MenuItem(ThunderFireUIToolConfig.Menu_CreateAssets + "/" + ThunderFireUIToolConfig.RecentlyOpened + "/PrefabRecentlyOpenedData", false, -45)]
        public static void Create()
        {
            JsonAssetManager.CreateAssets<PrefabOpenedSetting>(ThunderFireUIToolConfig.PrefabRecentOpenedPath);
        }

        //参数：GUID
        public void Add(string newLabel)
        {
            List.Add(newLabel);
            JsonAssetManager.SaveAssets(this);
            OnValueChanged();
        }

        public void Remove(string label)
        {
            var index = List.FindIndex(i => i == label); // like Where/Single
            if (index >= 0)
            {   // ensure item found
                List.RemoveAt(index);
            }
            //List.Remove(label);
            JsonAssetManager.SaveAssets(this);
            OnValueChanged();
        }

        public void ResortLast(string label)
        {
            var index = List.FindIndex(i => i == label);
            if (index >= 0)
            {   // ensure item found
                List.RemoveAt(index);
            }
            List.Add(label);

            JsonAssetManager.SaveAssets(this);
            OnValueChanged();
        }

        private void OnValueChanged()
        {
            if (PrefabRecentWindow.GetInstance() != null)
            {
                PrefabRecentWindow.GetInstance().RefreshWindow();
            }
        }
    }
}
#endif