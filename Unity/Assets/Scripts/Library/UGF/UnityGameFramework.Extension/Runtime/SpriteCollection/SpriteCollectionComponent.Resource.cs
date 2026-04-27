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
            ISetSpriteObject setSpriteObject = (ISetSpriteObject)userdata;
            string collectionPath = setSpriteObject.CollectionPath;
            m_SpriteCollectionBeingLoaded.Remove(collectionPath);
            if (m_WaitSetObjects.Remove(collectionPath, out UGFHashSet<ISetSpriteObject> awaitSets))
            {
                foreach (var awaitSet in awaitSets)
                {
                    ReferencePool.Release(awaitSet);
                }
                awaitSets.Dispose();
            }
            Log.Error("Can not load SpriteCollection from '{0}' with error message '{1}'.", assetName, errormessage);
        }

        private void OnLoadAssetSuccess(string assetName, object asset, float duration, object userdata)
        {
            ISetSpriteObject setSpriteObject = (ISetSpriteObject)userdata;
            SpriteCollection collection = (SpriteCollection)asset;
            string collectionPath = setSpriteObject.CollectionPath;

            var spriteCollectionItemObject = SpriteCollectionItemObject.Create(collectionPath, collection, m_ResourceComponent);
            spriteCollectionItemObject.Locked = true; //防止Register时被对象池自动释放
            m_SpriteCollectionPool.Register(spriteCollectionItemObject, false);
            m_SpriteCollectionBeingLoaded.Remove(collectionPath);

            if (m_WaitSetObjects.Remove(collectionPath, out UGFHashSet<ISetSpriteObject> awaitSets))
            {
                foreach (ISetSpriteObject awaitSet in awaitSets)
                {
                    if (m_SpriteCollectionPool.Spawn(collectionPath) == null)
                    {
                        throw new GameFrameworkException(Utility.Text.Format("Can not spawn SpriteCollectionItemObject for '{0}' from pool.", collectionPath));
                    }
                    awaitSet.SetSprite(collection.GetSprite(awaitSet.SpritePath));
                    m_LoadedSpriteObjectsLinkedList.AddLast(LoadSpriteObject.Create(awaitSet, collection));
                }
                awaitSets.Dispose();
            }

            spriteCollectionItemObject.Locked = false;
        }

        /// <summary>
        /// 设置精灵
        /// </summary>
        /// <param name="setSpriteObject">需要设置精灵的对象</param>
        public void SetSprite(ISetSpriteObject setSpriteObject)
        {
            string collectionPath = setSpriteObject.CollectionPath;
            if (m_SpriteCollectionPool.CanSpawn(collectionPath))
            {
                SpriteCollection collectionItem = (SpriteCollection)m_SpriteCollectionPool.Spawn(collectionPath).Target;
                setSpriteObject.SetSprite(collectionItem.GetSprite(setSpriteObject.SpritePath));
                m_LoadedSpriteObjectsLinkedList.AddLast(LoadSpriteObject.Create(setSpriteObject, collectionItem));
                return;
            }

            if (!m_WaitSetObjects.TryGetValue(collectionPath, out var loadSp))
            { 
                loadSp = UGFHashSet<ISetSpriteObject>.Create();
                m_WaitSetObjects.Add(collectionPath, loadSp);
            }
            loadSp.Add(setSpriteObject);

            if (!m_SpriteCollectionBeingLoaded.Add(collectionPath))
            {
                return;
            }

            m_ResourceComponent.LoadAsset(collectionPath, typeof(SpriteCollection), m_LoadAssetCallbacks, setSpriteObject);
        }
    }
}