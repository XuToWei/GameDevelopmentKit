using System.Collections.Generic;
using GameFramework;
using GameFramework.Resource;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace UnityGameFramework.Extension
{
    public partial class TextureSetComponent
    {
        /// <summary>
        /// 资源组件
        /// </summary>
        private ResourceComponent m_ResourceComponent;

        private LoadAssetCallbacks m_LoadAssetCallbacks;

        private void InitializedResources()
        {
            m_ResourceComponent = GameEntry.GetComponent<ResourceComponent>();
            m_LoadAssetCallbacks = new LoadAssetCallbacks(OnLoadAssetSuccess, OnLoadAssetFailure);
        }

        private void OnLoadAssetFailure(string assetName, LoadResourceStatus status, string errormessage, object userdata)
        {
            ResourceData resourceData = (ResourceData) userdata;
            string texturePath = resourceData.SetTexture2dObject.Texture2dFilePath;
            m_TextureBeingLoaded.Remove(texturePath);
            if (m_WaitSetObjects.Remove(texturePath, out UGFHashSet<ISetTexture2dObject> awaitSets))
            {
                foreach (var awaitSet in awaitSets)
                {
                    ReferencePool.Release(awaitSet);
                }
                awaitSets.Dispose();
            }
            ReferencePool.Release(resourceData);
            Log.Error("Can not load Texture2D from '{0}' with error message '{1}'.", assetName, errormessage);
        }

        private void OnLoadAssetSuccess(string assetName, object asset, float duration, object userdata)
        {
            ResourceData resourceData = (ResourceData) userdata;
            Texture2D texture = asset as Texture2D;
            if (texture != null)
            {
                string texturePath = resourceData.SetTexture2dObject.Texture2dFilePath;
                m_TexturePool.Register(TextureItemObject.Create(texturePath, texture, TextureLoad.FromResource, m_ResourceComponent), false);
                m_TextureBeingLoaded.Remove(texturePath);
                if (m_WaitSetObjects.Remove(texturePath, out UGFHashSet<ISetTexture2dObject> awaitSets))
                {
                    foreach (ISetTexture2dObject awaitSet in awaitSets)
                    {
                        m_TexturePool.Spawn(texturePath);
                        SetTexture(awaitSet, texture);
                    }
                    awaitSets.Dispose();
                }
            }
            else
            {
                Log.Error("Load Texture2D failure asset type is '{0}'.", asset.GetType());
            }

            ReferencePool.Release(resourceData);
        }

        /// <summary>
        /// 通过资源系统设置图片
        /// </summary>
        /// <param name="setTexture2dObject">需要设置图片的对象</param>
        public void SetTextureByResources(ISetTexture2dObject setTexture2dObject)
        {
            string texturePath = setTexture2dObject.Texture2dFilePath;
            if (m_TexturePool.CanSpawn(texturePath))
            {
                var texture = (Texture2D)m_TexturePool.Spawn(texturePath).Target;
                SetTexture(setTexture2dObject, texture);
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

            m_ResourceComponent.LoadAsset(texturePath, typeof(Texture2D), m_LoadAssetCallbacks, ResourceData.Create(setTexture2dObject));
        }
    }
}