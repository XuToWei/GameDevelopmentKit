#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ThunderFireUITool
{
    public class UXToolCommonData : ScriptableObject
    {
        [MenuItem(ThunderFireUIToolConfig.Menu_CreateAssets + "/" + ThunderFireUIToolConfig.CommonData, false, -98)]
        public static UXToolCommonData Create()
        {
            var settings = CreateInstance<UXToolCommonData>();
            if (settings == null)
                Debug.LogError("Create UXToolCommonData Failed!");

            string path = Path.GetDirectoryName(ThunderFireUIToolConfig.UXToolCommonDataPath);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var assetPath = ThunderFireUIToolConfig.UXToolCommonDataPath;
            AssetDatabase.CreateAsset(settings, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return settings;
        }

        public void Save()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#if UNITY_EDITOR_WIN
            CustomUnityWindowLogic.DoUpdateTitleFunc();
#endif
        }

        private void OnValidate()
        {
#if UNITY_EDITOR_WIN
            CustomUnityWindowLogic.DoUpdateTitleFunc();
#endif
        }

        private bool UseCustomUnityWindowTitle = false;
        public string CustomUnityWindowTitle = "";
    }
}
#endif