using System;
using GameFramework;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace UnityGameFramework.Extension
{
    public partial class SpriteCollectionComponent
    {
        [Serializable]
        public sealed class LoadSpriteObject : IReference
        {
#if ODIN_INSPECTOR
            [ShowInInspector]
#endif
            public ISetSpriteObject SpriteObject { get; private set; }
#if ODIN_INSPECTOR
            [ShowInInspector]
#endif
            public SpriteCollection Collection { get; private set; }
#if UNITY_EDITOR
            public bool IsSelect { get; set; }
#endif

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