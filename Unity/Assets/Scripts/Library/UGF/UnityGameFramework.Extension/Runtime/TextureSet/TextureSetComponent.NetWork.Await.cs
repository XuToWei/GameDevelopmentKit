using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameFramework;
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
            string texturePath = setTexture2dObject.Texture2dFilePath;
            if (m_TexturePool.CanSpawn(texturePath))
            {
                Texture2D texture = (Texture2D)m_TexturePool.Spawn(texturePath).Target;
                SetTexture(setTexture2dObject, texture);
                return true;
            }

            // 本地文件系统有，优先从本地加载
            if (!string.IsNullOrEmpty(saveFilePath) && HasFile(saveFilePath))
            {
                SetTextureByFileSystem(setTexture2dObject, saveFilePath);
                return true;
            }

            if (!m_WaitSetObjects.TryGetValue(texturePath, out var awaitSets))
            {
                awaitSets = new HashSet<ISetTexture2dObject>();
                m_WaitSetObjects.Add(texturePath, awaitSets);
            }
            awaitSets.Add(setTexture2dObject);

            AutoResetUniTaskCompletionSource<bool> tcs;
            if (!m_TextureBeingLoaded.Add(texturePath))
            {
                if (m_TextureLoadingTcs.TryGetValue(texturePath, out tcs))
                {
                    return await tcs.Task;
                }
                else
                {
                    tcs = AutoResetUniTaskCompletionSource<bool>.Create();
                    m_TextureLoadingTcs.Add(texturePath, tcs);
                    return await tcs.Task;
                }
            }

            (bool isCanceled, WebRequestResult data) = await m_WebRequestComponent.WebRequestAsync(texturePath, cancellationToken: cancellationToken).SuppressCancellationThrow();
            m_TextureBeingLoaded.Remove(texturePath);

            if (isCanceled)
            {
                awaitSets.Remove(setTexture2dObject);
                ReferencePool.Release(setTexture2dObject);
                if (m_TextureLoadingTcs.Remove(texturePath, out tcs))
                {
                    tcs.TrySetResult(false);
                }
                return false;
            }

            if (!data.IsError)
            {
                Texture2D texture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
                texture.LoadImage(data.Bytes);
                if (!string.IsNullOrEmpty(saveFilePath))
                {
                    SaveTexture(saveFilePath, data.Bytes);
                }
                m_TexturePool.Register(TextureItemObject.Create(texturePath, texture, TextureLoad.FromNet), false);

                if (m_WaitSetObjects.TryGetValue(texturePath, out HashSet<ISetTexture2dObject> waitSets))
                {
                    foreach (ISetTexture2dObject awaitSet in waitSets)
                    {
                        m_TexturePool.Spawn(texturePath);
                        SetTexture(awaitSet, texture);
                    }
                    waitSets.Clear();
                }
                if (m_TextureLoadingTcs.Remove(texturePath, out tcs))
                {
                    tcs.TrySetResult(true);
                }
                return true;
            }
            else
            {
                if (m_WaitSetObjects.TryGetValue(texturePath, out HashSet<ISetTexture2dObject> waitSets))
                {
                    foreach (var awaitSet in waitSets)
                    {
                        ReferencePool.Release(awaitSet);
                    }
                    waitSets.Clear();
                }
                if (m_TextureLoadingTcs.Remove(texturePath, out tcs))
                {
                    tcs.TrySetResult(false);
                }
                return false;
            }
        }
    }
}