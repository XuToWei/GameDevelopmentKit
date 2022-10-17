using System;
using GameFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace UGF
{
    [Serializable]
    public class WaitSetImage : ISetSpriteObject
    {
        [ShowInInspector]
        private Image m_Image;

        public static WaitSetImage Create(Image obj, string collection, string spriteName)
        {
            WaitSetImage waitSetImage = ReferencePool.Acquire<WaitSetImage>();
            waitSetImage.m_Image = obj;
            waitSetImage.SpritePath = spriteName;
            waitSetImage.CollectionPath = collection;
            int index1 = waitSetImage.SpritePath.LastIndexOf("/", StringComparison.Ordinal);
            int index2 = waitSetImage.SpritePath.LastIndexOf(".", StringComparison.Ordinal);
            waitSetImage.SpriteName = index2 < index1
                ? waitSetImage.SpritePath.Substring(index1 + 1)
                : waitSetImage.SpritePath.Substring(index1 + 1, index2 - index1 - 1);
            return waitSetImage;
        }
        
        [ShowInInspector]
        public string SpritePath { get; private set; }
        
        [ShowInInspector]
        public string CollectionPath { get; private set; }
        
        [ShowInInspector]
        private string SpriteName { get; set; }

        public void SetSprite(Sprite sprite)
        {
            if (m_Image != null)
            {
                m_Image.sprite = sprite;
            }
        }

        public bool IsCanRelease()
        {
            return m_Image == null || m_Image.sprite == null || m_Image.sprite.name != SpriteName;
        }
        
        public void Clear()
        {
            m_Image = null;
            SpritePath = null;
            CollectionPath = null;
            SpriteName = null;
        }
    }
}