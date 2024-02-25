#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace ThunderFireUITool
{
    static public class UIColorCreator
    {
        [MenuItem(ThunderFireUIToolConfig.Menu_CreateAssets + "/" + ThunderFireUIToolConfig.UIColor + "/UI Color Assets", false, -48)]
        public static void CreateColor()
        {
            var assetPath = UIColorConfig.ColorConfigPath + UIColorConfig.ColorConfigName + ".json";
            UIColorAsset config = JsonAssetManager.CreateAssets<UIColorAsset>(assetPath);
            config.GenColorDefScript();
        }

        [MenuItem(ThunderFireUIToolConfig.Menu_CreateAssets + "/" + ThunderFireUIToolConfig.UIColor + "/UI Gradient Assets", false, -48)]
        public static void CreateGradient()
        {
            var assetPath = UIColorConfig.ColorConfigPath + UIColorConfig.GradientConfigName + ".json";
            var config = JsonAssetManager.CreateAssets<UIGradientAsset>(assetPath);
            config.GenGradientScript();
        }
    }
}
#endif