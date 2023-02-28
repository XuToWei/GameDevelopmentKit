using GameFramework;

namespace Game
{
    public static class AssetUtility
    {
        public static string GetFontAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Res/Font/{0}.ttf", assetName);
        }

        public static string GetSceneAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Res/Scene/{0}.unity", assetName);
        }

        public static string GetMusicAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Res/Music/{0}.mp3", assetName);
        }

        public static string GetSoundAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Res/Sound/{0}.wav", assetName);
        }

        public static string GetEntityAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Res/Entity/{0}.prefab", assetName);
        }

        public static string GetUIFormAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Res/UI/UIForm/{0}.prefab", assetName);
        }

        public static string GetUISoundAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Res/UI/UISound/{0}.wav", assetName);
        }
        
        public static string GetUISpriteAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Res/UI/UISprite/{0}.png", assetName);
        }

        public static string GetCodeAsset(string assetName)
        {
            return Utility.Text.Format("Assets/Res/Code/{0}.byte", assetName);
        }
    }
}
