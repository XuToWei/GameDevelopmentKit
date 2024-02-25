using System;
using System.Collections;
using System.Linq;
using System.Net.NetworkInformation;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace ThunderFireUITool
{
    // [InitializeOnLoad]
    public static class UXToolUsed
    {
        [Serializable]
        public class PostData
        {
            public string content = "";
        }

        public static void InitUXToolUsed()
        {
            Debug.Log("Thanks for using our tool!");
            Upload();
        }

        private static void Upload()
        {
            var postData = new PostData();
            var data = JsonUtility.ToJson(postData);


#if UNITY_2022_3_OR_NEWER
            UnityWebRequest www =
                UnityWebRequest.PostWwwForm("https://uxtool.netease.com/uxtool/api/collect", data);
#else
            UnityWebRequest www =           
                UnityWebRequest.Post("https://uxtool.netease.com/uxtool/api/collect", data);
#endif
            www.SendWebRequest();

            UXToolAnalysis.SendUXToolLog(UXToolAnalysisLog.UXToolInstall);
        }
    }
}