using System;
using GameFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Extension;

namespace Game
{
    [Serializable]
    public class WaitSetUXImage : ISetSpriteObject
    {
        [ShowInInspector]
        private UXImage m_UXImage;

        public static WaitSetUXImage Create(UXImage obj, string collection, string spriteName)
        {
            WaitSetUXImage waitSetImage = ReferencePool.Acquire<WaitSetUXImage>();
            waitSetImage.m_UXImage = obj;
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
            if (m_UXImage != null)
            {
                m_UXImage.sprite = sprite;
                CurSprite = sprite;
            }
        }

        public bool IsCanRelease()
        {
            return m_UXImage == null || m_UXImage.sprite != CurSprite && CurSprite != null;
        }

        public void Clear()
        {
            m_UXImage = null;
            SpritePath = null;
            CollectionPath = null;
            CurSprite = null;
        }
    }
}