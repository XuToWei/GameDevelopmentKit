using Cysharp.Threading.Tasks;
using GameFramework.Localization;
using UnityGameFramework.Extension;
using UnityGameFramework.Runtime;

namespace Game
{
    public static partial class LocalizationExtension
    {
        public static UniTask LoadLanguageAsync(this LocalizationComponent localizationComponent, Language language)
        {
            GameEntry.Localization.RemoveAllRawStrings();
            GameEntry.BuiltinData.InitDefaultDictionary(language);
            return localizationComponent.ReadDataAsync(AssetUtility.GetLocalizationAsset(language), Constant.AssetPriority.DictionaryAsset);
        }
    }
}