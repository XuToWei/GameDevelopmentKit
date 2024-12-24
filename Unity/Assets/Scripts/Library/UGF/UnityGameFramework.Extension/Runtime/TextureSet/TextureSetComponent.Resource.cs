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
            ReferencePool.Release(resourceData);
            Log.Error("Can not load Texture2D from '{1}' with error message '{2}'.", assetName, errormessage);
        }

        private void OnLoadAssetSuccess(string assetName, object asset, float duration, object userdata)
        {
            ResourceData resourceData = (ResourceData) userdata;
            Texture2D texture = asset as Texture2D;
            if (texture != null)
            {
                if (!m_TexturePool.CanSpawn(resourceData.SetTexture2dObject.Texture2dFilePath))
                {
                    m_TexturePool.Register(TextureItemObject.Create(resourceData.SetTexture2dObject.Texture2dFilePath, texture, TextureLoad.FromResource, m_ResourceComponent), true);
                }
                else
                {
                    m_TexturePool.Spawn(resourceData.SetTexture2dObject.Texture2dFilePath);
                }

                SetTexture(resourceData.SetTexture2dObject, texture, resourceData.SerialId);
            }
            else
            {
                Log.Error(new GameFrameworkException($"Load Texture2D failure asset type is {asset.GetType()}."));
            }

            ReferencePool.Release(resourceData);
        }

        /// <summary>
        /// 通过资源系统设置图片
        /// </summary>
        /// <param name="setTexture2dObject">需要设置图片的对象</param>
        public int SetTextureByResources(ISetTexture2dObject setTexture2dObject)
        {
            int serialId = -1;
            if (m_TexturePool.CanSpawn(setTexture2dObject.Texture2dFilePath))
            {
                var texture = (Texture2D) m_TexturePool.Spawn(setTexture2dObject.Texture2dFilePath).Target;
                SetTexture(setTexture2dObject, texture);
            }
            else
            {
                serialId = m_SerialId++;
                m_ResourceComponent.LoadAsset(setTexture2dObject.Texture2dFilePath, typeof(Texture2D), m_LoadAssetCallbacks, ResourceData.Create(setTexture2dObject, serialId));
            }

            return serialId;
        }
    }
}