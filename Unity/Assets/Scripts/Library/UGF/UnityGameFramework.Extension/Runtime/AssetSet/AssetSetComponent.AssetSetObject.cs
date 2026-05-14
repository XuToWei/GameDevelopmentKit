using System;
using GameFramework;
using GameFramework.ObjectPool;
using UnityGameFramework.Runtime;

namespace UnityGameFramework.Extension
{
    public partial class AssetSetComponent
    {
        private sealed class AssetSetObject : ObjectBase
        {
            private ResourceComponent m_ResourceComponent;

            internal static AssetSetObject Create(string name, Type type, object target, ResourceComponent resourceComponent)
            {
                AssetSetObject assetSetObject = ReferencePool.Acquire<AssetSetObject>();
                assetSetObject.Initialize(name, type, target);
                assetSetObject.m_ResourceComponent = resourceComponent;
                return assetSetObject;
            }

            protected override void Release(bool isShutdown)
            {
                m_ResourceComponent.UnloadAsset(Target);
                m_ResourceComponent = null;
            }
        }
    }
}
