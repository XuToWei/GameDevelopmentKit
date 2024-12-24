using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameFramework;

namespace UnityGameFramework.Extension
{
    public partial class SpriteCollectionComponent
    {
        /// <summary>
        /// 设置精灵
        /// </summary>
        /// <param name="setSpriteObject">需要设置精灵的对象</param>
        public async UniTask SetSpriteAsync(ISetSpriteObject setSpriteObject)
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
                loadSp = new HashSet<ISetSpriteObject>();
                m_WaitSetObjects.Add(collectionPath, loadSp);
            }
            loadSp.Add(setSpriteObject);
            
            if (!m_SpriteCollectionBeingLoaded.Add(collectionPath))
            {
                return;
            }
            (bool isCancel, SpriteCollection collection) = await m_ResourceComponent.LoadAssetAsync<SpriteCollection>(collectionPath).SuppressCancellationThrow();
            m_SpriteCollectionBeingLoaded.Remove(collectionPath);
            if (isCancel)
            {
                loadSp.Remove(setSpriteObject);
                ReferencePool.Release(setSpriteObject);
            }
            m_SpriteCollectionPool.Register(SpriteCollectionItemObject.Create(collectionPath, collection, m_ResourceComponent), false);
            
            if(m_WaitSetObjects.TryGetValue(collectionPath, out HashSet<ISetSpriteObject> awaitSets))
            {
                if (awaitSets.Count > 0)
                {
                    foreach (ISetSpriteObject awaitSet in awaitSets)
                    {
                        m_SpriteCollectionPool.Spawn(collectionPath);
                        awaitSet.SetSprite(collection.GetSprite(awaitSet.SpritePath));
                        m_LoadedSpriteObjectsLinkedList.AddLast(LoadSpriteObject.Create(awaitSet, collection));
                    }
                    awaitSets.Clear();
                }
            }
        }
    }
}