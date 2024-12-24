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
            
            if(m_WaitSetObjects.TryGetValue(setSpriteObject.CollectionPath, out HashSet<ISetSpriteObject> awaitSets))
            {
                if (awaitSets.Count > 0)
                {
                    if (!awaitSets.Contains(setSpriteObject))
                    {
                        ReferencePool.Release(setSpriteObject);
                        return;
                    }
                    foreach (ISetSpriteObject awaitSet in awaitSets)
                    {
                        m_SpriteCollectionPool.Spawn(setSpriteObject.CollectionPath);
                        awaitSet.SetSprite(collection.GetSprite(awaitSet.SpritePath));
                        m_LoadedSpriteObjectsLinkedList.AddLast(LoadSpriteObject.Create(awaitSet, collection));
                    }
                    awaitSets.Clear();
                }
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

            if (!m_WaitSetObjects.TryGetValue(setSpriteObject.CollectionPath, out var loadSp))
            { 
                loadSp = new HashSet<ISetSpriteObject>();
                m_WaitSetObjects.Add(setSpriteObject.CollectionPath, loadSp);
            }
            loadSp.Add(setSpriteObject);

            if (!m_SpriteCollectionBeingLoaded.Add(setSpriteObject.CollectionPath))
            {
                return;
            }

            m_ResourceComponent.LoadAsset(setSpriteObject.CollectionPath, typeof(SpriteCollection), m_LoadAssetCallbacks, setSpriteObject);
        }

        public void RemoveLoadingSetSprite(ISetSpriteObject setSpriteObject)
        {
            if (m_WaitSetObjects.TryGetValue(setSpriteObject.CollectionPath, out HashSet<ISetSpriteObject> awaitSets))
            {
                if (awaitSets.Contains(setSpriteObject))
                {
                    awaitSets.Remove(setSpriteObject);
                }
            }
        }
        
        public void RemoveAllLoadingSetSprite()
        {
            foreach (var awaitSets in m_WaitSetObjects.Values)
            {
                foreach (var awaitSet in awaitSets)
                {
                    ReferencePool.Release(awaitSet);
                }
                awaitSets.Clear();
            }
        }
    }
}