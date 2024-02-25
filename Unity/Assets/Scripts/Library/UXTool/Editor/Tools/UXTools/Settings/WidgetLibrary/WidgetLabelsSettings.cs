#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ThunderFireUITool
{
    //组件标签列表
    [Serializable]
    public class WidgetLabelsSettings
    {
        public List<string> labelList = new List<string>();

        [MenuItem(ThunderFireUIToolConfig.Menu_CreateAssets + "/" + ThunderFireUIToolConfig.WidgetLibrary + "/WidgetLabelsSetting", false, -50)]
        public static void Create()
        {
            JsonAssetManager.CreateAssets<WidgetLabelsSettings>(ThunderFireUIToolConfig.WidgetLabelsPath);
        }

        public void AddNewLabel(string newLabel)
        {
            labelList.Add(newLabel);
            JsonAssetManager.SaveAssets(this);
            OnValueChanged();
        }

        public void RemoveLabel(string label)
        {
            labelList.Remove(label);
            JsonAssetManager.SaveAssets(this);
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