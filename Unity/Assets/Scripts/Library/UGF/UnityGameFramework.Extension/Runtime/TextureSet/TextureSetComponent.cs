using System.Collections;
using System.Collections.Generic;
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
        /// 序号
        /// </summary>
        [ReadOnly]
        [ShowInInspector]
        [PropertyOrder(int.MaxValue - 1)]
        private int m_SerialId = 0;

        /// <summary>
        /// 取消加载序号集合
        /// </summary>
        [ReadOnly]
        [ShowInInspector]
        [PropertyOrder(int.MaxValue - 1)]
        private HashSet<int> m_CancelIds;

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
            m_CancelIds = new HashSet<int>();
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

        private void SetTexture(ISetTexture2dObject setTexture2dObject, Texture2D texture, int serialId)
        {
            m_LoadTextureObjectsLinkedList.AddLast(LoadTextureObject.Create(setTexture2dObject, texture));
            if (!m_CancelIds.Contains(serialId))
            {
                setTexture2dObject.SetTexture(texture);
            }
            else
            {
                m_CancelIds.Remove(serialId);
            }
        }
        
        private void SetTexture(ISetTexture2dObject setTexture2dObject, Texture2D texture)
        {
            m_LoadTextureObjectsLinkedList.AddLast(LoadTextureObject.Create(setTexture2dObject, texture));
            setTexture2dObject.SetTexture(texture);
        }

        /// <summary>
        /// 取消设置图片。
        /// </summary>
        /// <param name="id"></param>
        public void CancelSetTexture(int id)
        {
            if (id < 0)
            {
                Log.Error($"Cancel Id:{id} is  not invalid! id must >= 0");
                return;
            }

            m_CancelIds.Add(id);
        }
    }
}