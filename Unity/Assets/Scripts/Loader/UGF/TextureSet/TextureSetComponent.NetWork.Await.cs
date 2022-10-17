using System;
using ET;
using UGF;
using UnityEngine;

namespace UGF
{
    public partial class TextureSetComponent
    {
        /// <summary>
        /// 通过网络设置图片
        /// </summary>
        /// <param name="setTexture2dObject">需要设置图片的对象</param>
        /// <param name="saveFilePath">保存网络图片到本地的路径</param>
        public async void SetTextureByNetworkAsync(ISetTexture2dObject setTexture2dObject,string saveFilePath = null, ETCancellationToken etCancellationToken = null)
        {
            int serialId = -1;

            Texture2D texture = null;
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
                    etCancellationToken?.Add(Cancel);
                    var data = await m_WebRequestComponent.AddWebRequestAsync(setTexture2dObject.Texture2dFilePath);
                    if (!data.IsError)
                    {
                        texture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
                        texture.LoadImage(data.Bytes);
                        if (!string.IsNullOrEmpty(saveFilePath))
                        {
                            SaveTexture(saveFilePath, data.Bytes);
                        }

                        m_TexturePool.Register(
                            TextureItemObject.Create(setTexture2dObject.Texture2dFilePath, texture,
                                TextureLoad.FromNet), true);
                    }
                }
                finally
                {
                    etCancellationToken?.Remove(Cancel);
                }
               
            }

            SetTexture(setTexture2dObject, texture,serialId);
        }
    }
}