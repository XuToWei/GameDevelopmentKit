using System;
using GameFramework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace UnityGameFramework.Extension
{
    [Serializable]
    public class RawImageSet : AssetSet<Texture2D>, ISerializeAssetSet, ISaveAbleAssetSet
    {
        [ShowInInspector]
        private RawImage m_RawImage;
        [ShowInInspector]
        private Texture2D m_CurTexture2D;
        [ShowInInspector]
        public bool NeedSave { get; private set; }

        public static RawImageSet Create(RawImage rawImage, string texturePath, bool needSave)
        {
            RawImageSet rawImageSet = ReferencePool.Acquire<RawImageSet>();
            rawImageSet.m_RawImage = rawImage;
            rawImageSet.AssetPath = texturePath;
            rawImageSet.Target = rawImage;
            rawImageSet.NeedSave = needSave;
            return rawImageSet;
        }

        public override void SetAsset(Texture2D asset)
        {
            if (m_RawImage != null)
            {
                m_RawImage.texture = asset;
                m_CurTexture2D = asset;
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
        }

        public Object Serialize(byte[] bytes)
        {
            Texture2D tex = new Texture2D(0, 0, TextureFormat.RGBA32, false);
            tex.LoadImage(bytes);
            return tex;
        }
    }
}