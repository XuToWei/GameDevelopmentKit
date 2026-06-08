using GameFramework;

namespace UnityGameFramework.Extension
{
    public partial class AssetSetComponent
    {
        private class WebRequestData : IReference
        {
            public string AssetPath { get; private set; }
            public AssetSetComponent UserData { get; private set; }

            public static WebRequestData Create(string assetPath, AssetSetComponent userData)
            {
                WebRequestData webRequestData = ReferencePool.Acquire<WebRequestData>();
                webRequestData.AssetPath = assetPath;
                webRequestData.UserData = userData;
                return webRequestData;
            }

            public void Clear()
            {
                AssetPath = null;
                UserData = null;
            }
        }
    }
}