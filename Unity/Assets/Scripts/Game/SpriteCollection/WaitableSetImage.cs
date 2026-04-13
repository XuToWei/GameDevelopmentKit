using System;
using Cysharp.Threading.Tasks;
using GameFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace UnityGameFramework.Extension
{
    [Serializable]
    public class WaitableSetImage : ISetSpriteObject
    {
        [ShowInInspector]
        private Image m_Image;

        private AutoResetUniTaskCompletionSource m_Tcs;

        public static WaitableSetImage Create(Image obj, string collection, string spritePath, AutoResetUniTaskCompletionSource tcs)
        {
            WaitableSetImage waitableSetImage = ReferencePool.Acquire<WaitableSetImage>();
            waitableSetImage.m_Image = obj;
            waitableSetImage.m_Tcs = tcs;
            waitableSetImage.SpritePath = spritePath;
            waitableSetImage.CollectionPath = collection;
            return waitableSetImage;
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

            if (m_Tcs != null)
            {
                m_Tcs.TrySetResult();
                m_Tcs = null;
            }
        }

        public bool IsCanRelease()
        {
            return m_Image == null || m_Image.sprite != CurSprite && CurSprite != null;
        }

        public void Clear()
        {
            m_Image = null;
            if (m_Tcs != null)
            {
                m_Tcs.TrySetCanceled();
                m_Tcs = null;
            }
            SpritePath = null;
            CollectionPath = null;
            CurSprite = null;
        }
    }
}