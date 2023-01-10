using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using GameFramework;
using GameFramework.Event;
using GameFramework.FileSystem;
using GameFramework.ObjectPool;
using GameFramework.Resource;
using UnityEngine;
using UnityGameFramework.Runtime;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace UnityGameFramework.Extension
{
    public partial class TextureSetComponent : GameFrameworkComponent
    {
        /// <summary>
        /// 检查是否可以释放间隔
        /// </summary>
        [SerializeField] private float m_CheckCanReleaseInterval = 30f;

        private float m_CheckCanReleaseTime = 0.0f;

        /// <summary>
        /// 对象池自动释放时间间隔
        /// </summary>
        [SerializeField] private float m_AutoReleaseInterval = 60f;

        /// <summary>
        /// 保存加载的图片对象
        /// </summary>
#if ODIN_INSPECTOR
        [ShowInInspector]
#endif
        private LinkedList<LoadTextureObject> m_LoadTextureObjectsLinkedList;

        /// <summary>
        /// 散图集合对象池
        /// </summary>
        private IObjectPool<TextureItemObject> m_TexturePool;

#if UNITY_EDITOR
        public LinkedList<LoadTextureObject> LoadTextureObjectsLinkedList
        {
            get => m_LoadTextureObjectsLinkedList;
            set => m_LoadTextureObjectsLinkedList = value;
        }
#endif
        /// <summary>
        /// 序号
        /// </summary>
        private int m_SerialId = 0;

        /// <summary>
        /// 取消加载序号集合
        /// </summary>
        public HashSet<int> m_CancelId;

        private IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();
            ObjectPoolComponent objectPoolComponent = GameEntry.GetComponent<ObjectPoolComponent>();
            m_TexturePool = objectPoolComponent.CreateMultiSpawnObjectPool<TextureItemObject>(
                "TexturePool",
                m_AutoReleaseInterval, 16, 60, 0);
            m_LoadTextureObjectsLinkedList = new LinkedList<LoadTextureObject>();
            m_CancelId = new HashSet<int>();
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
#if ODIN_INSPECTOR
        [Button("Release Unused")]
#endif
        public void ReleaseUnused()
        {
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

        private void SetTexture(ISetTexture2dObject setTexture2dObject, Texture2D texture, int serialId = -1)
        {
            m_LoadTextureObjectsLinkedList.AddLast(LoadTextureObject.Create(setTexture2dObject, texture));
            if (!m_CancelId.Contains(serialId))
            {
                setTexture2dObject.SetTexture(texture);
            }
            else
            {
                m_CancelId.Remove(serialId);
            }
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

            m_CancelId.Add(id);
        }
    }
}