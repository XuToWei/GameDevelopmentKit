#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace ThunderFireUITool
{
    public class SettingAssetsUtils
    {
        private static Dictionary<Type, string> AssetPathDic = new Dictionary<Type, string>
        {
            {typeof(HierarchyManagementSetting), ThunderFireUIToolConfig.HierarchyManagementSettingPath},
            {typeof(HierarchyManagementData), ThunderFireUIToolConfig.HierarchyManagementDataPath},
#if ODIN_INSPECTOR
            {typeof(UIAnimCheckSetting), ThunderFireUIToolConfig.UICheckAnimFullPath },
            {typeof(UIAtlasCheckRuleSettings), ThunderFireUIToolConfig.UICheckAnimFullPath },
            {typeof(UILegacyComponentSettings), ThunderFireUIToolConfig.UICheckAnimFullPath },
            {typeof(UIAtlasCheckUserData), ThunderFireUIToolConfig.UICheckUserDataFullPath }
#endif
        };

        public static T GetAssets<T>() where T : ScriptableObject
        {
            string path;
            AssetPathDic.TryGetValue(typeof(T), out path);
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError(string.Format("Can't Find {0} 's Asset File.", typeof(T).Name));
                return null;
            }

            T asset = null;

            if (File.Exists(path))
            {
                asset = AssetDatabase.LoadAssetAtPath<T>(path);
            }

            if (asset == null)
            {
                asset = CreateAssets<T>(path);
            }

            return asset;
        }

        public static T CreateAssets<T>(string path) where T : ScriptableObject
        {
            string folder = Path.GetDirectoryName(path);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            T setting = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(setting, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            return asset;
        }
    }
}
#endif
