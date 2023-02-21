using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace UnityGameFramework.Extension
{
    public partial class SpriteCollectionComponent
    {
        /// <summary>
        /// 设置精灵
        /// </summary>
        /// <param name="setSpriteObject">需要设置精灵的对象</param>
        public async UniTaskVoid SetSpriteAsync(ISetSpriteObject setSpriteObject, CancellationTokenSource cts = default)
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

            LinkedList<ISetSpriteObject> loadSp;
            if (m_WaitSetObjects.ContainsKey(setSpriteObject.CollectionPath))
            { 
                loadSp = m_WaitSetObjects[setSpriteObject.CollectionPath];
                loadSp.AddLast(setSpriteObject);
            }
            else
            { 
                loadSp = new LinkedList<ISetSpriteObject>();
                loadSp.AddFirst(setSpriteObject);
                m_WaitSetObjects.Add(setSpriteObject.CollectionPath, loadSp);
            }
            
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
            ctr?.Dispose();
            if (collection == null)
                return;
            m_SpriteCollectionPool.Register(SpriteCollectionItemObject.Create(setSpriteObject.CollectionPath, collection,m_ResourceComponent), false);
            m_SpriteCollectionBeingLoaded.Remove(setSpriteObject.CollectionPath);
            m_WaitSetObjects.TryGetValue(setSpriteObject.CollectionPath, out LinkedList<ISetSpriteObject> awaitSetImages);
            LinkedListNode<ISetSpriteObject> current = awaitSetImages?.First;
            while (current != null)
            {
                m_SpriteCollectionPool.Spawn(setSpriteObject.CollectionPath);
                current.Value.SetSprite(collection.GetSprite(current.Value.SpritePath));
                m_LoadedSpriteObjectsLinkedList.AddLast(LoadSpriteObject.Create(current.Value, collection));
                current = current.Next;
            }
        }
    }
}