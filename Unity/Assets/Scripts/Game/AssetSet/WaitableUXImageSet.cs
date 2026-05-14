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
    public class WaitableUXImageSet : AssetSet<Sprite>
    {
        [ShowInInspector]
        private UXImage m_UXImage;
        [ShowInInspector]
        private Sprite m_CurSprite;

        private AutoResetUniTaskCompletionSource m_Tcs;

        public static WaitableUXImageSet Create(UXImage uxImage, string spritePath, AutoResetUniTaskCompletionSource tcs)
        {
            WaitableUXImageSet waitableUXImageSet = ReferencePool.Acquire<WaitableUXImageSet>();
            waitableUXImageSet.m_UXImage = uxImage;
            waitableUXImageSet.m_Tcs = tcs;
            waitableUXImageSet.AssetPath = spritePath;
            waitableUXImageSet.Target = uxImage;
            return waitableUXImageSet;
        }

        public override void SetAsset(Sprite asset)
        {
            if (m_UXImage != null)
            {
                m_UXImage.sprite = asset;
                m_CurSprite = asset;
            }

            if (m_Tcs != null)
            {
                m_Tcs.TrySetResult();
                m_Tcs = null;
            }
        }

        public override bool IsCanRelease()
        {
            return m_UXImage == null || m_UXImage.sprite != m_CurSprite && m_CurSprite != null;
        }

        public override void Clear()
        {
            base.Clear();
            m_UXImage = null;
            m_CurSprite = null;
            if (m_Tcs != null)
            {
                m_Tcs.TrySetCanceled();
                m_Tcs = null;
            }
        }
    }
}