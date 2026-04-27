using System.Collections.Generic;
using GameFramework;
using GameFramework.Event;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace UnityGameFramework.Extension
{
    public partial class TextureSetComponent
    {
        private WebRequestComponent m_WebRequestComponent;
     

        private void InitializedWeb()
        {
            m_WebRequestComponent = GameEntry.GetComponent<WebRequestComponent>();
            EventComponent eventComponent = GameEntry.GetComponent<EventComponent>();
            eventComponent.Subscribe(WebRequestSuccessEventArgs.EventId,OnWebGetTextureSuccess);
            eventComponent.Subscribe(WebRequestFailureEventArgs.EventId,OnWebGetTextureFailure);
        }
        /// <summary>
        /// 通过网络设置图片
        /// </summary>
        /// <param name="setTexture2dObject">需要设置图片的对象</param>
        /// <param name="saveFilePath">保存网络图片到本地的路径</param>
        public void SetTextureByNetwork(ISetTexture2dObject setTexture2dObject, string saveFilePath = null)
        {
            string texturePath = setTexture2dObject.Texture2dFilePath;
            if (m_TexturePool.CanSpawn(texturePath))
            {
                var texture = (Texture2D)m_TexturePool.Spawn(texturePath).Target;
                SetTexture(setTexture2dObject, texture);
                return;
            }

            // 本地文件系统有，优先从本地加载
            if (!string.IsNullOrEmpty(saveFilePath) && HasFile(saveFilePath))
            {
                SetTextureByFileSystem(setTexture2dObject, saveFilePath);
                return;
            }

            if (!m_WaitSetObjects.TryGetValue(texturePath, out var awaitSets))
            {
                awaitSets = UGFHashSet<ISetTexture2dObject>.Create();
                m_WaitSetObjects.Add(texturePath, awaitSets);
            }
            awaitSets.Add(setTexture2dObject);

            if (!m_TextureBeingLoaded.Add(texturePath))
            {
                return;
            }

            m_WebRequestComponent.AddWebRequest(texturePath, WebGetTextureData.Create(setTexture2dObject, this, saveFilePath));
        }

        private void OnWebGetTextureFailure(object sender, GameEventArgs e)
        {
            WebRequestFailureEventArgs webRequestFailureEventArgs = (WebRequestFailureEventArgs)e;
            WebGetTextureData webGetTextureData = webRequestFailureEventArgs.UserData as WebGetTextureData;
            if (webGetTextureData == null || webGetTextureData.UserData != this)
            {
                return;
            }
            string texturePath = webGetTextureData.SetTexture2dObject.Texture2dFilePath;
            m_TextureBeingLoaded.Remove(texturePath);
            if (m_WaitSetObjects.Remove(texturePath, out UGFHashSet<ISetTexture2dObject> awaitSets))
            {
                foreach (var awaitSet in awaitSets)
                {
                    ReferencePool.Release(awaitSet);
                }
                awaitSets.Dispose();
            }
            Log.Error("Can not download Texture2D from '{0}' with error message '{1}'.", webRequestFailureEventArgs.WebRequestUri, webRequestFailureEventArgs.ErrorMessage);
            ReferencePool.Release(webGetTextureData);
        }

        private void OnWebGetTextureSuccess(object sender, GameEventArgs e)
        {
            WebRequestSuccessEventArgs webRequestSuccessEventArgs = (WebRequestSuccessEventArgs)e;
            WebGetTextureData webGetTextureData = webRequestSuccessEventArgs.UserData as WebGetTextureData;
            if (webGetTextureData == null || webGetTextureData.UserData != this)
            {
                return;
            }

            string texturePath = webGetTextureData.SetTexture2dObject.Texture2dFilePath;
            Texture2D tex = new Texture2D(0, 0, TextureFormat.RGBA32, false);
            var bytes = webRequestSuccessEventArgs.GetWebResponseBytes();
            tex.LoadImage(bytes);
            if (!string.IsNullOrEmpty(webGetTextureData.FilePath))
            {
                SaveTexture(webGetTextureData.FilePath, bytes);
            }

            var textureItemObject = TextureItemObject.Create(texturePath, tex, TextureLoad.FromNet);
            textureItemObject.Locked = true;
            m_TexturePool.Register(textureItemObject, false);
            m_TextureBeingLoaded.Remove(texturePath);

            if (m_WaitSetObjects.Remove(texturePath, out UGFHashSet<ISetTexture2dObject> awaitSets))
            {
                foreach (ISetTexture2dObject awaitSet in awaitSets)
                {
                    m_TexturePool.Spawn(texturePath);
                    SetTexture(awaitSet, tex);
                }
                awaitSets.Dispose();
            }

            textureItemObject.Locked = false;
            ReferencePool.Release(webGetTextureData);
        }
    }
}