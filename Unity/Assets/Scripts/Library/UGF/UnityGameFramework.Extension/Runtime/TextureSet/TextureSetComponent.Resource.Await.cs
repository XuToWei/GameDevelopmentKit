using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UnityGameFramework.Extension
{
    public partial class TextureSetComponent
    {
        /// <summary>
        /// 通过资源系统设置图片
        /// </summary>
        /// <param name="setTexture2dObject">需要设置图片的对象</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>是否成功</returns>
        public async UniTask SetTextureByResourcesAsync(ISetTexture2dObject setTexture2dObject, CancellationToken cancellationToken = default)
        {
            if (m_TexturePool.CanSpawn(setTexture2dObject.Texture2dFilePath))
            {
                Texture2D texture = (Texture2D)m_TexturePool.Spawn(setTexture2dObject.Texture2dFilePath).Target;
                SetTexture(setTexture2dObject, texture);
            }
            else
            {
                int serialId = m_SerialId++;

                (bool isCanceled, Texture2D texture) = await m_ResourceComponent.LoadAssetAsync<Texture2D>(setTexture2dObject.Texture2dFilePath, cancellationToken: cancellationToken).SuppressCancellationThrow();
                
                if (isCanceled)
                {
                    CancelSetTexture(serialId);
                    return;
                }

                if (!m_TexturePool.CanSpawn(setTexture2dObject.Texture2dFilePath))
                {
                    m_TexturePool.Register(TextureItemObject.Create(
                        setTexture2dObject.Texture2dFilePath, texture, TextureLoad.FromResource, m_ResourceComponent), true);
                }
                else
                {
                    m_TexturePool.Spawn(setTexture2dObject.Texture2dFilePath);
                }

                SetTexture(setTexture2dObject, texture, serialId);
            }
        }
    }
}