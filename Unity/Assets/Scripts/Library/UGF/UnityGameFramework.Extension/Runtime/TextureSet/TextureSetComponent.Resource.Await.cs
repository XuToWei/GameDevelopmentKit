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
        /// 通过资源系统设置图片
        /// </summary>
        /// <param name="setTexture2dObject">需要设置图片的对象</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>是否成功</returns>
        public async UniTask<bool> SetTextureByResourcesAsync(ISetTexture2dObject setTexture2dObject, CancellationToken cancellationToken = default)
        {
            string texturePath = setTexture2dObject.Texture2dFilePath;
            if (m_TexturePool.CanSpawn(texturePath))
            {
                Texture2D texture = (Texture2D)m_TexturePool.Spawn(texturePath).Target;
                SetTexture(setTexture2dObject, texture);
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

            (bool isCanceled, Texture2D tex) = await m_ResourceComponent.LoadAssetAsync<Texture2D>(texturePath, cancellationToken: cancellationToken).SuppressCancellationThrow();
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

            m_TexturePool.Register(TextureItemObject.Create(texturePath, tex, TextureLoad.FromResource, m_ResourceComponent), false);

            if (m_WaitSetObjects.TryGetValue(texturePath, out HashSet<ISetTexture2dObject> waitSets))
            {
                foreach (ISetTexture2dObject awaitSet in waitSets)
                {
                    m_TexturePool.Spawn(texturePath);
                    SetTexture(awaitSet, tex);
                }
                waitSets.Clear();
            }
            if (m_TextureLoadingTcs.Remove(texturePath, out tcs))
            {
                tcs.TrySetResult(true);
            }
            return true;
        }
    }
}