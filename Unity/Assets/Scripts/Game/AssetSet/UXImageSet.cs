using System;
using GameFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Extension;

namespace Game
{
    [Serializable]
    public class UXImageSet : AssetSet<Sprite>
    {
        [ShowInInspector]
        private UXImage m_UXImage;
        [ShowInInspector]
        private Sprite m_CurSprite;

        public static UXImageSet Create(UXImage uxImage, string spritePath)
        {
            UXImageSet uxImageSet = ReferencePool.Acquire<UXImageSet>();
            uxImageSet.m_UXImage = uxImage;
            uxImageSet.AssetPath = spritePath;
            uxImageSet.Target = uxImage;
            return uxImageSet;
        }

        public override void SetAsset(Sprite asset)
        {
            if (m_UXImage != null)
            {
                m_UXImage.sprite = asset;
                m_CurSprite = asset;
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
        }
    }
}