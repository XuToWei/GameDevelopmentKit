using GameFramework;

namespace Game
{
    public static partial class AssetUtility
    {
        public static string GetFontAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Res/Font/{0}.otf", assetName);
        }

        public static string GetSceneAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Res/Scene/{0}.unity", assetName);
        }

        public static string GetShaderAsset(string shaderName)
        {
            return Utility.Text.Format("Assets/Res/Shader/{0}.shader", shaderName);
        }

        public static string GetShaderShaderVariantsAsset(string variantsName)
        {
            return Utility.Text.Format("Assets/Res/Shader/{0}.shadervariants", variantsName);
        }

        public static string GetLocalizationAsset(string assetName)
        {
            return Utility.Text.Format("Assets/GameMain/Localization/{0}/{1}", GameEntry.Localization.Language, assetName);
        }

        public static string GetMusicAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Res/Music/{0}.mp3", assetName);
        }

        public static string GetSoundAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Res/Sound/{0}.wav", assetName);
        }
        
        public static string GetPrefabAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Res/Prefab/{0}.prefab", assetName);
        }

        public static string GetEntityAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Res/Entity/{0}.prefab", assetName);
        }

        public static string GetUIFormAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Res/UI/UIForm/{0}.prefab", assetName);
        }

        public static string GetUIPrefabAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Res/UI/UIPrefab/{0}.prefab", assetName);
        }

        public static string GetUISoundAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Res/UI/UISound/{0}.wav", assetName);
        }

        public static string GetUISpriteAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Res/UI/UISprite/{0}.png", assetName);
        }

        public static string GetLubanAsset(string assetName, bool fromJson)
        {
            return Utility.Text.Format("Assets/Res/Luban/{0}.{1}", assetName, fromJson ? "json" : "bytes");
        }
        
        public static string GetETAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Res/ET/{0}", assetName);
        }
        
        public static string GetGameHotAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Res/Hot/{0}", assetName);
        }
        
        public static string GetConfigAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Res/Config/{0}", assetName);
        }
    }
}