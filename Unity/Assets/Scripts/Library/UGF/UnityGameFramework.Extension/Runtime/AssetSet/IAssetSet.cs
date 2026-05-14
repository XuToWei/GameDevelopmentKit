using System;
using GameFramework;
using UnityGameFramework.Runtime;

namespace UnityGameFramework.Extension
{
    /// <summary>
    /// 资源设置接口。
    /// </summary>
    public interface IAssetSet : IReference
    {
        /// <summary>
        /// 资源路径。
        /// </summary>
        string AssetPath { get; }

        /// <summary>
        /// 获取对象目标。
        /// </summary>
        object Target { get; }

        /// <summary>
        /// 资源类型。
        /// </summary>
        Type AssetType { get; }

        /// <summary>
        /// 设置资源。
        /// </summary>
        /// <param name="asset">资源对象。</param>
        void SetAsset(UnityEngine.Object asset);

        /// <summary>
        /// 是否可以回收。
        /// </summary>
        bool IsCanRelease();
    }

    /// <summary>
    /// 资源设置序列化接口。
    /// </summary>
    public interface ISerializeAssetSet
    {
        public UnityEngine.Object Serialize(byte[] bytes);
    }

    /// <summary>
    /// 资源设置序列化接口。
    /// </summary>
    public interface ISaveAbleAssetSet
    {
        public bool NeedSave { get; }
    }

    /// <summary>
    /// 资源设置项抽象基类。
    /// </summary>
    public abstract class AssetSet<T> : IAssetSet where T : UnityEngine.Object
    {
        public string AssetPath { get; protected set; }

        public Type AssetType => typeof(T);

        public object Target { get; protected set; }

        public abstract void SetAsset(T asset);

        public abstract bool IsCanRelease();

        public virtual void Clear()
        {
            AssetPath = null;
            Target = null;
        }

        void IAssetSet.SetAsset(UnityEngine.Object asset)
        {
            T tAsset = asset as T;
            if (tAsset == null)
            {
                Log.Error("Asset set is invalid, asset type is '{0}', but expected '{1}'.", asset.GetType().FullName, AssetType.FullName);
                return;
            }
            SetAsset(tAsset);
        }
    }
}
