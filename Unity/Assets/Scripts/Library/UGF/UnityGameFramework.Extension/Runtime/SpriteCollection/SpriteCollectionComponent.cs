using System.Collections.Generic;
using GameFramework;
using GameFramework.ObjectPool;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace UnityGameFramework.Extension
{
    public sealed partial class SpriteCollectionComponent : GameFrameworkComponent
    {
        /// <summary>
        /// 散图集合对象池
        /// </summary>
        private IObjectPool<SpriteCollectionItemObject> m_SpriteCollectionPool;
        
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
        [DisableIf("m_SpriteCollectionPool")]
        private int m_PoolCapacity = 16;

        /// <summary>
        /// 对象过期时间
        /// </summary>
        [SerializeField]
        [DisableIf("m_SpriteCollectionPool")]
        private float m_PoolExpireTime = 60f;

        [ReadOnly]
        [ShowInInspector]
        private LinkedList<LoadSpriteObject> m_LoadedSpriteObjectsLinkedList;

        [ReadOnly]
        [ShowInInspector]
        private float m_CheckCanReleaseTime = 0.0f;

        private HashSet<string> m_SpriteCollectionBeingLoaded;
        private Dictionary<string, HashSet<ISetSpriteObject>> m_WaitSetObjects;

        /// <summary>
        /// 对象池容量
        /// </summary>
        public int PoolCapacity
        {
            get
            {
                return m_SpriteCollectionPool.Capacity;
            }
            set
            {
                m_SpriteCollectionPool.Capacity = m_PoolCapacity = value;
            }
        }

        /// <summary>
        /// 对象过期时间
        /// </summary>
        public float PoolExpireTime
        {
            get
            {
                return m_SpriteCollectionPool.ExpireTime;
            }
            set
            {
                m_SpriteCollectionPool.ExpireTime = m_PoolExpireTime = value;
            }
        }

        private void Start()
        {
            ObjectPoolComponent objectPoolComponent = GameEntry.GetComponent<ObjectPoolComponent>();
            m_SpriteCollectionPool = objectPoolComponent.CreateMultiSpawnObjectPool<SpriteCollectionItemObject>("SpriteCollection", m_AutoReleaseInterval, m_PoolCapacity, m_PoolExpireTime, 0);
            m_LoadedSpriteObjectsLinkedList = new LinkedList<LoadSpriteObject>();
            m_SpriteCollectionBeingLoaded = new HashSet<string>();
            m_WaitSetObjects = new Dictionary<string, HashSet<ISetSpriteObject>>();

            InitializedResources();
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
            if (m_LoadedSpriteObjectsLinkedList == null)
                return;
            LinkedListNode<LoadSpriteObject> current = m_LoadedSpriteObjectsLinkedList.First;
            while (current != null)
            {
                var next = current.Next;
                if (current.Value.SpriteObject.IsCanRelease())
                {
                    m_SpriteCollectionPool.Unspawn(current.Value.Collection);
                    ReferencePool.Release(current.Value.SpriteObject);
                    m_LoadedSpriteObjectsLinkedList.Remove(current);
                    ReferencePool.Release(current.Value);
                }
                current = next;
            }
            m_CheckCanReleaseTime = 0;
        }
    }
}