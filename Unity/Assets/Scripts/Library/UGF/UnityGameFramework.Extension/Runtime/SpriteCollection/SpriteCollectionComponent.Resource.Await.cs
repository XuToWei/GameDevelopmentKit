using System.Collections.Generic;
using System.Threading;
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
        public async UniTask SetSpriteAsync(ISetSpriteObject setSpriteObject, CancellationTokenSource cts = null)
        {
            if (cts is { IsCancellationRequested: true })
                return;
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
            
            CancellationTokenRegistration? ctr = cts?.Token.Register(() =>
            {
                loadSp.Remove(setSpriteObject);
            });

            if (m_SpriteCollectionBeingLoaded.Contains(setSpriteObject.CollectionPath))
            {
                return;
            }

            m_SpriteCollectionBeingLoaded.Add(setSpriteObject.CollectionPath);
            SpriteCollection collection = await m_ResourceComponent.LoadAssetAsync<SpriteCollection>(setSpriteObject.CollectionPath, cts: cts);
            
            if (cts is { IsCancellationRequested: true })
            {
                return;
            }
            ctr?.Dispose();
            
            m_SpriteCollectionPool.Register(SpriteCollectionItemObject.Create(setSpriteObject.CollectionPath, collection,m_ResourceComponent), false);
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
    }
}