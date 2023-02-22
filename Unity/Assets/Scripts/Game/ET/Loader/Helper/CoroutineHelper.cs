using System;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

namespace ET
{
    public static class CoroutineHelper
    {
        public static async UniTask<string> HttpGet(string link)
        {
            try
            {
                UnityWebRequest req = UnityWebRequest.Get(link);
                await req.SendWebRequest();
                return req.downloadHandler.text;
            }
            catch (Exception e)
            {
                throw new Exception($"http request fail: {link.Substring(0,link.IndexOf('?'))}\n{e}");
            }
        }
    }
}