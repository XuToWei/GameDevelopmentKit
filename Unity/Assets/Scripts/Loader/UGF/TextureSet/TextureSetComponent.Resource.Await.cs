using ET;
using UGF;
using UnityEngine;

namespace UGF
{
    public partial class TextureSetComponent
    {
        /// <summary>
        /// 通过资源系统设置图片
        /// </summary>
        /// <param name="setTexture2dObject">需要设置图片的对象</param>
        public async void SetTextureByResourcesAsync(ISetTexture2dObject setTexture2dObject,ETCancellationToken cancellationToken = null)
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
                    cancellationToken?.Add(Cancel);
                    texture = await m_ResourceComponent.LoadAssetAsync<Texture2D>(setTexture2dObject.Texture2dFilePath);
                    m_TexturePool.Register(
                        TextureItemObject.Create(setTexture2dObject.Texture2dFilePath, texture,
                            TextureLoad.FromResource, m_ResourceComponent), true);
                }
                finally
                {
                    cancellationToken?.Remove(Cancel);
                }
            }

            SetTexture(setTexture2dObject, texture,serialId);
        }
    }
}