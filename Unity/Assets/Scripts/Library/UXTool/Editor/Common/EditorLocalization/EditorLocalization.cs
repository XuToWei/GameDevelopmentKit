using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace ThunderFireUITool
{
    public class EditorLocalization
    {
        public static EditorLocalizationSettings currentLocalSettings;
        public static EditorLocalizationData currentLocalizationData;
        public static EditorLocalizationUIInspectorData currentLocalizationUIInspectorData;
        public static Dictionary<string, string> List = new Dictionary<string, string>();
        public static void Clear()
        {
            currentLocalSettings = null;
            currentLocalizationData = null;
            currentLocalizationUIInspectorData = null;
        }
        public static string GetLocalization(long key)
        {
            bool needCheck = false;
            if (currentLocalSettings == null)
            {
                currentLocalSettings = JsonAssetManager.GetAssets<EditorLocalizationSettings>();
                needCheck = true;
            }
            var currentLocal = currentLocalSettings.LocalType;
            var localDataPath = EditorLocalizationConfig.LocalizationData + currentLocal.ToString() + EditorLocalizationConfig.Jsonsuffix;

            if (needCheck || currentLocalizationData == null)
            {
                currentLocalizationData = JsonAssetManager.LoadAssetAtPath<EditorLocalizationData>(localDataPath);
            }
            var strList = currentLocalizationData;
            if (strList == null)
            {
                LocalizationDecode.Decode();
            }
            return strList.GetValue(key);
        }

        public static string GetLocalization(string type, string fieldName)
        {
            bool needCheck = false;
            if (currentLocalSettings == null)
            {
                currentLocalSettings = JsonAssetManager.GetAssets<EditorLocalizationSettings>();
                needCheck = true;
            }
            var currentLocal = currentLocalSettings.LocalType;
            var inspectorDataPath = EditorLocalizationConfig.LocalizationUIInspectorData + currentLocal.ToString() + EditorLocalizationConfig.Jsonsuffix;
            if (needCheck || currentLocalizationUIInspectorData == null)
            {
                currentLocalizationUIInspectorData = JsonAssetManager.LoadAssetAtPath<EditorLocalizationUIInspectorData>(inspectorDataPath);
            }
            var strList = currentLocalizationUIInspectorData;
            if (strList == null)
            {
                InspectorLocalizationDecode.Decode();
            }
            return strList.GetValue(type, fieldName);
        }


        public static void RefreshDict()
        {
            //var currentLocal = EditorLocalizationSettings.GetAssets().LocalType;
            var currentLocal = JsonAssetManager.GetAssets<EditorLocalizationSettings>().LocalType;
            var localDataPath = EditorLocalizationConfig.LocalizationUIInspectorData + currentLocal.ToString() + EditorLocalizationConfig.Jsonsuffix;

            var strList = JsonAssetManager.LoadAssetAtPath<EditorLocalizationUIInspectorData>(localDataPath);
            if (strList == null)
            {
                InspectorLocalizationDecode.Decode();
            }
            strList.RefreshDict();
        }
    }
}
