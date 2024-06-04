using Cysharp.Threading.Tasks;
using GameFramework.Resource;
using UnityGameFramework.Extension;
using UnityGameFramework.Runtime;
using Object = UnityEngine.Object;

public class ResourceManager
{
    private static ResourceComponent s_ResourceComponent;
    private static AssetCollection s_PreloadAsset;

    public static async UniTask InitAsync()
    {
        s_ResourceComponent = GameEntry.GetComponent<ResourceComponent>();
        s_PreloadAsset = await s_ResourceComponent.LoadAssetAsync<AssetCollection>(UXGUIConfig.UXToolAssetCollectionPath);
    }

    public static void Clear()
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
    public static T Load<T>(string path) where T : Object
    {
        if(path == null) return null;
#if UNITY_EDITOR
        if (s_PreloadAsset == null)
        {
            s_PreloadAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<AssetCollection>(UXGUIConfig.UXToolAssetCollectionPath);
        }
#endif
        return s_PreloadAsset.GetAsset<T>(path);
    }

    public static void AsyncLoad<T>(string path, System.Action<T> onLoadFinish) where T : Object
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

    public static void Unload(Object obj)
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