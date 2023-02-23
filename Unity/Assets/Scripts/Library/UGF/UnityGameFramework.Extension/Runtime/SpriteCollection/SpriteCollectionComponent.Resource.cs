using System.Collections.Generic;
using GameFramework;
using GameFramework.Resource;
using UnityGameFramework.Runtime;

namespace UnityGameFramework.Extension
{
    public partial class SpriteCollectionComponent
    {
        /// <summary>
        /// 资源组件
        /// </summary>
        private ResourceComponent m_ResourceComponent;

        private LoadAssetCallbacks m_LoadAssetCallbacks;

        private void InitializedResources()
        {
            m_ResourceComponent = GameEntry.GetComponent<ResourceComponent>();
            m_LoadAssetCallbacks = new LoadAssetCallbacks(OnLoadAssetSuccess, OnLoadAssetFailure);
        }

        private void OnLoadAssetFailure(string assetName, LoadResourceStatus status, string errormessage, object userdata)
        {
            Log.Error("Can not load SpriteCollection from '{0}' with error message '{1}'.", assetName, errormessage);
        }

        private void OnLoadAssetSuccess(string assetName, object asset, float duration, object userdata)
        {
            ISetSpriteObject setSpriteObject = (ISetSpriteObject)userdata;
            SpriteCollection collection = (SpriteCollection)asset;
            m_SpriteCollectionPool.Register(SpriteCollectionItemObject.Create(setSpriteObject.CollectionPath, collection, m_ResourceComponent), false);
            m_SpriteCollectionBeingLoaded.Remove(setSpriteObject.CollectionPath);
            m_WaitSetObjects.TryGetValue(setSpriteObject.CollectionPath, out LinkedList<ISetSpriteObject> awaitSetImages);
            LinkedListNode<ISetSpriteObject> current = awaitSetImages?.First;
            while (current != null)
            {
                if (m_SpriteObjectsToReleaseOnLoad.Contains(current.Value))
                {
                    ReferencePool.Release(current.Value);
                }
                else
                {
                    m_SpriteCollectionPool.Spawn(setSpriteObject.CollectionPath);
                    current.Value.SetSprite(collection.GetSprite(current.Value.SpritePath));
                    m_LoadedSpriteObjectsLinkedList.AddLast(LoadSpriteObject.Create(current.Value, collection));
                }
                current = current.Next;
            }
        }
        
        /// <summary>
        /// 设置精灵
        /// </summary>
        /// <param name="setSpriteObject">需要设置精灵的对象</param>
        public void SetSprite(ISetSpriteObject setSpriteObject)
        {
            if (m_SpriteCollectionPool.CanSpawn(setSpriteObject.CollectionPath))
            {
                SpriteCollection collectionItem = (SpriteCollection)m_SpriteCollectionPool.Spawn(setSpriteObject.CollectionPath).Target;
                setSpriteObject.SetSprite(collectionItem.GetSprite(setSpriteObject.SpritePath));
                m_LoadedSpriteObjectsLinkedList.AddLast(LoadSpriteObject.Create(setSpriteObject, collectionItem));
                return;
            }

            if (m_WaitSetObjects.ContainsKey(setSpriteObject.CollectionPath))
            {
                var loadSp = m_WaitSetObjects[setSpriteObject.CollectionPath];
                loadSp.AddLast(setSpriteObject);
            }
            else
            {
                var loadSp = new LinkedList<ISetSpriteObject>();
                loadSp.AddFirst(setSpriteObject);
                m_WaitSetObjects.Add(setSpriteObject.CollectionPath, loadSp);
            }

            if (m_SpriteCollectionBeingLoaded.Contains(setSpriteObject.CollectionPath))
            {
                return;
            }

            m_SpriteCollectionBeingLoaded.Add(setSpriteObject.CollectionPath);
            m_ResourceComponent.LoadAsset(setSpriteObject.CollectionPath, typeof(SpriteCollection), m_LoadAssetCallbacks, setSpriteObject);
        }

        public void RemoveLoadingSetSprite(ISetSpriteObject setSpriteObject)
        {
            if (m_SpriteObjectsToReleaseOnLoad.Contains(setSpriteObject))
            {
                throw new GameFrameworkException("SetSprite already added ToReleaseOnLoad.");
            }
            m_SpriteObjectsToReleaseOnLoad.Add(setSpriteObject);
        }
        
        public void RemoveAllLoadingSetSprite()
        {
            foreach (var loadSp in m_WaitSetObjects.Values)
            {
                LinkedListNode<ISetSpriteObject> current = loadSp?.First;
                while (current != null)
                {
                    ReferencePool.Release(current.Value);
                    current = current.Next;
                }
            }
            
            m_SpriteObjectsToReleaseOnLoad.Clear();
            m_WaitSetObjects.Clear();
        }
    }
}