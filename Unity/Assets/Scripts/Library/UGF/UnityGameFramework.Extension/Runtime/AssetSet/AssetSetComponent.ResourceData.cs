using System;
using GameFramework;

namespace UnityGameFramework.Extension
{
    public partial class AssetSetComponent
    {
        private class ResourceData : IReference
        {
            public string AssetPath { get; private set; }
            public Type AssetType { get; private set; }

            public void Clear()
            {
                AssetPath = null;
                AssetType = null;
            }

            public static ResourceData Create(string assetPath, Type assetType)
            {
                ResourceData resourceData = ReferencePool.Acquire<ResourceData>();
                resourceData.AssetPath = assetPath;
                resourceData.AssetType = assetType;
                return resourceData;
            }
        }
    }
}