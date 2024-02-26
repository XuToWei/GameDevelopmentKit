public enum LocalizationTypeDef
{
    zhCN = 0,
    zhTW = 1,
    enUS = 2,
    deDE = 3,
    jaJP = 4,
    koKR = 5,
    frFR = 6,
    esES = 7,
    ptPT = 8,
    ruRU = 9,
    trTR = 10,
    viVN = 11,
    arSA = 12,
    thTH = 13,
    idID = 14,
}

public static class UXGUIConfig
{
#if UNITY_EDITOR
    public static readonly string ScriptsRootPath = "Assets/Scripts/Library/UXTool/";
    public static readonly string EditorRootPath = "Assets/Res/Editor/UI/UXTool/";
#endif

    public static readonly string RootPath = "Assets/Res/UI/UXTool/";
    public static readonly string GUIPath = RootPath + "GUI/";
    public static readonly string UXToolAssetCollectionPath = RootPath + "UXToolAssetCollection.asset";

    public static readonly string UXImageDefaultMatPath = GUIPath + "Material/UXImage.mat";
    public static readonly string UXTextDefaultMatPath = GUIPath + "Material/UXImage.mat";
    public static readonly string UXGUINeedReplaceSpritePathReplace = GUIPath + "need_replace.png";

    public static readonly string ThaiWordDictPath = GUIPath + "thai_dict.txt";

    public static readonly string UIBeginnerGuideResPath = RootPath + "Feature/BeginnerGuide/";

    // Localization
    public static LocalizationTypeDef CurLocalizationType = LocalizationTypeDef.zhCN;
    // UIStateAnimator
    public static bool EnableOptimizeUIStateAnimator = true;

    public static string LocalizationFolder = "Assets/Res/Localization";
}