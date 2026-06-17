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

            /// <summary>
            /// resourceComponent 传 null 表示资源是运行时创建的（如文件系统、Web下载反序列化的资源），释放时直接销毁而非通过资源系统卸载。
            /// </summary>
            internal static AssetSetObject Create(string name, Type type, object target, ResourceComponent resourceComponent)
            {
                AssetSetObject assetSetObject = ReferencePool.Acquire<AssetSetObject>();
                assetSetObject.Initialize(name, type, target);
                assetSetObject.m_ResourceComponent = resourceComponent;
                return assetSetObject;
            }

            protected override void Release(bool isShutdown)
            {
                if (m_ResourceComponent != null)
                {
                    m_ResourceComponent.UnloadAsset(Target);
                    m_ResourceComponent = null;
                }
                else if (Target is UnityEngine.Object unityObject)
                {
                    UnityEngine.Object.Destroy(unityObject);
                }
            }
        }
    }
}
