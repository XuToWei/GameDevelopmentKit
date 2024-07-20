using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using ThunderFireUnityEx;
using ThunderFireUITool;

[Serializable]
public class JsonAssetManager
{
    [Serializable]
    public class AssetPath
    {
        public Dictionary<string, string> data;
    }


    private static string metapath = ThunderFireUIToolConfig.AssetsRootPath + "JsonAssetManager.json";

    [SerializeField]
    private static Dictionary<string, string> AssetPathDic = new Dictionary<string, string> { };
    public static void InitMetaData()
    {
        AssetPathDic = new Dictionary<string, string>()
        {
            { "EditorLocalizationSettings",             EditorLocalizationConfig.LocalizationSettingsFullPath },
            { "EditorLocalizationData",                 EditorLocalizationConfig.LocalizationJsonPath},
            { "EditorLocalizationUIInspectorData",      EditorLocalizationConfig.LocalizationUIInspectorJsonPath},

            { "PrefabTabsData",                         ThunderFireUIToolConfig.PrefabTabsPath},
            { "LocationLinesData",                      ThunderFireUIToolConfig.LocationLinesDataPath},
            { "PrefabOpenedSetting",                    ThunderFireUIToolConfig.PrefabRecentOpenedPath},
            { "SwitchSetting",                          ThunderFireUIToolConfig.SwitchSettingPath },
            { "QuickBackgroundData",                    ThunderFireUIToolConfig.QuickBackgroundDataPath },
            { "UIColorAsset",                           UIColorConfig.ColorConfigPath + UIColorConfig.ColorConfigName + ".json" },
            { "UIGradientAsset",                        UIColorConfig.ColorConfigPath + UIColorConfig.GradientConfigName + ".json" },
            { "WidgetLabelsSettings",                   ThunderFireUIToolConfig.WidgetLabelsPath },
            { "WidgetListSetting",                      ThunderFireUIToolConfig.WidgetListPath },
            { "HierarchyManagementSetting",             ThunderFireUIToolConfig.HierarchyManagementSettingPath },
            { "HierarchyManagementEditorData",          ThunderFireUIToolConfig.HierarchyManagemenEditorDataPath },
            { "HierarchyManagementData",                ThunderFireUIToolConfig.HierarchyManagementDataPath },
            { "RecentFilesSetting",                     ThunderFireUIToolConfig.FilesRecentSelectedPath},
            { "ToolGlobalData",                         ThunderFireUIToolConfig.GlobalDataPath},
        };
    }

    public static string GetAssetsPath<T>() where T : class
    {
        if (AssetPathDic.Count == 0)
        {
            InitMetaData();
        }

        string path = "";
        if (AssetPathDic.ContainsKey(typeof(T).Name))
        {
            path = AssetPathDic[typeof(T).Name];
        }
        return path;
    }

    // 将Json文件的数据读取到内存
    public static T GetAssets<T>() where T : class
    {
        string path = "";
        InitMetaData();
        if (AssetPathDic.ContainsKey(typeof(T).Name))
        {
            path = AssetPathDic[typeof(T).Name];
        }
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogFormat("Can't Find {0} 's Json File, Make Sure the File Exists", typeof(T).Name);
            return null;
        }

        T asset = LoadAssetAtPath<T>(path);

        if (asset == null)
        {
            Debug.LogFormat("Can't Load {0} , Please Check the File", path);
            asset = CreateAssets<T>(path);
        }

        return asset;
    }
    //将外部的Json文件的数据读取到内存中（不会保存到Meta中）
    public static T LoadAssetAtPath<T>(string path) where T : class
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError(string.Format("Can't Find {0} 's Json File. Please Create it First.", typeof(T).Name));
            return null;
        }

        T asset = FromJson<T>(path);
        return asset;
    }
    public static T FromJson<T>(string path) where T : class
    {
        if (File.Exists(path))
        {
            T asset = null;

            string dataAsJson = File.ReadAllText(path);
            if (typeof(ScriptableObject).IsAssignableFrom(typeof(T)))
            {
                asset = ScriptableObject.CreateInstance(typeof(T)) as T;
                JsonUtility.FromJsonOverwrite(dataAsJson, asset);
            }
            else
            {
                asset = JsonUtility.FromJson<T>(dataAsJson);
            }

            return asset;
        }
        return null;
    }


    public static T CreateAssets<T>() where T : class
    {
        string path = GetAssetsPath<T>();
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError(string.Format("Can't Find {0} 's Json File. Please Create it First.", typeof(T).Name));
            return null;
        }

        return (T)CreateAssets(path, typeof(T));
    }

    public static T CreateAssets<T>(string path) where T : class
    {
        return (T)CreateAssets(path, typeof(T));
    }
    public static object CreateAssets(string path, Type type)
    {
        object newObject = Activator.CreateInstance(type);

        string folder = Path.GetDirectoryName(path);
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        string dataAsJson = JsonUtility.ToJson(newObject);
        File.WriteAllText(path, dataAsJson);
        return newObject;
    }

    public static void SaveAssets<T>(T obj) where T : class
    {
        string path = "";
        if (AssetPathDic.ContainsKey(typeof(T).Name))
        {
            path = AssetPathDic[typeof(T).Name];
        }
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
        {
            Debug.LogError(string.Format("Can't Find {0} 's Json File. Please Create it First.", typeof(T).Name));
            return;
        }

        string dataAsJson = JsonUtility.ToJson(obj);
        File.WriteAllText(path, dataAsJson);
        return;
    }
    public static void SaveAssetsAtPath<T>(T obj, string path) where T : class
    {
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
        {
            Debug.LogError(string.Format("Can't Find {0} 's Json File. Please Create it First.", typeof(T).Name));
            return;
        }

        string dataAsJson = JsonUtility.ToJson(obj);
        File.WriteAllText(path, dataAsJson);
        return;
    }
}
