using System;
using GameFramework;
using Sirenix.OdinInspector;

namespace UnityGameFramework.Extension
{
    public partial class SpriteCollectionComponent
    {
        [Serializable]
        public sealed class LoadSpriteObject : IReference
        {
            [ShowInInspector]
            public ISetSpriteObject SpriteObject { get; private set; }

            [ShowInInspector]
            public SpriteCollection Collection { get; private set; }

            public static LoadSpriteObject Create(ISetSpriteObject obj, SpriteCollection collection)
            {
                LoadSpriteObject loadSpriteObject = ReferencePool.Acquire<LoadSpriteObject>();
                loadSpriteObject.SpriteObject = obj;
                loadSpriteObject.Collection = collection;
                return loadSpriteObject;
            }

            public void Clear()
            {
                SpriteObject = default;
                Collection = default;
            }
        }
    }
}