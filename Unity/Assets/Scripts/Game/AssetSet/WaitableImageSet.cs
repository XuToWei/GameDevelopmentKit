using System;
using Cysharp.Threading.Tasks;
using GameFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace UnityGameFramework.Extension
{
    [Serializable]
    public class WaitableImageSet : AssetSet<Sprite>
    {
        [ShowInInspector]
        private Image m_Image;
        [ShowInInspector]
        private Sprite m_CurSprite;

        private AutoResetUniTaskCompletionSource m_Tcs;

        public static WaitableImageSet Create(Image image, string spritePath, AutoResetUniTaskCompletionSource tcs)
        {
            WaitableImageSet waitableImageSet = ReferencePool.Acquire<WaitableImageSet>();
            waitableImageSet.m_Image = image;
            waitableImageSet.m_Tcs = tcs;
            waitableImageSet.AssetPath = spritePath;
            waitableImageSet.Target = image;
            return waitableImageSet;
        }

        public override void SetAsset(Sprite asset)
        {
            if (m_Image != null)
            {
                m_Image.sprite = asset;
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
            return m_Image == null || m_Image.sprite != m_CurSprite && m_CurSprite != null;
        }

        public override void Clear()
        {
            base.Clear();
            m_Image = null;
            m_CurSprite = null;
            if (m_Tcs != null)
            {
                m_Tcs.TrySetCanceled();
                m_Tcs = null;
            }
        }
    }
}