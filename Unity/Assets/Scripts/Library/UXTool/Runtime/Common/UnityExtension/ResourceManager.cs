using System.IO;
using UnityEditor;
using UnityEngine;

public class ResourceManager
{
    /// <summary>
    /// 资源加载接口,接入时需要统一替换成项目使用的资源加载实现方式
    /// </summary>
    /// <param name="path">资源路径，可传相对于Assets的路径，可带扩展名</param>
    /// <typeparam name="T">资源类型</typeparam>
    /// <returns>资源</returns>
    public static T Load<T>(string path) where T : Object
    {
        if(path == null) return null;
        return AssetDatabase.LoadAssetAtPath<T>(path);
    }

    public static void Load<T>(string path, System.Action<T> onLoadFinish) where T : Object
    {
        if (path == null) return;
        int index = path.IndexOf("/Resources/");
        T go = Resources.Load<T>(Path.ChangeExtension(index == -1 ? path : path.Substring(index + 11), null));
        onLoadFinish?.Invoke(go);
    }

    public static void AsyncLoad<T>(string path, System.Action<T> onLoadFinish) where T : Object
    {
        if (path == null) return;
        int index = path.IndexOf("/Resources/");
        T go = Resources.Load<T>(Path.ChangeExtension(index == -1 ? path : path.Substring(index + 11), null));
        onLoadFinish?.Invoke(go);
    }
}
