//RuntimePlatform无法处理不同引擎情况的平台判断，比如PC的引擎没有PS5的枚举，通过宏来判断平台更为通用
//https://docs.unity3d.com/Manual/PlatformDependentCompilation.html

using UnityEngine;
using UnityEditor;

public partial class UXPlatform
{

	public enum XboxType
	{
		Unknown,
		One,
		OneS,
		OneX,
		SeriesS,
		SeriesX,
	}

	private static XboxType xbtype = XboxType.Unknown;

	public enum PlatformType
	{
		Other = 0,
		PC = 1,
		Console = 2,
		Mobile = 3,
	}

	public static PlatformType CheckGetPlatformType = PlatformType.PC;


	public static bool IsInitialized = false;

	public static bool CheckIsEditor = false;
	public static bool CheckIsWindows = false;
	public static bool CheckIsMac = false;
	public static bool CheckIsStandalone = false;
	public static bool CheckIsPS4 = false;
	public static bool CheckIsRealPS4 = false;
	public static bool CheckIsRealPS4Pro = false;
	public static bool CheckIsPS5 = false;
	public static bool CheckIsRealPS5 = false;
	public static bool CheckIsPlaystation = false;
	public static bool CheckIsAndroid = false;
	public static bool CheckIsIOS = false;
	public static bool CheckIsUnityXboxSeries = false;
	public static bool CheckIsUnityXboxOne = false;
	public static bool CheckIsXboxSeriesS = false;
	public static bool CheckIsXboxSeriesX = false;
	public static bool CheckIsXboxOne = false;
	public static bool CheckIsXboxOneS = false;
	public static bool CheckIsXboxOneX = false;
	public static bool CheckIsXbox = false;
	public static bool CheckIsXboxPC = false;
	public static bool CheckIsConsole = false;
	public static bool CheckIsMobile = false;
	public static bool CheckIsSecondPfmAliCloud = false;
	public static bool CheckCanOpenOnlineGM = false;
	public static bool CheckIsSecondPfmNeteaseCloud = false;
	public static bool CheckIsSteamDeck = false;
	public static bool CheckIsDevOrEditor = false;
	public static bool CheckIsLowerConsole = false;


    public static void InitPlatformInfo()
    {
        if (IsInitialized)
		{
			Debug.LogError("Platform is already initialized");
			return;
		}
		IsInitialized = true;

		InitPlatformInfoEarly();

		{
			if (UXPlatform.CheckIsStandalone || UXPlatform.CheckIsEditor || UXPlatform.CheckIsXboxPC)
			{
				CheckGetPlatformType = PlatformType.PC;
			}
			else if (UXPlatform.CheckIsPS4 || UXPlatform.CheckIsPS5 || UXPlatform.CheckIsXbox)
			{
				CheckGetPlatformType = PlatformType.Console;
			}
			else if (UXPlatform.CheckIsAndroid || UXPlatform.CheckIsIOS)
			{
				CheckGetPlatformType = PlatformType.Mobile;
			}
			else
			{
				CheckGetPlatformType = PlatformType.Other;
			}
		}
	}

	public static void InitSteamDeckInfo()
	{
		CheckIsSteamDeck = false;
	}

    /// <summary>
    /// Assembly初始化后提前初始化依赖宏的平台判断，避免一些早于脚本Awake的东西用错误的平台初始化
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
	public static void InitPlatformInfoEarly()
    {
        if (xbtype == XboxType.Unknown)
		{
#if UNITY_GAMECORE_XBOXSERIES
			if (SystemInfo.deviceModel == "XboxScarlettSeriesS" || SystemInfo.deviceModel == "XboxSeriesS")
				xbtype = XboxType.SeriesS;
			else if (SystemInfo.deviceModel == "XboxScarlettDevkit" || SystemInfo.deviceModel == "XboxSeriesX")
				xbtype = XboxType.SeriesX;
#elif UNITY_GAMECORE_XBOXONE
			if (SystemInfo.deviceModel == "XboxOneXDevkit" || SystemInfo.deviceModel == "XboxOneX")
				xbtype = XboxType.OneX;
			else if (SystemInfo.deviceModel == "XboxOneS")
				xbtype = XboxType.OneS;
			else if (SystemInfo.deviceModel == "XboxOne")
				xbtype = XboxType.One;
#endif
		}

		{
#if UNITY_EDITOR
			CheckIsEditor = true;
#else
			CheckIsEditor = false;
#endif
		}


		{
#if UNITY_STANDALONE_WIN
			CheckIsWindows = true;
#else
			CheckIsWindows = false;
#endif
		}

		{
#if UNITY_STANDALONE_OSX
			CheckIsMac = true;
#else
			CheckIsMac = false;
#endif
		}

		{
#if UNITY_STANDALONE
			CheckIsStandalone = true;
#else
			CheckIsStandalone = false;
#endif
		}


		{
#if UNITY_PS4
			CheckIsPS4 = true;
#else
			CheckIsPS4 = false;
#endif
		}


		{
#if UNITY_PS4 && !UNITY_EDITOR
        	CheckIsRealPS4 = true;
#else
			CheckIsRealPS4 = false;
#endif
		}

		{
#if UNITY_PS4 && !UNITY_EDITOR
        	CheckIsRealPS4Pro = UnityEngine.PS4.Utility.neoMode;
#else
			CheckIsRealPS4Pro = false;
#endif
		}

		{
#if UNITY_PS5
			CheckIsPS5 = true;
#else
			CheckIsPS5 = false;
#endif
		}


		{
#if UNITY_PS5 && !UNITY_EDITOR
			CheckIsRealPS5 = true;
#else
			CheckIsRealPS5 = false;
#endif
		}

		{
#if UNITY_PLAYSTATION
			CheckIsPlaystation = true;
#else
			CheckIsPlaystation = false;
#endif
		}

		{
#if UNITY_ANDROID
			CheckIsAndroid = true;
#else
			CheckIsAndroid = false;
#endif
		}

		{
#if UNITY_IOS
			CheckIsIOS = true;
#else
			CheckIsIOS = false;
#endif
		}


		{
#if UNITY_GAMECORE_XBOXSERIES
			CheckIsUnityXboxSeries = true;
#else
			CheckIsUnityXboxSeries = false;
#endif
		}

        //XBOXone有3个型号  one oneS oneX. 这个函数判断是否属于one系列机型
        //每个型号还有单独的判断函数: IsXboxOne IsXboxOneS IsXboxOneX
        {
#if UNITY_GAMECORE_XBOXONE
			CheckIsUnityXboxOne = true;
#else
            CheckIsUnityXboxOne = false;
#endif
		}

		{
#if UNITY_GAMECORE_XBOXSERIES
			CheckIsXboxSeriesS = xbtype == XboxType.SeriesS;
#else
			CheckIsXboxSeriesS = false;
#endif
		}

		{
#if UNITY_GAMECORE_XBOXSERIES
			CheckIsXboxSeriesX = xbtype == XboxType.SeriesX;
#else
			CheckIsXboxSeriesX = false;
#endif
		}

		{
#if UNITY_GAMECORE_XBOXONE
			CheckIsXboxOne = xbtype == XboxType.One;
#else
			CheckIsXboxOne = false;
#endif
		}

		{
#if UNITY_GAMECORE_XBOXONE
			CheckIsXboxOneS = xbtype == XboxType.OneS;
#else
			CheckIsXboxOneS = false;
#endif
		}

		{
#if UNITY_GAMECORE_XBOXONE
			CheckIsXboxOneX = xbtype == XboxType.OneX;
#else
			CheckIsXboxOneX = false;
#endif
		}

		{
#if UNITY_GAMECORE
			CheckIsXbox = true;
#else
			CheckIsXbox = false;
#endif
		}

		{
			CheckIsConsole = CheckIsPlaystation || CheckIsXbox;
		}

		{
			CheckIsMobile = CheckIsAndroid || CheckIsIOS;
		}

		{
#if UNITY_EDITOR || DEVELOPMENT_BUILD
			CheckIsDevOrEditor = true;
#else
			CheckIsDevOrEditor = false;
#endif
		}


		{
			if (UXPlatform.CheckIsStandalone || UXPlatform.CheckIsEditor || UXPlatform.CheckIsXboxPC)
			{
				CheckGetPlatformType = PlatformType.PC;
			}
			else if (UXPlatform.CheckIsPS4 || UXPlatform.CheckIsPS5 || UXPlatform.CheckIsXbox)
			{
				CheckGetPlatformType = PlatformType.Console;
			}
			else if (UXPlatform.CheckIsAndroid || UXPlatform.CheckIsIOS)
			{
				CheckGetPlatformType = PlatformType.Mobile;
			}
			else
			{
				CheckGetPlatformType = PlatformType.Other;
			}
		}

		{
			CheckIsLowerConsole = CheckIsXboxOne || CheckIsXboxOneS || CheckIsPS4;
		}
	}
}