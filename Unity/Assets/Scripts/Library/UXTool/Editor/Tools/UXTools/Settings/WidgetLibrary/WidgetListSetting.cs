#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ThunderFireUITool
{
    //设为组件的Prefab列表
    [Serializable]
    public class WidgetListSetting
    {
        public List<string> List = new List<string>();
        private static int previewSize = 144;
        [MenuItem(ThunderFireUIToolConfig.Menu_CreateAssets + "/" + ThunderFireUIToolConfig.WidgetLibrary + "/WidgetListSettings", false, -50)]
        public static void Create()
        {

            var setting = JsonAssetManager.CreateAssets<WidgetListSetting>(ThunderFireUIToolConfig.WidgetListPath);
            var guids = AssetDatabase.FindAssets("t:Prefab", new string[] { "Assets/Res/UI/UIPrefab", "Assets/Res/UI/UISprite" });
            foreach (var guid in guids)
            {
                if (!setting.List.Contains(guid))
                    setting.List.Add(guid);
            }
            JsonAssetManager.SaveAssets(setting);
        }

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
            Utils.UpdatePreviewTexture(label, previewSize);
            OnValueChanged();
        }

        private void OnValueChanged()
        {
            if (WidgetRepositoryWindow.GetInstance() != null)
            {
                WidgetRepositoryWindow.GetInstance().RefreshWindow();
            }
        }
    }
}
#endif