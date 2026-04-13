using System;
using Cysharp.Threading.Tasks;
using GameFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Extension;

namespace Game
{
    [Serializable]
    public class WaitableSetUXImage : ISetSpriteObject
    {
        [ShowInInspector]
        private UXImage m_UXImage;

        private AutoResetUniTaskCompletionSource m_Tcs;

        public static WaitableSetUXImage Create(UXImage obj, string collection, string spritePath, AutoResetUniTaskCompletionSource tcs)
        {
            WaitableSetUXImage waitSetImage = ReferencePool.Acquire<WaitableSetUXImage>();
            waitSetImage.m_UXImage = obj;
            waitSetImage.m_Tcs = tcs;
            waitSetImage.SpritePath = spritePath;
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

            if (m_Tcs != null)
            {
                m_Tcs.TrySetResult();
                m_Tcs = null;
            }
        }

        public bool IsCanRelease()
        {
            return m_UXImage == null || m_UXImage.sprite != CurSprite && CurSprite != null;
        }

        public void Clear()
        {
            m_UXImage = null;
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