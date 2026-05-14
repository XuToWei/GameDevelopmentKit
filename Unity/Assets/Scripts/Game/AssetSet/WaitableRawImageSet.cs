using System;
using Cysharp.Threading.Tasks;
using GameFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace UnityGameFramework.Extension
{
    [Serializable]
    public class WaitableRawImageSet : AssetSet<Texture2D>, ISerializeAssetSet, ISaveAbleAssetSet
    {
        [ShowInInspector]
        private RawImage m_RawImage;
        [ShowInInspector]
        private Texture2D m_CurTexture2D;
        [ShowInInspector]
        public bool NeedSave { get; private set; }

        private AutoResetUniTaskCompletionSource m_Tcs;

        public static WaitableRawImageSet Create(RawImage rawImage, string texturePath, bool needSave, AutoResetUniTaskCompletionSource tcs)
        {
            WaitableRawImageSet waitableRawImageSet = ReferencePool.Acquire<WaitableRawImageSet>();
            waitableRawImageSet.m_RawImage = rawImage;
            waitableRawImageSet.NeedSave = needSave;
            waitableRawImageSet.m_Tcs = tcs;
            waitableRawImageSet.AssetPath = texturePath;
            waitableRawImageSet.Target = rawImage;
            return waitableRawImageSet;
        }

        public override void SetAsset(Texture2D asset)
        {
            if (m_RawImage != null)
            {
                m_RawImage.texture = asset;
                m_CurTexture2D = asset;
            }

            if (m_Tcs != null)
            {
                m_Tcs.TrySetResult();
                m_Tcs = null;
            }
        }

        public override bool IsCanRelease()
        {
            return m_RawImage == null || m_RawImage.texture != m_CurTexture2D && m_CurTexture2D != null;
        }

        public override void Clear()
        {
            base.Clear();
            m_RawImage = null;
            m_CurTexture2D = null;
            NeedSave = false;
            if (m_Tcs != null)
            {
                m_Tcs.TrySetCanceled();
                m_Tcs = null;
            }
        }

        public Object Serialize(byte[] bytes)
        {
            Texture2D tex = new Texture2D(0, 0, TextureFormat.RGBA32, false);
            tex.LoadImage(bytes);
            return tex;
        }
    }
}