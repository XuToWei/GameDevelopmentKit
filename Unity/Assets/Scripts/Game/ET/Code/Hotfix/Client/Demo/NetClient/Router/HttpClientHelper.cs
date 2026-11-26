using System;
#if UNITY_WEBGL && UNITY_ET && !UNITY_EDITOR
using UnityEngine.Networking;
#else
using System.Net.Http;
#endif
using Cysharp.Threading.Tasks;

namespace ET.Client
{
    public static partial class HttpClientHelper
    {
        public static async UniTask<string> Get(string link)
        {
            try
            {
#if UNITY_WEBGL && UNITY_ET && !UNITY_EDITOR
                using UnityWebRequest request = UnityWebRequest.Get(link);
                await request.SendWebRequest();
                return request.downloadHandler.text;
#else
                using HttpClient httpClient = new();
                HttpResponseMessage response =  await httpClient.GetAsync(link);
                string result = await response.Content.ReadAsStringAsync();
                return result;
#endif
            }
            catch (Exception e)
            {
                throw new Exception($"http request fail: {link.Substring(0,link.IndexOf('?'))}\n{e}");
            }
        }
    }
}
