using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using ThunderFireUnityEx;

namespace ThunderFireUITool
{
    public class LocalizationDecode : Editor
    {
#if UXTOOLS_DEV
        [MenuItem(ThunderFireUIToolConfig.Menu_CreateAssets + "/" + ThunderFireUIToolConfig.Menu_UXToolLocalization + "/EditorLocalizationDecode", false, -97)]
#endif
        public static void Decode()
        {
            var jsonText = AssetDatabase.LoadAssetAtPath<TextAsset>(EditorLocalizationConfig.LocalizationJsonPath);
            List<LocalizationMetaData> data = JsonUtilityEx.FromJsonLegacy<LocalizationMetaData>(jsonText.text);
            GenData(data);
        }
#if UXTOOLS_DEV
        [MenuItem(ThunderFireUIToolConfig.Menu_CreateAssets + "/" + ThunderFireUIToolConfig.Menu_UXToolLocalization + "/EditorLocalizationStorage", false, -97)]
#endif
        public static void BuildUIScript()
        {
            var jsonText = AssetDatabase.LoadAssetAtPath<TextAsset>(EditorLocalizationConfig.LocalizationJsonPath);
            List<LocalizationMetaData> data = JsonUtilityEx.FromJsonLegacy<LocalizationMetaData>(jsonText.text);

            var listPath = EditorLocalizationConfig.EditorLocalizationStoragePath;
            var memberStr = "";
            var count = data.Count;
            for (var i = 0; i < count; i++)
            {
                memberStr += "public static readonly long Def_" + data[i].description + $" = {data[i].key}" + ";\r\n\t\t";
            }
            var classStr = EditorLocalizationConfig.EditorLocalizationStorageCode;
            classStr = classStr.Replace("#成员#", memberStr);
            CreateScript(listPath, classStr);
        }

        private static void CreateScript(string scriptPath, string code)
        {
            if (File.Exists(scriptPath))
                File.Delete(scriptPath);
            var file = new FileStream(scriptPath, FileMode.CreateNew);
            var fileW = new StreamWriter(file, System.Text.Encoding.UTF8);
            fileW.Write(code);
            fileW.Flush();
            fileW.Close();
            file.Close();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void GenData(List<LocalizationMetaData> data)
        {
            var folderPath = EditorLocalizationConfig.LocalizationAssetsPath;
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            var strList = new List<string>();

            foreach (var value in Enum.GetValues(typeof(EditorLocalName)))
            {
                var fileName = EditorLocalizationConfig.LocalizationData + value.ToString() + EditorLocalizationConfig.Jsonsuffix;
                var newData = //ScriptableObject.CreateInstance<EditorLocalizationData>();
                JsonAssetManager.CreateAssets<EditorLocalizationData>(fileName);
                newData.keyList = new List<long>();
                newData.valueList = new List<string>();
                foreach (var d in data)
                {
                    newData.keyList.Add(d.key);
                    switch (value)
                    {
                        case EditorLocalName.Chinese:
                            newData.valueList.Add(d.zhCN);
                            break;
                        case EditorLocalName.English:
                            newData.valueList.Add(d.EN);
                            break;
                        case EditorLocalName.Japanese:
                            newData.valueList.Add(d.JAN);
                            break;
                        case EditorLocalName.Korean:
                            newData.valueList.Add(d.KR);
                            break;
                        case EditorLocalName.TraditionalChinese:
                            newData.valueList.Add(d.znHans);
                            break;
                    }
                }
                JsonAssetManager.SaveAssetsAtPath<EditorLocalizationData>(newData, fileName);
            }
        }
    }

    //多语言元数据，需要后续更新
    [Serializable]
    public class LocalizationMetaData
    {
        public string description;
        public long key;
        public string zhCN;
        public string EN;
        public string KR;
        public string JAN;
        public string znHans;
    }

    [Serializable]
    public class EditorLocalizationData
    {
        public List<long> keyList;
        public List<string> valueList;

        public string GetValue(long key)
        {
            var count = keyList.Count;
            for (var i = 0; i < count; i++)
            {
                if (keyList[i] == key)
                {
                    return valueList[i];
                }
            }
            return null;
        }
    }

}
