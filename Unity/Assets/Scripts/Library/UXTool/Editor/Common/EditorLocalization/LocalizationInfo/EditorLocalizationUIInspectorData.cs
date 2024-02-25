using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using ThunderFireUnityEx;

namespace ThunderFireUITool
{
    public class InspectorLocalizationDecode : Editor
    {
#if UXTOOLS_DEV
        [MenuItem(ThunderFireUIToolConfig.Menu_CreateAssets + "/" + ThunderFireUIToolConfig.Menu_UXToolLocalization + "/EditorLocalizationUIInspectorDecode", false, -97)]
#endif
        public static void Decode()
        {
            var jsonText = AssetDatabase.LoadAssetAtPath<TextAsset>(EditorLocalizationConfig.LocalizationUIInspectorJsonPath);
            List<LocalizationUIInspectorData> data = JsonUtilityEx.FromJsonLegacy<LocalizationUIInspectorData>(jsonText.text);
            GenData(data);
            EditorLocalization.RefreshDict();
        }

        private static void GenData(List<LocalizationUIInspectorData> data)
        {
            var folderPath = EditorLocalizationConfig.LocalizationAssetsPath;
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            //var strList = new List<string>();

            foreach (var value in Enum.GetValues(typeof(EditorLocalName)))
            {
                var fileName = EditorLocalizationConfig.LocalizationUIInspectorData + value.ToString() + EditorLocalizationConfig.Jsonsuffix;
                var newData = //ScriptableObject.CreateInstance<EditorLocalizationUIInspectorData>();
                JsonAssetManager.CreateAssets<EditorLocalizationUIInspectorData>(fileName);
                newData.typeList = new List<string>();
                newData.fieldList = new List<string>();
                newData.valueList = new List<string>();
                foreach (var d in data)
                {
                    switch (value)
                    {
                        case EditorLocalName.Chinese:
                            newData.typeList.Add(d.UIComponentType);
                            newData.fieldList.Add(d.FieldName);
                            newData.valueList.Add(d.zhCN);
                            break;
                        case EditorLocalName.English:
                            newData.typeList.Add(d.UIComponentType);
                            newData.fieldList.Add(d.FieldName);
                            newData.valueList.Add(d.EN);
                            break;
                        case EditorLocalName.Japanese:
                            newData.typeList.Add(d.UIComponentType);
                            newData.fieldList.Add(d.FieldName);
                            newData.valueList.Add(d.JAN);
                            break;
                        case EditorLocalName.Korean:
                            newData.typeList.Add(d.UIComponentType);
                            newData.fieldList.Add(d.FieldName);
                            newData.valueList.Add(d.KR);
                            break;
                        case EditorLocalName.TraditionalChinese:
                            newData.typeList.Add(d.UIComponentType);
                            newData.fieldList.Add(d.FieldName);
                            newData.valueList.Add(d.znHans);
                            break;
                    }
                }
                JsonAssetManager.SaveAssetsAtPath<EditorLocalizationUIInspectorData>(newData, fileName);
            }

        }

    }

    [Serializable]
    public class LocalizationUIInspectorData
    {
        public string UIComponentType;
        public string FieldName;
        public string zhCN;
        public string EN;
        public string KR;
        public string JAN;
        public string znHans;
    }

    [Serializable]
    public class EditorLocalizationUIInspectorData
    {
        //private Dictionary<string, string> List = new Dictionary<string, string>();
        public List<string> typeList;
        public List<string> fieldList;
        public List<string> valueList;


        public void RefreshDict()
        {
            EditorLocalization.List.Clear();
            var count = valueList.Count;
            for (var i = 0; i < count; i++)
            {
                if (!EditorLocalization.List.ContainsKey(typeList[i] + fieldList[i]))
                {
                    EditorLocalization.List.Add(typeList[i] + fieldList[i], valueList[i]);
                }
            }
        }

        public string GetValue(string type, string fieldName)
        {

            var count = valueList.Count;
            if (EditorLocalization.List.Count == 0)
            {
                RefreshDict();
            }
            if (EditorLocalization.List.ContainsKey(type + fieldName) && !string.IsNullOrEmpty(EditorLocalization.List[type + fieldName]))
            {
                return EditorLocalization.List[type + fieldName];
            }
            return fieldName;
        }
    }

}

