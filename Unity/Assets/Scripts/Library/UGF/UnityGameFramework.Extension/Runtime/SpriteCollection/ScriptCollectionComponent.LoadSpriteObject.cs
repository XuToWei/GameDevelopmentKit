using System;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace UnityGameFramework.Extension
{
    public partial class SpriteCollectionComponent
    {
        [Serializable]
        public class LoadSpriteObject
        {
#if ODIN_INSPECTOR
            [ShowInInspector]
#endif
            public ISetSpriteObject SpriteObject { get; }
#if ODIN_INSPECTOR
            [ShowInInspector]
#endif
            public SpriteCollection Collection { get; }
#if UNITY_EDITOR
            public bool IsSelect { get; set; }
#endif

            public LoadSpriteObject(ISetSpriteObject obj, SpriteCollection collection)
            {
                SpriteObject = obj;
                Collection = collection;
            }
        }
    }
}