using System;
using Cysharp.Threading.Tasks;
using GameFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace UnityGameFramework.Extension
{
    [Serializable]
    public class WaitableSetRawImage : ISetTexture2dObject
    {
        [ShowInInspector]
        private RawImage m_RawImage;

        private AutoResetUniTaskCompletionSource m_Tcs;

        [ShowInInspector]
        public string Texture2dFilePath { get; private set; }

        [ShowInInspector]
        public Texture2D CurTexture2D { get; private set; }

        public void SetTexture(Texture2D texture)
        {
            if (m_RawImage != null)
            {
                m_RawImage.texture = texture;
                CurTexture2D = texture;
            }

            if (m_Tcs != null)
            {
                m_Tcs.TrySetResult();
                m_Tcs = null;
            }
        }

        public bool IsCanRelease()
        {
            return m_RawImage == null || m_RawImage.texture == null || m_RawImage.texture != CurTexture2D;
        }

        public static WaitableSetRawImage Create(RawImage rawImage, string filePath, AutoResetUniTaskCompletionSource tcs)
        {
            WaitableSetRawImage item = ReferencePool.Acquire<WaitableSetRawImage>();
            item.m_RawImage = rawImage;
            item.m_Tcs = tcs;
            item.Texture2dFilePath = filePath;
            return item;
        }

        public void Clear()
        {
            m_RawImage = null;
            if (m_Tcs != null)
            {
                m_Tcs.TrySetCanceled();
                m_Tcs = null;
            }
            CurTexture2D = null;
            Texture2dFilePath = null;
        }
    }
}