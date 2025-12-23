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
    private static SpriteCollectionComponent s_SpriteCollectionComponent;
    private static AssetCollection s_PreloadAsset;

    internal static async UniTask InitAsync()
    {
        s_ResourceComponent = GameEntry.GetComponent<ResourceComponent>();
        s_SpriteCollectionComponent = GameEntry.GetComponent<SpriteCollectionComponent>();
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
    internal class UXImageLocalizationWaitSet : ISetSpriteObject
    {
        [ShowInInspector]
        private Image m_Image;

        public static UXImageLocalizationWaitSet Create(Image obj, string collection, string spriteName)
        {
            UXImageLocalizationWaitSet waitSet = ReferencePool.Acquire<UXImageLocalizationWaitSet>();
            waitSet.m_Image = obj;
            waitSet.SpritePath = spriteName;
            waitSet.CollectionPath = collection;
            return waitSet;
        }

        [ShowInInspector]
        public string SpritePath { get; private set; }

        [ShowInInspector]
        public string CollectionPath { get; private set; }

        [ShowInInspector]
        public Sprite CurSprite { get; private set; }

        public void SetSprite(Sprite sprite)
        {
            if (m_Image != null)
            {
                m_Image.sprite = sprite;
                CurSprite = sprite;
                if (m_Image.sprite == null)
                {
                    m_Image.sprite = Load<Sprite>(UXGUIConfig.UXGUINeedReplaceSpritePathReplace);
                }
            }
        }

        public bool IsCanRelease()
        {
            return m_Image == null || m_Image.sprite != CurSprite && CurSprite != null;
        }

        public void Clear()
        {
            m_Image = null;
            SpritePath = null;
            CollectionPath = null;
            CurSprite = null;
        }
    }
    
    internal static void LoadSprite(Image image, string path)
    {
        if(string.IsNullOrEmpty(path)) return;
        if (s_SpriteCollectionComponent != null)
        {
            UXImageLocalizationWaitSet waitSet = UXImageLocalizationWaitSet.Create(image, Path.ChangeExtension(path, ".asset"), path);
            s_SpriteCollectionComponent.SetSprite(waitSet);
        }
        else
        {
#if UNITY_EDITOR
            image.sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(path);
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