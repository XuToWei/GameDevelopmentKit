using System.Collections.Generic;

namespace UnityGameFramework.Extension
{
    public partial class SpriteCollectionComponent
    {
        /// <summary>
        /// 设置精灵
        /// </summary>
        /// <param name="setSpriteObject">需要设置精灵的对象</param>
        public async void SetSpriteAsync(ISetSpriteObject setSpriteObject)
        {
            if (m_SpriteCollectionPool.CanSpawn(setSpriteObject.CollectionPath))
            {
                SpriteCollection collectionItem = (SpriteCollection)m_SpriteCollectionPool.Spawn(setSpriteObject.CollectionPath).Target;
                setSpriteObject.SetSprite(collectionItem.GetSprite(setSpriteObject.SpritePath));
                m_LoadSpriteObjectsLinkedList.AddLast(new LoadSpriteObject(setSpriteObject, collectionItem));
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
            SpriteCollection collection = await m_ResourceComponent.LoadAssetAsync<SpriteCollection>(setSpriteObject.CollectionPath);
            m_SpriteCollectionPool.Register(SpriteCollectionItemObject.Create(setSpriteObject.CollectionPath, collection,m_ResourceComponent), false);
            m_SpriteCollectionBeingLoaded.Remove(setSpriteObject.CollectionPath);
            m_WaitSetObjects.TryGetValue(setSpriteObject.CollectionPath, out LinkedList<ISetSpriteObject> awaitSetImages);
            LinkedListNode<ISetSpriteObject> current = awaitSetImages?.First;
            while (current != null)
            {
                m_SpriteCollectionPool.Spawn(setSpriteObject.CollectionPath);
                current.Value.SetSprite(collection.GetSprite(current.Value.SpritePath));
                m_LoadSpriteObjectsLinkedList.AddLast(new LoadSpriteObject(current.Value, collection));
                current = current.Next;
            }
        }
    }
}