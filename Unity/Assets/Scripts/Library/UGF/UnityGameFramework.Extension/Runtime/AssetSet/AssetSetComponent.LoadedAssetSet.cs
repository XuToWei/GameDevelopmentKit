using System;
using GameFramework;
using Sirenix.OdinInspector;

namespace UnityGameFramework.Extension
{
    public partial class AssetSetComponent
    {
        [Serializable]
        private sealed class LoadedAssetSet : IReference
        {
            [ShowInInspector]
            public IAssetSet AssetSet { get; private set; }

            [ShowInInspector]
            public UnityEngine.Object Asset { get; private set; }

            public static LoadedAssetSet Create(IAssetSet assetSet, UnityEngine.Object asset)
            {
                LoadedAssetSet loadedAssetSet = ReferencePool.Acquire<LoadedAssetSet>();
                loadedAssetSet.AssetSet = assetSet;
                loadedAssetSet.Asset = asset;
                return loadedAssetSet;
            }

            public void Clear()
            {
                AssetSet = null;
                Asset = null;
            }
        }
    }
}