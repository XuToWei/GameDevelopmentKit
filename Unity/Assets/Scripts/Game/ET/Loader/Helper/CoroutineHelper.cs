using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace ET
{
    public static class CoroutineHelper
    {
        // 有了这个方法，就可以直接await Unity的AsyncOperation了
        public static async UniTask GetAwaiter(this AsyncOperation asyncOperation)
        {
            var task = AutoResetUniTaskCompletionSource.Create();
            asyncOperation.completed += _ => { task.TrySetResult(); };
            await task.Task;
        }
        
        // public static async UniTask<string> HttpGet(string link)
        // {
        //     try
        //     {
        //         UnityWebRequest req = UnityWebRequest.Get(link);
        //         await req.SendWebRequest();
        //         return req.downloadHandler.text;
        //     }
        //     catch (Exception e)
        //     {
        //         throw new Exception($"http request fail: {link.Substring(0,link.IndexOf('?'))}\n{e}");
        //     }
        // }
    }
}