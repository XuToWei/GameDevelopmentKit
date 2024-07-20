//RuntimePlatform无法处理不同引擎情况的平台判断，比如PC的引擎没有PS5的枚举，通过宏来判断平台更为通用
//https://docs.unity3d.com/Manual/PlatformDependentCompilation.html
public class UXPlatform
{
    public static bool IsEditor()
    {
#if UNITY_EDITOR
        return true;
#else
		return false;
#endif
    }

    public static bool IsWindows()
    {
#if UNITY_STANDALONE_WIN
		return true;
#else
        return false;
#endif
    }

    public static bool IsMac()
    {
#if UNITY_STANDALONE_OSX
		return true;
#else
        return false;
#endif
    }

    public static bool IsStandalone()
    {
#if UNITY_STANDALONE
		return true;
#else
        return false;
#endif
    }

    public static bool IsPS4()
    {
#if UNITY_PS4
		return true;
#else
        return false;
#endif
    }

    public static bool IsPS5()
    {
#if UNITY_PS5
		return true;
#else
        return false;
#endif
    }

    public static bool IsAndroid()
    {
#if UNITY_ANDROID
        return true;
#else
		return false;
#endif
    }

    public static bool IsIOS()
    {
#if UNITY_IOS
		return true;
#else
        return false;
#endif
    }

    public static bool IsXboxSeries()
    {
#if UNITY_GAMECORE_XBOXSERIES
		return true;
#else
        return false;
#endif
    }

    public static bool IsXbox()
    {
#if UNITY_GAMECORE
		return true;
#else
        return false;
#endif
    }

    public static bool IsConsole()
    {
        return IsPS4() || IsPS5() || IsXbox();
    }

    //平台大类
    public enum PlatformType
    {
        Other = 0,
        PC = 1,
        Console = 2,
        Mobile = 3,
    }

    public static PlatformType GetPlatformType()
    {
        if (UXPlatform.IsStandalone() || UXPlatform.IsEditor())
        {
            return PlatformType.PC;
        }
        if (UXPlatform.IsPS4() || UXPlatform.IsPS5() || UXPlatform.IsXbox())
        {
            return PlatformType.Console;
        }
        if (UXPlatform.IsAndroid() || UXPlatform.IsIOS())
        {
            return PlatformType.Mobile;
        }
        return PlatformType.Other;
    }

}