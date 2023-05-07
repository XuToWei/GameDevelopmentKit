using GameFramework.Localization;

namespace Game.Editor
{
    public static class LocalizationReadyLanguage
    {
        public static Language[] Languages => new Language[]
        {
            Language.ChineseSimplified,
            Language.ChineseTraditional,
            Language.English,
        };
    }
}
