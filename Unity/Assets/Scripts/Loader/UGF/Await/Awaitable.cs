using System;
using System.Collections.Generic;
using ET;
using GameFramework;
using GameFramework.Event;
using GameFramework.Resource;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace UGF
{
    public static partial class AwaitableExtensions
    {
        private static readonly Dictionary<int, ETTask<UIForm>> s_UIFormTcs = new();

        private static readonly Dictionary<int, ETTask<UnityGameFramework.Runtime.Entity>> s_EntityTcs = new();

        private static readonly Dictionary<string, ETTask<bool>> s_LoadSceneTcs = new();
        
        private static readonly Dictionary<string, ETTask<bool>> s_UnLoadSceneTcs = new();

        private static readonly HashSet<int> s_WebSerialIDs = new();
        private static readonly List<WebResult> s_DelayReleaseWebResult = new();

        private static readonly HashSet<int> s_DownloadSerialIds = new();
        private static readonly List<DownLoadResult> s_DelayReleaseDownloadResult = new();

#if UNITY_EDITOR
        private static bool s_IsSubscribeEvent = false;
#endif

        /// <summary>
        /// 注册需要的事件 (需再流程入口处调用 防止框架重启导致事件被取消问题)
        /// </summary>
        public static void SubscribeEvent()
        {
            EventComponent eventComponent = UnityGameFramework.Runtime.GameEntry.GetComponent<EventComponent>();
            eventComponent.Subscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);
            eventComponent.Subscribe(OpenUIFormFailureEventArgs.EventId, OnOpenUIFormFailure);

            eventComponent.Subscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
            eventComponent.Subscribe(ShowEntityFailureEventArgs.EventId, OnShowEntityFailure);

            eventComponent.Subscribe(LoadSceneSuccessEventArgs.EventId, OnLoadSceneSuccess);
            eventComponent.Subscribe(LoadSceneFailureEventArgs.EventId, OnLoadSceneFailure);
            eventComponent.Subscribe(UnloadSceneSuccessEventArgs.EventId, OnUnloadSceneSuccess);
            eventComponent.Subscribe(UnloadSceneFailureEventArgs.EventId, OnUnloadSceneFailure);

            eventComponent.Subscribe(WebRequestSuccessEventArgs.EventId, OnWebRequestSuccess);
            eventComponent.Subscribe(WebRequestFailureEventArgs.EventId, OnWebRequestFailure);

            eventComponent.Subscribe(DownloadSuccessEventArgs.EventId, OnDownloadSuccess);
            eventComponent.Subscribe(DownloadFailureEventArgs.EventId, OnDownloadFailure);
#if UNITY_EDITOR
            s_IsSubscribeEvent = true;
#endif
        }

#if UNITY_EDITOR
        private static void TipsSubscribeEvent()
        {
            if (!s_IsSubscribeEvent)
            {
                throw new Exception("Use await/async extensions must to subscribe event!");
            }
        }
#endif

        /// <summary>
        /// 打开界面（可等待）
        /// </summary>
        public static ETTask<UIForm> OpenUIFormAsync(this UIComponent uiComponent,string uiFormAssetName, string uiGroupName, int priority, bool pauseCoveredUIForm, object userData)
        {
#if UNITY_EDITOR
            TipsSubscribeEvent();
#endif
            int serialId = uiComponent.OpenUIForm(uiFormAssetName, uiGroupName, priority, pauseCoveredUIForm, userData);
            var tcs = ETTask<UIForm>.Create(true);
            s_UIFormTcs.Add(serialId, tcs);
            return tcs;
        }

        private static void OnOpenUIFormSuccess(object sender, GameEventArgs e)
        {
            OpenUIFormSuccessEventArgs ne = (OpenUIFormSuccessEventArgs)e;
            s_UIFormTcs.TryGetValue(ne.UIForm.SerialId, out ETTask<UIForm> tcs);
            if (tcs != null)
            {
                tcs.SetResult(ne.UIForm);
                s_UIFormTcs.Remove(ne.UIForm.SerialId);
            }
        }

        private static void OnOpenUIFormFailure(object sender, GameEventArgs e)
        {
            OpenUIFormFailureEventArgs ne = (OpenUIFormFailureEventArgs)e;
            s_UIFormTcs.TryGetValue(ne.SerialId, out ETTask<UIForm> tcs);
            if (tcs != null)
            {
                Debug.LogError(ne.ErrorMessage);
                tcs.SetException(new GameFrameworkException(ne.ErrorMessage));
                s_UIFormTcs.Remove(ne.SerialId);
            }
        }

        /// <summary>
        /// 显示实体（可等待）
        /// </summary>
        public static ETTask<UnityGameFramework.Runtime.Entity> ShowEntityAsync(this EntityComponent entityComponent, int entityId,
            Type entityLogicType, string entityAssetName, string entityGroupName, int priority,object userData)
        {
#if UNITY_EDITOR
            TipsSubscribeEvent();
#endif
            var tcs = ETTask<UnityGameFramework.Runtime.Entity>.Create(true);
            s_EntityTcs.Add(entityId, tcs);
            entityComponent.ShowEntity(entityId, entityLogicType, entityAssetName, entityGroupName, priority, userData);
            return tcs;
        }


        private static void OnShowEntitySuccess(object sender, GameEventArgs e)
        {
            ShowEntitySuccessEventArgs ne = (ShowEntitySuccessEventArgs)e;
            s_EntityTcs.TryGetValue(ne.Entity.Id, out var tcs);
            if (tcs != null)
            {
                tcs.SetResult(ne.Entity);
                s_EntityTcs.Remove(ne.Entity.Id);
            }
        }

        private static void OnShowEntityFailure(object sender, GameEventArgs e)
        {
            ShowEntityFailureEventArgs ne = (ShowEntityFailureEventArgs)e;
            s_EntityTcs.TryGetValue(ne.EntityId, out var tcs);
            if (tcs != null)
            {
                Debug.LogError(ne.ErrorMessage);
                tcs.SetException(new GameFrameworkException(ne.ErrorMessage));
                s_EntityTcs.Remove(ne.EntityId);
            }
        }


        /// <summary>
        /// 加载场景（可等待）
        /// </summary>
        public static async ETTask<bool> LoadSceneAsync(this SceneComponent sceneComponent, string sceneAssetName)
        {
#if UNITY_EDITOR
            TipsSubscribeEvent();
#endif
            var tcs = ETTask<bool>.Create(true);
            var isUnLoadScene = s_UnLoadSceneTcs.TryGetValue(sceneAssetName, out var unloadSceneTcs);
            if (isUnLoadScene)
            {
                await unloadSceneTcs;
            }
            s_LoadSceneTcs.Add(sceneAssetName, tcs);

            try
            {
                sceneComponent.LoadScene(sceneAssetName);
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
                tcs.SetException(e);
                s_LoadSceneTcs.Remove(sceneAssetName);
            }
            return await tcs;
        }

        private static void OnLoadSceneSuccess(object sender, GameEventArgs e)
        {
            LoadSceneSuccessEventArgs ne = (LoadSceneSuccessEventArgs)e;
            s_LoadSceneTcs.TryGetValue(ne.SceneAssetName, out var tcs);
            if (tcs != null)
            {
                tcs.SetResult(true);
                s_LoadSceneTcs.Remove(ne.SceneAssetName);
            }
        }

        private static void OnLoadSceneFailure(object sender, GameEventArgs e)
        {
            LoadSceneFailureEventArgs ne = (LoadSceneFailureEventArgs)e;
            s_LoadSceneTcs.TryGetValue(ne.SceneAssetName, out var tcs);
            if (tcs != null)
            {
                Debug.LogError(ne.ErrorMessage);
                tcs.SetException(new GameFrameworkException(ne.ErrorMessage));
                s_LoadSceneTcs.Remove(ne.SceneAssetName);
            }
        }
        
        /// <summary>
        /// 卸载场景（可等待）
        /// </summary>
        public static async ETTask<bool> UnLoadSceneAsync(this SceneComponent sceneComponent, string sceneAssetName)
        {
#if UNITY_EDITOR
            TipsSubscribeEvent();
#endif
            var tcs = ETTask<bool>.Create(true);
            var isLoadSceneTcs = s_LoadSceneTcs.TryGetValue(sceneAssetName, out var loadSceneTcs);
            if (isLoadSceneTcs)
            {
                Debug.Log("Unload  loading scene");
                await loadSceneTcs;
            }
            s_UnLoadSceneTcs.Add(sceneAssetName, tcs);
            try
            {
                sceneComponent.UnloadScene(sceneAssetName);
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
                tcs.SetException(e);
                s_UnLoadSceneTcs.Remove(sceneAssetName);
            }
            return await tcs;
        }
        private static void OnUnloadSceneSuccess(object sender, GameEventArgs e)
        {
            UnloadSceneSuccessEventArgs ne = (UnloadSceneSuccessEventArgs)e;
            s_UnLoadSceneTcs.TryGetValue(ne.SceneAssetName, out var tcs);
            if (tcs != null)
            {
                tcs.SetResult(true);
                s_UnLoadSceneTcs.Remove(ne.SceneAssetName);
            }
        }

        private static void OnUnloadSceneFailure(object sender, GameEventArgs e)
        {
            UnloadSceneFailureEventArgs ne = (UnloadSceneFailureEventArgs)e;
            s_UnLoadSceneTcs.TryGetValue(ne.SceneAssetName, out var tcs);
            if (tcs != null)
            {
                Debug.LogError($"Unload scene {ne.SceneAssetName} failure.");
                tcs.SetException(new GameFrameworkException($"Unload scene {ne.SceneAssetName} failure."));
                s_UnLoadSceneTcs.Remove(ne.SceneAssetName);
            }
        }

        /// <summary>
        /// 加载资源（可等待）
        /// </summary>
        public static ETTask<T> LoadAssetAsync<T>(this ResourceComponent resourceComponent, string assetName)
            where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            TipsSubscribeEvent();
#endif
            ETTask<T> loadAssetTcs = ETTask<T>.Create(true);
            resourceComponent.LoadAsset(assetName, typeof(T), new LoadAssetCallbacks(
                (tempAssetName, asset, duration, userdata) =>
                {
                    var source = loadAssetTcs;
                    loadAssetTcs = null;
                    T tAsset = asset as T;
                    if (tAsset != null)
                    {
                        source.SetResult(tAsset);
                    }
                    else
                    {
                        Debug.LogError($"Load asset failure load type is {asset.GetType()} but asset type is {typeof(T)}.");
                        source.SetException(new GameFrameworkException(
                            $"Load asset failure load type is {asset.GetType()} but asset type is {typeof(T)}."));
                    }
                },
                (tempAssetName, status, errorMessage, userdata) =>
                {
                    Debug.LogError(errorMessage);
                    loadAssetTcs.SetException(new GameFrameworkException(errorMessage));
                }
            ));

            return loadAssetTcs;
        }

        /// <summary>
        /// 加载多个资源（可等待）
        /// </summary>
        public static async ETTask<T[]> LoadAssetsAsync<T>(this ResourceComponent resourceComponent, string[] assetName) where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            TipsSubscribeEvent();
#endif
            if (assetName == null)
            {
                return null;
            }
            T[] assets = new T[assetName.Length];
            
            ETTask<T>[] tasks = new ETTask<T>[assets.Length];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = resourceComponent.LoadAssetAsync<T>(assetName[i]);
            }

            await ETTaskHelper.WaitAll(tasks);
            for (int i = 0; i < assets.Length; i++)
            {
                assets[i] = tasks[i].GetResult();
            }

            return assets;
        }


        /// <summary>
        /// 增加Web请求任务（可等待）
        /// </summary>
        public static ETTask<WebResult> AddWebRequestAsync(this WebRequestComponent webRequestComponent,
            string webRequestUri, WWWForm wwwForm = null, object userdata = null)
        {
#if UNITY_EDITOR
            TipsSubscribeEvent();
#endif
            var tsc = ETTask<WebResult>.Create(true);
            int serialId = webRequestComponent.AddWebRequest(webRequestUri, wwwForm,
                AwaitDataWrap<WebResult>.Create(userdata, tsc));
            s_WebSerialIDs.Add(serialId);
            return tsc;
        }

        /// <summary>
        /// 增加Web请求任务（可等待）
        /// </summary>
        public static ETTask<WebResult> AddWebRequestAsync(this WebRequestComponent webRequestComponent,
            string webRequestUri, byte[] postData, object userdata = null)
        {
#if UNITY_EDITOR
            TipsSubscribeEvent();
#endif
            var tsc = ETTask<WebResult>.Create(true);
            int serialId = webRequestComponent.AddWebRequest(webRequestUri, postData,
                AwaitDataWrap<WebResult>.Create(userdata, tsc));
            s_WebSerialIDs.Add(serialId);
            return tsc;
        }

        private static void OnWebRequestSuccess(object sender, GameEventArgs e)
        {
            WebRequestSuccessEventArgs ne = (WebRequestSuccessEventArgs)e;
            if (s_WebSerialIDs.Contains(ne.SerialId))
            {
                if (ne.UserData is AwaitDataWrap<WebResult> webRequestUserdata)
                {
                    WebResult result = WebResult.Create(ne.GetWebResponseBytes(), false, string.Empty,
                        webRequestUserdata.UserData);
                    s_DelayReleaseWebResult.Add(result);
                    webRequestUserdata.Source.SetResult(result);
                    ReferencePool.Release(webRequestUserdata);
                }

                s_WebSerialIDs.Remove(ne.SerialId);
                if (s_WebSerialIDs.Count == 0)
                {
                    for (int i = 0; i < s_DelayReleaseWebResult.Count; i++)
                    {
                        ReferencePool.Release(s_DelayReleaseWebResult[i]);
                    }

                    s_DelayReleaseWebResult.Clear();
                }
            }
        }

        private static void OnWebRequestFailure(object sender, GameEventArgs e)
        {
            WebRequestFailureEventArgs ne = (WebRequestFailureEventArgs)e;
            if (s_WebSerialIDs.Contains(ne.SerialId))
            {
                if (ne.UserData is AwaitDataWrap<WebResult> webRequestUserdata)
                {
                    WebResult result = WebResult.Create(null, true, ne.ErrorMessage, webRequestUserdata.UserData);
                    webRequestUserdata.Source.SetResult(result);
                    s_DelayReleaseWebResult.Add(result);
                    ReferencePool.Release(webRequestUserdata);
                }

                s_WebSerialIDs.Remove(ne.SerialId);
                if (s_WebSerialIDs.Count == 0)
                {
                    for (int i = 0; i < s_DelayReleaseWebResult.Count; i++)
                    {
                        ReferencePool.Release(s_DelayReleaseWebResult[i]);
                    }

                    s_DelayReleaseWebResult.Clear();
                }
            }
        }

        /// <summary>
        /// 增加下载任务（可等待)
        /// </summary>
        public static ETTask<DownLoadResult> AddDownloadAsync(this DownloadComponent downloadComponent,
            string downloadPath,
            string downloadUri,
            object userdata = null)
        {
#if UNITY_EDITOR
            TipsSubscribeEvent();
#endif
            var tcs = ETTask<DownLoadResult>.Create(true);
            int serialId = downloadComponent.AddDownload(downloadPath, downloadUri,
                AwaitDataWrap<DownLoadResult>.Create(userdata, tcs));
            s_DownloadSerialIds.Add(serialId);
            return tcs;
        }

        private static void OnDownloadSuccess(object sender, GameEventArgs e)
        {
            DownloadSuccessEventArgs ne = (DownloadSuccessEventArgs)e;
            if (s_DownloadSerialIds.Contains(ne.SerialId))
            {
                if (ne.UserData is AwaitDataWrap<DownLoadResult> awaitDataWrap)
                {
                    DownLoadResult result = DownLoadResult.Create(false, string.Empty, awaitDataWrap.UserData);
                    s_DelayReleaseDownloadResult.Add(result);
                    awaitDataWrap.Source.SetResult(result);
                    ReferencePool.Release(awaitDataWrap);
                }

                s_DownloadSerialIds.Remove(ne.SerialId);
                if (s_DownloadSerialIds.Count == 0)
                {
                    for (int i = 0; i < s_DelayReleaseDownloadResult.Count; i++)
                    {
                        ReferencePool.Release(s_DelayReleaseDownloadResult[i]);
                    }

                    s_DelayReleaseDownloadResult.Clear();
                }
            }
        }

        private static void OnDownloadFailure(object sender, GameEventArgs e)
        {
            DownloadFailureEventArgs ne = (DownloadFailureEventArgs)e;
            if (s_DownloadSerialIds.Contains(ne.SerialId))
            {
                if (ne.UserData is AwaitDataWrap<DownLoadResult> awaitDataWrap)
                {
                    DownLoadResult result = DownLoadResult.Create(true, ne.ErrorMessage, awaitDataWrap.UserData);
                    s_DelayReleaseDownloadResult.Add(result);
                    awaitDataWrap.Source.SetResult(result);
                    ReferencePool.Release(awaitDataWrap);
                }

                s_DownloadSerialIds.Remove(ne.SerialId);
                if (s_DownloadSerialIds.Count == 0)
                {
                    for (int i = 0; i < s_DelayReleaseDownloadResult.Count; i++)
                    {
                        ReferencePool.Release(s_DelayReleaseDownloadResult[i]);
                    }

                    s_DelayReleaseDownloadResult.Clear();
                }
            }
        }
    }
}