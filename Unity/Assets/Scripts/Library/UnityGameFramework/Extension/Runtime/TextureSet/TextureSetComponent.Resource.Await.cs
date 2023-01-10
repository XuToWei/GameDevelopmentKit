using GameFramework;
using UnityEngine;

namespace UnityGameFramework.Extension
{
    public partial class TextureSetComponent
    {
        /// <summary>
        /// 通过资源系统设置图片
        /// </summary>
        /// <param name="setTexture2dObject">需要设置图片的对象</param>
        /// <param name="cancelAction">取消图片设置函数</param>
        public async void SetTextureByResourcesAsync(ISetTexture2dObject setTexture2dObject, GameFrameworkAction cancelAction = null)
        {
            Texture2D texture;
            int serialId = -1;
            if (m_TexturePool.CanSpawn(setTexture2dObject.Texture2dFilePath))
            {
                texture = (Texture2D)m_TexturePool.Spawn(setTexture2dObject.Texture2dFilePath).Target;
            }
            else
            {
                serialId = m_SerialId++;

                void Cancel()
                {
                    CancelSetTexture(serialId);
                }

                try
                {
                    if (cancelAction != null)
                    {
                        cancelAction += Cancel;
                    }

                    texture = await m_ResourceComponent.LoadAssetAsync<Texture2D>(setTexture2dObject.Texture2dFilePath);
                    if (!m_TexturePool.CanSpawn(setTexture2dObject.Texture2dFilePath))
                    {
                        m_TexturePool.Register(
                            TextureItemObject.Create(setTexture2dObject.Texture2dFilePath, texture,
                                TextureLoad.FromResource, m_ResourceComponent), true);
                    }
                    else
                    {
                        m_TexturePool.Spawn(setTexture2dObject.Texture2dFilePath);
                    }
                }
                finally
                {
                    if (cancelAction != null)
                    {
                        cancelAction -= Cancel;
                    }
                }
            }

            SetTexture(setTexture2dObject, texture, serialId);
        }
    }
}