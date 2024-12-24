using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UnityGameFramework.Extension
{
    public partial class TextureSetComponent
    {
        /// <summary>
        /// 通过网络设置图片
        /// </summary>
        /// <param name="setTexture2dObject">需要设置图片的对象</param>
        /// <param name="saveFilePath">保存网络图片到本地的路径</param>
        /// <param name="cancellationToken">CancellationToken</param>
        public async UniTask<bool> SetTextureByNetworkAsync(ISetTexture2dObject setTexture2dObject, string saveFilePath = null, CancellationToken cancellationToken = default)
        {
            Texture2D texture;
            if (m_TexturePool.CanSpawn(setTexture2dObject.Texture2dFilePath))
            {
                texture = (Texture2D)m_TexturePool.Spawn(setTexture2dObject.Texture2dFilePath).Target;
                SetTexture(setTexture2dObject, texture);
                return true;
            }
            else
            {
                int serialId = m_SerialId++;

                (bool isCanceled, WebRequestResult data) = await m_WebRequestComponent.WebRequestAsync(setTexture2dObject.Texture2dFilePath, cancellationToken: cancellationToken).SuppressCancellationThrow();

                if (isCanceled)
                {
                    CancelSetTexture(serialId);
                    return false;
                }

                if (!data.IsError)
                {
                    texture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
                    texture.LoadImage(data.Bytes);
                    if (!string.IsNullOrEmpty(saveFilePath))
                    {
                        SaveTexture(saveFilePath, data.Bytes);
                    }
                    m_TexturePool.Register(TextureItemObject.Create(setTexture2dObject.Texture2dFilePath, texture, TextureLoad.FromNet), true);
                    SetTexture(setTexture2dObject, texture, serialId);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}