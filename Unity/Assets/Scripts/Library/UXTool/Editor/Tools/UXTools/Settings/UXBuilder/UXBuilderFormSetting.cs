using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ThunderFireUITool
{
    [Serializable]
    public class UXBuilderFormSetting : ScriptableObject
    {
        public List<UXBuilderFormSettingStruct> List = new List<UXBuilderFormSettingStruct>();

        public void AddNewLabel(UXBuilderFormSettingStruct newLabel)
        {
            List.Add(newLabel);
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            OnValueChanged();
        }

        public void RemoveLabel(UXBuilderFormSettingStruct label)
        {
            var index = List.FindIndex(i => i == label); // like Where/Single
            if (index >= 0)
            {   // ensure item found
                List.RemoveAt(index);
            }
            //List.Remove(label);
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            OnValueChanged();
        }

        public void ResortLastLabel(UXBuilderFormSettingStruct label)
        {
            var index = List.FindIndex(i => i == label);
            if (index >= 0)
            {   // ensure item found
                List.RemoveAt(index);
            }
            List.Add(label);

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            OnValueChanged();
        }

        private void OnValueChanged()
        {
            // if (PrefabRecentWindow2.GetInstance() != null)
            // {
            //     PrefabRecentWindow2.GetInstance().RefreshWindow();
            // }
        }
    }

    public class SettledSettings : Editor
    {
        // [MenuItem(ThunderFireUIToolConfig.Menu_CreateAssets + "/UXBuilderFormSetting")]
        public static void Create()
        {
            var settings = ScriptableObject.CreateInstance<UXBuilderFormSetting>();
            if (settings == null)
                Debug.LogError("Create PrefabLabelsSetting Failed!");

            if (!Directory.Exists(ThunderFireUIToolConfig.WidgetLibrarySettingsPath))
                Directory.CreateDirectory(ThunderFireUIToolConfig.WidgetLibrarySettingsPath);

            var assetPath = ThunderFireUIToolConfig.WidgetLibrarySettingsPath + "UXBuilderFormSetting.asset";
            AssetDatabase.CreateAsset(settings, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    [Serializable]
    public class UXBuilderFormSettingStruct
    {
        public ComponentType componentType;
        public string label;
        public string stringValue;
        public bool boolValue;
        public float floatValue;
        public float minValue = 0f;
        public float maxValue = 100f;
        public List<string> options = new List<string>();

        //protected bool Equals(UXBuilderFormSettingStruct other)
        //{
        //    return label == other.label && componentType == other.componentType && Equals(value, other.value);
        //}
        //
        //public override int GetHashCode()
        //{
        //    return HashCode.Combine(label, (int)componentType, value);
        //}
    }

}
