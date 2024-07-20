using System.IO;
using UnityEngine;

namespace ThunderFireUITool
{
    //UXTools中的路径和常量
    public partial class ThunderFireUIToolConfig
    {
        public static readonly string TextTablePath = "../Design/Excel/Localization.xlsx";

        public static readonly string UXEditorCommonPath = $"{EditorAssetsRootPath}Common/";
        public static readonly string UXEditorToolsPath = $"{EditorAssetsRootPath}Tools/";
        public static readonly string UXGUIPath = $"{AssetsRootPath}GUI/";

        public static readonly string UXEditorFeaturePath = $"{EditorAssetsRootPath}Feature/";
        public static readonly string UXFeaturePath = $"{AssetsRootPath}Feature/";

        #region Editor Res
        public static readonly string IconPath = UXEditorToolsPath + "Res/Icon/";
        public static readonly string IconCursorPath = UXEditorToolsPath + "Res/Cursor/";
        public static readonly string UIBuilderPath = UXEditorToolsPath + "Window_uibuilder/";
        public static readonly string ScenePath = UXEditorToolsPath + "Scene/";
        #endregion

        #region Widget Setting
        public static readonly string WidgetLibrarySettingsPath = UXEditorToolsPath + "Settings/Widget/";
        //组件库-组件类型数据
        public static readonly string WidgetLabelsPath = WidgetLibrarySettingsPath + "WidgetLabels.json";
        //组件库-被认定为组件的Prefab信息
        public static readonly string WidgetListPath = WidgetLibrarySettingsPath + "WidgetList.json";

        public static readonly string WidgetLibraryDefaultLabel = "All";
        #endregion

        #region HierarchyManagement
        public static readonly string HierarchyManagementSettingsPath = UXEditorToolsPath + "Settings/HierarchyManagement/";
        //层级管理工具 层级配置 用于绘制编辑器窗口
        public static readonly string HierarchyManagementSettingPath =
            HierarchyManagementSettingsPath + "HierarchyManagementSetting.json";
        //层级管理工具 编辑器中层级数据 用于绘制编辑器窗口 保存了Tag等Editor信息
        public static readonly string HierarchyManagemenEditorDataPath =
            HierarchyManagementSettingsPath + "HierarchyManagementEditorData.json";

        //层级管理工具 导出后的层级数据 用于项目实际读取 只保存Channel和Level
        public static readonly string HierarchyManagementDataPath =
            HierarchyManagementSettingsPath + "HierarchyManagementData.json";

        //层级管理工具 Sample中 层级配置
        public static readonly string HierarchyManagementSettingPath_Sample =
            SamplesRootPath + "HierarchyManageSample/Resources/HierarchyManagementSetting_Sample.json";
        //层级管理工具 Sample中 编辑器层级数据
        public static readonly string HierarchyManagementEditorDataPath_Sample =
            SamplesRootPath + "HierarchyManageSample/Resources/HierarchyManagementEditorData_Sample.json";

        //层级管理工具 Sample中 导出后层级配置
        public static readonly string HierarchyManagementDataPath_Sample =
            SamplesRootPath + "HierarchyManageSample/Resources/HierarchyManagementData_Sample.json";
        #endregion


        #region User Data
        public static readonly string UserDataPath = UXEditorToolsPath + "UserDatas/";
        //Common数据 目前包括: 标题
        public static readonly string UXToolCommonDataPath = UserDataPath + "UXToolCommonData.asset";
        //辅助线数据
        public static readonly string LocationLinesDataPath = UserDataPath + "LocationLinesData.json";
        //最近打开的Prefab数据
        public static readonly string PrefabRecentOpenedPath = UserDataPath + "PrefabRecentlyOpenedData.json";
        //Scene窗口Tab页签数据
        public static readonly string PrefabTabsPath = UserDataPath + "PrefabTabsData.json";
        //快速背景图数据
        public static readonly string QuickBackgroundDataPath = UserDataPath + "QuickBackgroundData.json";
        //功能开关数据
        public static readonly string SwitchSettingPath = UserDataPath + "SwitchSetting.json";
        //最近选中文件数据
        public static readonly string FilesRecentSelectedPath = UserDataPath + "FilesRecentlySelectedData.json";
        //工具全局数据
        public static readonly string GlobalDataPath = $"{UXEditorCommonPath}ToolGlobalData/ToolGlobalData.json";
        #endregion

        #region Res Check Setting
        public static readonly string SettingsPath = UXEditorToolsPath + "Settings/UICheck/";
        public static readonly string UICheckSettingFullPath = SettingsPath + "UIResCheckSetting.asset";
        public static readonly string UICheckLegacyComponentFullPath = SettingsPath + "UILegacyComponentCheckSetting.asset";
        public static readonly string UICheckAnimFullPath = SettingsPath + "UIAnimCheckSetting.asset";

        public static readonly string UICheckUserDataFullPath = UserDataPath + "UIResCheckUserData.asset";
        #endregion

        #region MenuItem Name
        public const string MenuName = "ThunderFireUXTool/";
        public const string ToolBar = "工具栏 (Toolbar)";

        public const string About = "关于 (About)";
        public const string WelcomePage = "欢迎页 (Welcome Page)";

        public const string Setting = "设置 (Setting)";
        public const string CreateAssets = "新建配置文件 (Create Assets)";

        public const string CommonData = "通用数据 (Common Data)";
        public const string WidgetLibrary = "组件库 (Widget Library)";
        public const string HierarchyManage = "层级管理工具 (Hierarchy Manage)";
        public const string CreateBeginnerGuide = "创建新手引导(Create BeginnerGuide)";
        public const string Localization = "本地化 (Localization)";
        public const string UIColor = "颜色配置 (UIColorConfig)";
        public const string BackGround = "创建背景图 (Create QuickBackground)";
        public const string AutoStitching = "自动拼接工具 (AutoStitchingTool)";

        public const string RecentlySelected = "最近选中文件 (Recently Selected)";
        public const string RecentlyOpened = "最近打开 (Recently Opened)";
        public const string PrefabTabs = "Prefab页签 (Prefab Tabs)";
        public const string ResourceCheck = "资源检查工具 (ResourceCheckTools)";
        public const string ComponentCheck = "脚本检查工具 (ComponentCheckTool)";


        public const string Menu_About = MenuName + About;  //-150
        public const string Menu_WelcomePage = MenuName + WelcomePage;  //-149
        public const string Menu_Setting = MenuName + Setting;  //-100
        public const string Menu_CreateAssets = MenuName + CreateAssets;    //-99  -50到-1留给配置文件进行排序

        public const string Menu_WidgetLibrary = MenuName + WidgetLibrary;  //51
        public const string Menu_HierarchyManage = MenuName + HierarchyManage;  //52
        public const string Menu_UIColor = MenuName + UIColor;  //53
        public const string Menu_Localization = MenuName + Localization;    //54
        public const string Menu_CreateBeginnerGuide = MenuName + CreateBeginnerGuide;  //55

        public const string Menu_ToolBar = MenuName + ToolBar;  //101

        public const string Menu_BackGround = MenuName + BackGround;    //151
        public const string Menu_AutoAssemblePrefab = MenuName + AutoStitching;    //152
        public const string Menu_RecentlyOpened = MenuName + RecentlyOpened;    //153
        public const string Menu_RecentlySelected = MenuName + RecentlySelected; // 154
        public const string Menu_ResourceCheck = MenuName + ResourceCheck;  //160
        public const string Menu_ComponentCheck = MenuName + ComponentCheck;  //161
        public const int Menu_ResourceCheckIndex = 170;

        public const string Menu_UXToolLocalization = "UXToolLocalization";
        public const string Menu_ReferenceLine = "辅助线 (Reference Line)";
        #endregion

        #region EditorPref Name
        //用于存储需要在Play状态前后保持，但是又没有重要到需要持久化的Editor数据
        //其实就是持久化数据的简便做法
        public const string PreviewPrefabPath = "PreviewPrefabPath";
        public const string PreviewOriginScene = "PreviewOriginScene";
        #endregion

        #region Const Color
        // RGBA(60, 60, 60, 1)
        public static Color disableColor = new Color(0.235f, 0.235f, 0.235f, 1f);
        // RGBA(65, 65, 65, 1)
        public static Color hoverColor = new Color(0.255f, 0.255f, 0.255f, 1f);
        // RGBA(51, 51, 51, 1)
        public static Color normalColor = new Color(0.2f, 0.2f, 0.2f, 1f);

        #endregion

        #region Prefab
        public static readonly int m_maxCharacters = 20;
        public static readonly int m_minCharacters = 13;
        public static readonly int m_maxWidth = 150;
        public static readonly int m_minWidth = 100;
        #endregion
    }
}
