using System;
using GameFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace UnityGameFramework.Extension
{
    [Serializable]
    public class WaitSetRawImage : ISetTexture2dObject
    {
        [ShowInInspector]
        private RawImage m_RawImage;

        [ShowInInspector] 
        private Texture2D Texture2D { get; set; }

        [ShowInInspector]
        public string Texture2dFilePath { get; private set; }

        public void SetTexture(Texture2D texture)
        {
            if (m_RawImage != null)
            {
                m_RawImage.texture = texture;
                Texture2D = texture;
            }
        }

        public bool IsCanRelease()
        {
            return m_RawImage == null || m_RawImage.texture == null || m_RawImage.texture != Texture2D;
        }

        public static WaitSetRawImage Create(RawImage rawImage, string filePath)
        {
            WaitSetRawImage item = ReferencePool.Acquire<WaitSetRawImage>();
            item.m_RawImage = rawImage;
            item.Texture2dFilePath = filePath;
            return item;
        }

        public void Clear()
        {
            m_RawImage = null;
            Texture2D = null;
            Texture2dFilePath = null;
        }
    }
}