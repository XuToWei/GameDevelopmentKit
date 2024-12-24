using System;
using GameFramework;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UnityGameFramework.Extension
{
    public partial class TextureSetComponent
    {
        [Serializable]
        public class LoadTextureObject : IReference
        {
            [ShowInInspector]
            public ISetTexture2dObject Texture2dObject { get; private set; }

            [ShowInInspector]
            public Texture2D Texture2D { get; private set; }

            private LoadTextureObject(ISetTexture2dObject obj,Texture2D texture2D)
            {
                Texture2dObject = obj;
                Texture2D = texture2D;
            }

            public LoadTextureObject()
            {
            }

            public static LoadTextureObject Create(ISetTexture2dObject obj, Texture2D texture2D)
            {
                var loadTextureObject = ReferencePool.Acquire<LoadTextureObject>();
                loadTextureObject.Texture2dObject = obj;
                loadTextureObject.Texture2D = texture2D;
                return loadTextureObject;
            }

            public void Clear()
            {
                Texture2dObject = null;
                Texture2D = null;
            }
        }
    }
}