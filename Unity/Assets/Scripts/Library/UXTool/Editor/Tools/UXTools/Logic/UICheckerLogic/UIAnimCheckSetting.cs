#if UNITY_EDITOR && ODIN_INSPECTOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace ThunderFireUITool
{

    public class UIAnimCheckSetting : ScriptableObject
    {

        [MenuItem(ThunderFireUIToolConfig.Menu_CreateAssets + "/" + ThunderFireUIToolConfig.ResourceCheck + "/UIAnimCheckSetting", false, -1)]
        private static void SelectedGameObject()
        {
            var uiAnimCheckSetting = CreateInstance<UIAnimCheckSetting>();
            string path = Path.GetDirectoryName(ThunderFireUIToolConfig.UICheckAnimFullPath);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            AssetDatabase.CreateAsset(uiAnimCheckSetting, ThunderFireUIToolConfig.UICheckAnimFullPath);
        }

        public List<DontRebuildObject> objects;
    }

    [Serializable]
    public class DontRebuildObject
    {
        public MonoScript DontRebuild;
        public List<string> PropertyNames;
    }
}
#endif