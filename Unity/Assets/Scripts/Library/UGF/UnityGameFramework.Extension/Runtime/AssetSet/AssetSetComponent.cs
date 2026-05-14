using System.Collections.Generic;
using GameFramework;
using GameFramework.ObjectPool;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace UnityGameFramework.Extension
{
    public sealed partial class AssetSetComponent : GameFrameworkComponent
    {
        /// <summary>
        /// 资源设置对象池
        /// </summary>
        private IObjectPool<AssetSetObject> m_AssetSetObjectPool;
        
        /// <summary>
        /// 检查是否可以释放间隔
        /// </summary>
        [SerializeField]
        private float m_CheckCanReleaseInterval = 30f;

        /// <summary>
        /// 对象池自动释放时间间隔
        /// </summary>
        [SerializeField]
        private float m_AutoReleaseInterval = 60f;

        /// <summary>
        /// 对象池容量
        /// </summary>
        [SerializeField]
        [DisableIf(nameof(m_AssetSetObjectPool))]
        private int m_PoolCapacity = 16;

        /// <summary>
        /// 对象过期时间
        /// </summary>
        [SerializeField]
        [DisableIf(nameof(m_AssetSetObjectPool))]
        private float m_PoolExpireTime = 60f;

        [ReadOnly]
        [ShowInInspector]
        private LinkedList<LoadedAssetSet> m_LoadedAssetSetLinkedList;

        [ReadOnly]
        [ShowInInspector]
        private float m_CheckCanReleaseTime = 0.0f;

        private HashSet<NameTypePair> m_LoadingAssets;
        private Dictionary<NameTypePair, UGFDictionary<object, IAssetSet>> m_WaitingAssetSets;

        /// <summary>
        /// 对象池容量
        /// </summary>
        public int PoolCapacity
        {
            get
            {
                return m_AssetSetObjectPool.Capacity;
            }
            set
            {
                m_AssetSetObjectPool.Capacity = m_PoolCapacity = value;
            }
        }

        /// <summary>
        /// 对象过期时间
        /// </summary>
        public float PoolExpireTime
        {
            get
            {
                return m_AssetSetObjectPool.ExpireTime;
            }
            set
            {
                m_AssetSetObjectPool.ExpireTime = m_PoolExpireTime = value;
            }
        }

        private void Start()
        {
            ObjectPoolComponent objectPoolComponent = GameEntry.GetComponent<ObjectPoolComponent>();
            m_AssetSetObjectPool = objectPoolComponent.CreateMultiSpawnObjectPool<AssetSetObject>("AssetSet", m_AutoReleaseInterval, m_PoolCapacity, m_PoolExpireTime, 0);
            m_LoadedAssetSetLinkedList = new LinkedList<LoadedAssetSet>();
            m_LoadingAssets = new HashSet<NameTypePair>();
            m_WaitingAssetSets = new Dictionary<NameTypePair, UGFDictionary<object, IAssetSet>>();

            InitializeResources();
            InitializeFileSystem();
            InitializeWeb();
        }

        private void Update()
        {
            m_CheckCanReleaseTime += Time.unscaledDeltaTime;
            if (m_CheckCanReleaseTime < (double)m_CheckCanReleaseInterval)
                return;
            ReleaseUnused();
        }

        /// <summary>
        /// 回收无引用的 Image 对应图集。
        /// </summary>
        [Button("Release Unused")]
        public void ReleaseUnused()
        {
            if (m_LoadedAssetSetLinkedList == null)
                return;
            LinkedListNode<LoadedAssetSet> current = m_LoadedAssetSetLinkedList.First;
            while (current != null)
            {
                var next = current.Next;
                var value = current.Value;
                if (value.AssetSet.IsCanRelease())
                {
                    m_AssetSetObjectPool.Unspawn(value.Asset);
                    ReferencePool.Release(value.AssetSet);
                    m_LoadedAssetSetLinkedList.Remove(current);
                    ReferencePool.Release(value);
                }
                current = next;
            }
            m_CheckCanReleaseTime = 0;
        }

        public void RemoveLoadingAssetSet(IAssetSet assetSet)
        {
            NameTypePair assetKey = new NameTypePair(assetSet.AssetPath, assetSet.AssetType);
            if (m_WaitingAssetSets.TryGetValue(assetKey, out UGFDictionary<object, IAssetSet> waitingAssetSetDictionary))
            {
                if (waitingAssetSetDictionary.Remove(assetSet.Target))
                {
                    ReferencePool.Release(assetSet);
                }

                if (waitingAssetSetDictionary.Count == 0)
                {
                    m_WaitingAssetSets.Remove(assetKey);
                    waitingAssetSetDictionary.Dispose();
                }
            }
        }

        public void RemoveAllLoadingAssetSet()
        {
            foreach (var waitingAssetSetDictionary in m_WaitingAssetSets.Values)
            {
                foreach (var waitingAssetSet in waitingAssetSetDictionary.Values)
                {
                    ReferencePool.Release(waitingAssetSet);
                }
                waitingAssetSetDictionary.Dispose();
            }
            m_WaitingAssetSets.Clear();
        }
    }
}