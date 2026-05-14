using GameFramework;

namespace UnityGameFramework.Extension
{
    public partial class AssetSetComponent
    {
        private class WebRequestData : IReference
        {
            public IAssetSet AssetSet { get; private set; }
            public AssetSetComponent UserData { get; private set; }

            public static WebRequestData Create(IAssetSet assetSet, AssetSetComponent userData)
            {
                WebRequestData webRequestData = ReferencePool.Acquire<WebRequestData>();
                webRequestData.AssetSet = assetSet;
                webRequestData.UserData = userData;
                return webRequestData;
            }

            public void Clear()
            {
                AssetSet = null;
                UserData = null;
            }
        }
    }
}