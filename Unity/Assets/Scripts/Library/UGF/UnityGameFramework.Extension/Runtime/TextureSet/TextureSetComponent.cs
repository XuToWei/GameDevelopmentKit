using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameFramework;
using GameFramework.ObjectPool;
using UnityEngine;
using UnityGameFramework.Runtime;
using Sirenix.OdinInspector;

namespace UnityGameFramework.Extension
{
    public sealed partial class TextureSetComponent : GameFrameworkComponent
    {
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
        [DisableIf("m_TexturePool")]
        private int m_PoolCapacity = 16;

        /// <summary>
        /// 对象过期时间
        /// </summary>
        [SerializeField]
        [DisableIf("m_TexturePool")]
        private float m_PoolExpireTime = 60f;

        /// <summary>
        /// 保存加载的图片对象
        /// </summary>
        [ReadOnly]
        [ShowInInspector]
        [PropertyOrder(int.MaxValue - 1)]
        private LinkedList<LoadTextureObject> m_LoadTextureObjectsLinkedList;

        /// <summary>
        /// 正在加载的图片路径集合
        /// </summary>
        private HashSet<string> m_TextureBeingLoaded;

        /// <summary>
        /// 等待设置的对象集合
        /// </summary>
        private Dictionary<string, UGFHashSet<ISetTexture2dObject>> m_WaitSetObjects;

        [ReadOnly]
        [ShowInInspector]
        [PropertyOrder(int.MaxValue - 1)]
        private float m_CheckCanReleaseTime = 0.0f;

        /// <summary>
        /// 散图集合对象池
        /// </summary>
        private IObjectPool<TextureItemObject> m_TexturePool;

        /// <summary>
        /// 对象池容量
        /// </summary>
        public int PoolCapacity
        {
            get
            {
                return m_TexturePool.Capacity;
            }
            set
            {
                m_TexturePool.Capacity = m_PoolCapacity = value;
            }
        }

        /// <summary>
        /// 对象过期时间
        /// </summary>
        public float PoolExpireTime
        {
            get
            {
                return m_TexturePool.ExpireTime;
            }
            set
            {
                m_TexturePool.ExpireTime = m_PoolExpireTime = value;
            }
        }

        private IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();
            ObjectPoolComponent objectPoolComponent = GameEntry.GetComponent<ObjectPoolComponent>();
            m_TexturePool = objectPoolComponent.CreateMultiSpawnObjectPool<TextureItemObject>("TexturePool", m_AutoReleaseInterval, m_PoolCapacity, m_PoolExpireTime, 0);
            m_LoadTextureObjectsLinkedList = new LinkedList<LoadTextureObject>();
            m_TextureBeingLoaded = new HashSet<string>();
            m_WaitSetObjects = new Dictionary<string, UGFHashSet<ISetTexture2dObject>>();
            InitializedFileSystem();
            InitializedResources();
            InitializedWeb();
        }

        private void Update()
        {
            m_CheckCanReleaseTime += Time.unscaledDeltaTime;
            if (m_CheckCanReleaseTime < (double)m_CheckCanReleaseInterval)
                return;
            ReleaseUnused();
        }


        /// <summary>
        /// 回收无引用的Texture。
        /// </summary>
        [Button("Release Unused")]
        [PropertyOrder(int.MaxValue)]
        public void ReleaseUnused()
        {
            if (m_LoadTextureObjectsLinkedList == null)
                return;
            LinkedListNode<LoadTextureObject> current = m_LoadTextureObjectsLinkedList.First;
            while (current != null)
            {
                var next = current.Next;
                if (current.Value.Texture2dObject.IsCanRelease())
                {
                    m_TexturePool.Unspawn(current.Value.Texture2D);
                    ReferencePool.Release(current.Value.Texture2dObject);
                    ReferencePool.Release(current.Value);
                    m_LoadTextureObjectsLinkedList.Remove(current);
                }

                current = next;
            }

            m_CheckCanReleaseTime = 0f;
        }

        private void SetTexture(ISetTexture2dObject setTexture2dObject, Texture2D texture)
        {
            m_LoadTextureObjectsLinkedList.AddLast(LoadTextureObject.Create(setTexture2dObject, texture));
            setTexture2dObject.SetTexture(texture);
        }

        /// <summary>
        /// 移除正在加载的设置图片对象
        /// </summary>
        /// <param name="setTexture2dObject">设置图片对象</param>
        public void RemoveLoadingSetTexture(ISetTexture2dObject setTexture2dObject)
        {
            if (m_WaitSetObjects.TryGetValue(setTexture2dObject.Texture2dFilePath, out UGFHashSet<ISetTexture2dObject> awaitSets))
            {
                if (awaitSets.Remove(setTexture2dObject))
                {
                    ReferencePool.Release(setTexture2dObject);
                }

                if (awaitSets.Count == 0)
                {
                    m_WaitSetObjects.Remove(setTexture2dObject.Texture2dFilePath);
                    awaitSets.Dispose();
                }
            }
        }

        /// <summary>
        /// 移除所有正在加载的设置图片对象
        /// </summary>
        public void RemoveAllLoadingSetTexture()
        {
            foreach (var awaitSets in m_WaitSetObjects.Values)
            {
                foreach (var awaitSet in awaitSets)
                {
                    ReferencePool.Release(awaitSet);
                }
                awaitSets.Dispose();
            }
            m_WaitSetObjects.Clear();
        }
    }
}