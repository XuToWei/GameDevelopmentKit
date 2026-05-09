using System;
using System.IO;
using Cysharp.Threading.Tasks;
using GameFramework;
using GameFramework.Resource;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Extension;
using UnityGameFramework.Runtime;
using Object = UnityEngine.Object;

internal class ResourceManager
{
    private static ResourceComponent s_ResourceComponent;
    private static AssetSetComponent s_AssetSetComponent;
    private static AssetCollection s_PreloadAsset;

    internal static async UniTask InitAsync()
    {
        s_ResourceComponent = GameEntry.GetComponent<ResourceComponent>();
        s_AssetSetComponent = GameEntry.GetComponent<AssetSetComponent>();
        s_PreloadAsset = await s_ResourceComponent.LoadAssetAsync<AssetCollection>(UXGUIConfig.UXToolAssetCollectionPath);
    }

    internal static void Clear()
    {
        AssetCollection preloadAsset = s_PreloadAsset;
        s_ResourceComponent = null;
        s_PreloadAsset = null;

        ResourceComponent resourceComponent = GameEntry.GetComponent<ResourceComponent>();
        if (resourceComponent != null && preloadAsset != null)
        {
            resourceComponent.UnloadAsset(preloadAsset);
        }
    }

    /// <summary>
    /// 资源加载接口,接入时需要统一替换成项目使用的资源加载实现方式
    /// </summary>
    /// <param name="path">资源路径，可传相对于Assets的路径，可带扩展名</param>
    /// <typeparam name="T">资源类型</typeparam>
    /// <returns>资源</returns>
    internal static T Load<T>(string path) where T : Object
    {
        if(string.IsNullOrEmpty(path)) return null;
#if UNITY_EDITOR
        if (s_PreloadAsset == null)
        {
            s_PreloadAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<AssetCollection>(UXGUIConfig.UXToolAssetCollectionPath);
        }
#endif
        return s_PreloadAsset.GetAsset<T>(path);
    }

    [Serializable]
    internal class UXImageLocalizationSet : AssetSet<Sprite>
    {
        [ShowInInspector]
        private UXImage m_UXImage;
        [ShowInInspector]
        private Sprite m_CurSprite;

        public static UXImageLocalizationSet Create(UXImage uxImage, string spritePath)
        {
            UXImageLocalizationSet localizationSet = ReferencePool.Acquire<UXImageLocalizationSet>();
            localizationSet.m_UXImage = uxImage;
            localizationSet.AssetPath = spritePath;
            localizationSet.Target = uxImage;
            return localizationSet;
        }

        public override void SetAsset(Sprite asset)
        {
            if (m_UXImage != null)
            {
                m_UXImage.sprite = asset;
                m_CurSprite = asset;
                if (asset == null)
                {
                    m_UXImage.sprite = Load<Sprite>(UXGUIConfig.UXGUINeedReplaceSpritePathReplace);
                }
            }
        }

        public override bool IsCanRelease()
        {
            return m_UXImage == null || m_UXImage.sprite != m_CurSprite && m_CurSprite != null;
        }

        public override void Clear()
        {
            base.Clear();
            m_UXImage = null;
            m_CurSprite = null;
        }
    }
    
    internal static void LoadSprite(UXImage uxImage, string spritePath)
    {
        if(string.IsNullOrEmpty(spritePath))
            return;
        if (s_AssetSetComponent != null)
        {
            UXImageLocalizationSet localizationSet = UXImageLocalizationSet.Create(uxImage, spritePath);
            s_AssetSetComponent.SetByResource(localizationSet);
        }
        else
        {
#if UNITY_EDITOR
            uxImage.sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
#endif
        }
    }

    internal static void AsyncLoad<T>(string path, System.Action<T> onLoadFinish) where T : Object
    {
#if UNITY_EDITOR
        if (s_ResourceComponent == null)
        {
            onLoadFinish?.Invoke(UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path));
            return;
        }
#endif
        s_ResourceComponent.LoadAsset(path, new LoadAssetCallbacks(
            (string _, object asset, float _, object _) =>
            {
                onLoadFinish?.Invoke((T)asset);
            }));
    }

    internal static void Unload(Object obj)
    {
#if UNITY_EDITOR
        if (s_ResourceComponent == null)
        {
            return;
        }
#endif
        s_ResourceComponent.UnloadAsset(obj);
    }
}