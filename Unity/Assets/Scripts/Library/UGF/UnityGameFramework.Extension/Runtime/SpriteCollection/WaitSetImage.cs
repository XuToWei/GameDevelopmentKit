using System;
using GameFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace UnityGameFramework.Extension
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
            return waitSetImage;
        }

        [ShowInInspector]
        public string SpritePath { get; private set; }

        [ShowInInspector]
        public string CollectionPath { get; private set; }

        [ShowInInspector]
        public Sprite CurSprite { get; private set; }

        public void SetSprite(Sprite sprite)
        {
            if (m_Image != null)
            {
                m_Image.sprite = sprite;
                CurSprite = sprite;
            }
        }

        public bool IsCanRelease()
        {
            return m_Image == null || m_Image.sprite != CurSprite && CurSprite != null;
        }

        public void Clear()
        {
            m_Image = null;
            SpritePath = null;
            CollectionPath = null;
            CurSprite = null;
        }
    }
}