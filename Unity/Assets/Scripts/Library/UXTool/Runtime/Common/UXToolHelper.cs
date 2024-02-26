using System;

public static class UXToolHelper
{
    public static bool IsInitRuntime { get; private set; } = false;

    public static void InitRuntime(Func<LocalizationHelper.LanguageType, string, string, string> getStringFunc)
    {
        IsInitRuntime = true;
        LocalizationHelper.InitGetStringFunc(getStringFunc);
    }
}