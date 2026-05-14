using System;
using GameFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace UnityGameFramework.Extension
{
    [Serializable]
    public class ImageSet : AssetSet<Sprite>
    {
        [ShowInInspector]
        private Image m_Image;
        [ShowInInspector]
        private Sprite m_CurSprite;

        public static ImageSet Create(Image image, string spritePath)
        {
            ImageSet imageSet = ReferencePool.Acquire<ImageSet>();
            imageSet.m_Image = image;
            imageSet.AssetPath = spritePath;
            imageSet.Target = image;
            return imageSet;
        }

        public override void SetAsset(Sprite asset)
        {
            if (m_Image != null)
            {
                m_Image.sprite = asset;
                m_CurSprite = asset;
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
        }
    }
}