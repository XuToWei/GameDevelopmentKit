using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace ThunderFireUITool
{
    //UXTools中用到的本地化配置和常量
    public class EditorLocalizationConfig
    {
        public static readonly string UXCommonPath = $"{ThunderFireUIToolConfig.EditorAssetsRootPath}Common/";

        #region Editor Localization
        //编辑器本地化路径
        public static readonly string LocalizationAssetsPath = UXCommonPath + "EditorLocalization/";
        public static readonly string LocalizationPath = $"{ThunderFireUIToolConfig.ScriptsRootPath}Editor/Common/EditorLocalization/";
        //编辑器本地化设置路径
        public static readonly string LocalizationSettingsPath = LocalizationAssetsPath + "Settings/";
        public static readonly string LocalizationSettingsFullPath = LocalizationSettingsPath + "EditorLocalizationSettings.json";

        public static readonly string LocalizationData = LocalizationAssetsPath + "EditorLocalizationData_";
        public static readonly string LocalizationUIInspectorData = LocalizationAssetsPath + "EditorLocalizationUIInspectorData_";

        public static readonly string LocalizationJsonPath = LocalizationAssetsPath + "Json/EditorLocalization.json";
        public static readonly string LocalizationUIInspectorJsonPath = LocalizationAssetsPath + "Json/Localization_UIInspector.json";

        public static readonly string EditorLocalizationStorageCode =
@"namespace ThunderFireUITool
{
    public class EditorLocalizationStorage
    {
        //auto Code
        #成员#
    }
}";//自动化生成代码的模板函数

        //多语言CS的位置
        public static readonly string EditorLocalizationStoragePath =
            Application.dataPath.Replace("Assets", string.Empty) + LocalizationPath + "EditorLocalizationStorage" + ".cs";

        public static readonly string[] LocalType = { "zhCn", "en" };
        public static readonly string Assetsuffix = ".asset";
        public static readonly string Jsonsuffix = ".json";
        #endregion
    }
}
