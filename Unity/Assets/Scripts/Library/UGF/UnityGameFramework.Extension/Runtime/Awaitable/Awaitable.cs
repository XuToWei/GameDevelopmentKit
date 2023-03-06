using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameFramework;
using GameFramework.Event;
using GameFramework.Resource;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace UnityGameFramework.Extension
{
    public static class Awaitable
    {
#if UNITY_EDITOR
        private static bool s_IsSubscribeEvent = false;
#endif
        private static readonly Dictionary<int, AwaitTaskWrap<UIForm>> s_OpenUFormTcsDict = new Dictionary<int, AwaitTaskWrap<UIForm>>();
        private static readonly Dictionary<int, AwaitTaskWrap<Entity>> s_ShowEntityTcsDict = new Dictionary<int, AwaitTaskWrap<Entity>>();
        private static readonly Dictionary<string, AutoResetUniTaskCompletionSource> s_LoadSceneTcsDict = new Dictionary<string, AutoResetUniTaskCompletionSource>();
        private static readonly Dictionary<string, AutoResetUniTaskCompletionSource> s_UnLoadSceneTcsDict = new Dictionary<string, AutoResetUniTaskCompletionSource>();

        /// <summary>
        /// 注册需要的事件 (需再流程入口处调用 防止框架重启导致事件被取消问题)
        /// </summary>
        public static void SubscribeEvent()
        {
            if (s_OpenUFormTcsDict.Count > 0 || s_ShowEntityTcsDict.Count > 0 ||
                s_LoadSceneTcsDict.Count > 0 || s_UnLoadSceneTcsDict.Count > 0)
            {
                throw new GameFrameworkException("Awaitable Task is not clean!");
            }
            
            EventComponent eventComponent = GameEntry.GetComponent<EventComponent>();
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
        public static UniTask<UIForm> OpenUIFormAsync(this UIComponent uiComponent, string uiFormAssetName, string uiGroupName,
        int priority, bool pauseCoveredUIForm, object userData = null, CancellationTokenSource cts = null)
        {
#if UNITY_EDITOR
            TipsSubscribeEvent();
#endif
            int serialId = uiComponent.OpenUIForm(uiFormAssetName, uiGroupName, priority, pauseCoveredUIForm, userData);
            var tcs = AutoResetUniTaskCompletionSource<UIForm>.Create();
            CancellationTokenRegistration? ctr = cts?.Token.Register(() =>
            {
                if (uiComponent.IsLoadingUIForm(serialId))
                {
                    if (s_OpenUFormTcsDict.ContainsKey(serialId))
                    {
                        s_OpenUFormTcsDict.Remove(serialId);
                    }
                    uiComponent.CloseUIForm(serialId);
                    tcs.TrySetResult(null);
                }
            });
            AwaitTaskWrap<UIForm> awaitTaskWrap = AwaitTaskWrap<UIForm>.Create(tcs, ctr);
            s_OpenUFormTcsDict.Add(serialId, awaitTaskWrap);
            return tcs.Task;
        }

        private static void OnOpenUIFormSuccess(object sender, GameEventArgs e)
        {
            OpenUIFormSuccessEventArgs ne = (OpenUIFormSuccessEventArgs)e;
            int serialId = ne.UIForm.SerialId;
            if (s_OpenUFormTcsDict.TryGetValue(serialId, out AwaitTaskWrap<UIForm> awaitTaskWrap))
            {
                s_OpenUFormTcsDict.Remove(serialId);
                awaitTaskWrap.CancellationTokenRegistration?.Dispose();
                var taskCompletionSource = awaitTaskWrap.TaskCompletionSource;
                ReferencePool.Release(awaitTaskWrap);
                taskCompletionSource.TrySetResult(ne.UIForm);
            }
        }

        private static void OnOpenUIFormFailure(object sender, GameEventArgs e)
        {
            OpenUIFormFailureEventArgs ne = (OpenUIFormFailureEventArgs)e;
            int serialId = ne.SerialId;
            if (s_OpenUFormTcsDict.TryGetValue(serialId, out AwaitTaskWrap<UIForm> awaitTaskWrap))
            {
                s_OpenUFormTcsDict.Remove(serialId);
                Log.Error(ne.ErrorMessage);
                awaitTaskWrap.CancellationTokenRegistration?.Dispose();
                var taskCompletionSource = awaitTaskWrap.TaskCompletionSource;
                ReferencePool.Release(awaitTaskWrap);
                taskCompletionSource.TrySetException(new GameFrameworkException(ne.ErrorMessage));
            }
        }

        /// <summary>
        /// 显示实体（可等待）
        /// </summary>
        public static UniTask<Entity> ShowEntityAsync(this EntityComponent entityComponent, int entityId, Type entityLogicType, string entityAssetName,
        string entityGroupName, int priority = 0, object userData = null, CancellationTokenSource cts = null)
        {
#if UNITY_EDITOR
            TipsSubscribeEvent();
#endif
            entityComponent.ShowEntity(entityId, entityLogicType, entityAssetName, entityGroupName, priority, userData);
            var tcs = AutoResetUniTaskCompletionSource<Entity>.Create();
            CancellationTokenRegistration? ctr = cts?.Token.Register(() =>
            {
                if (entityComponent.IsLoadingEntity(entityId))
                {
                    if (s_ShowEntityTcsDict.ContainsKey(entityId))
                    {
                        s_ShowEntityTcsDict.Remove(entityId);
                    }
                    entityComponent.HideEntity(entityId);
                    tcs.TrySetResult(null);
                }
            });
            AwaitTaskWrap<Entity> awaitTaskWrap = AwaitTaskWrap<Entity>.Create(tcs, ctr);
            s_ShowEntityTcsDict.Add(entityId, awaitTaskWrap);
            return tcs.Task;
        }

        private static void OnShowEntitySuccess(object sender, GameEventArgs e)
        {
            ShowEntitySuccessEventArgs ne = (ShowEntitySuccessEventArgs)e;
            int entityId = ne.Entity.Id;
            if (s_ShowEntityTcsDict.TryGetValue(entityId, out AwaitTaskWrap<Entity> awaitTaskWrap))
            {
                s_ShowEntityTcsDict.Remove(entityId);
                awaitTaskWrap.CancellationTokenRegistration?.Dispose();
                var taskCompletionSource = awaitTaskWrap.TaskCompletionSource;
                ReferencePool.Release(awaitTaskWrap);
                taskCompletionSource.TrySetResult(ne.Entity);
            }
        }

        private static void OnShowEntityFailure(object sender, GameEventArgs e)
        {
            ShowEntityFailureEventArgs ne = (ShowEntityFailureEventArgs)e;
            int entityId = ne.EntityId;
            if (s_ShowEntityTcsDict.TryGetValue(entityId, out AwaitTaskWrap<Entity> awaitTaskWrap))
            {
                s_ShowEntityTcsDict.Remove(entityId);
                Log.Error(ne.ErrorMessage);
                awaitTaskWrap.CancellationTokenRegistration?.Dispose();
                var taskCompletionSource = awaitTaskWrap.TaskCompletionSource;
                ReferencePool.Release(awaitTaskWrap);
                taskCompletionSource.TrySetException(new GameFrameworkException(ne.ErrorMessage));
            }
        }

        /// <summary>
        /// 加载场景（可等待）
        /// </summary>
        public static UniTask LoadSceneAsync(this SceneComponent sceneComponent, string sceneAssetName, int priority = 0 , object userData = null)
        {
#if UNITY_EDITOR
            TipsSubscribeEvent();
#endif
            var tcs = AutoResetUniTaskCompletionSource.Create();
            try
            {
                sceneComponent.LoadScene(sceneAssetName, priority, userData);
                s_LoadSceneTcsDict.Add(sceneAssetName, tcs);
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                tcs.TrySetException(e);
            }
            return tcs.Task;
        }

        private static void OnLoadSceneSuccess(object sender, GameEventArgs e)
        {
            LoadSceneSuccessEventArgs ne = (LoadSceneSuccessEventArgs)e;
            string sceneAssetName = ne.SceneAssetName;
            if (s_LoadSceneTcsDict.TryGetValue(sceneAssetName, out AutoResetUniTaskCompletionSource taskCompletionSource))
            {
                s_LoadSceneTcsDict.Remove(sceneAssetName);
                taskCompletionSource.TrySetResult();
            }
        }

        private static void OnLoadSceneFailure(object sender, GameEventArgs e)
        {
            LoadSceneFailureEventArgs ne = (LoadSceneFailureEventArgs)e;
            string sceneAssetName = ne.SceneAssetName;
            if (s_LoadSceneTcsDict.TryGetValue(sceneAssetName, out AutoResetUniTaskCompletionSource taskCompletionSource))
            {
                s_LoadSceneTcsDict.Remove(sceneAssetName);
                Log.Error(ne.ErrorMessage);
                taskCompletionSource.TrySetException(new GameFrameworkException(ne.ErrorMessage));
            }
        }

        /// <summary>
        /// 卸载场景（可等待）
        /// </summary>
        public static UniTask UnLoadSceneAsync(this SceneComponent sceneComponent, string sceneAssetName, object userData = null)
        {
#if UNITY_EDITOR
            TipsSubscribeEvent();
#endif
            var tcs = AutoResetUniTaskCompletionSource.Create();
            try
            {
                sceneComponent.UnloadScene(sceneAssetName, AwaitDataWrap.Create(userData, tcs));
                s_UnLoadSceneTcsDict.Add(sceneAssetName, tcs);
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                tcs.TrySetException(e);
            }
            return tcs.Task;
        }

        private static void OnUnloadSceneSuccess(object sender, GameEventArgs e)
        {
            UnloadSceneSuccessEventArgs ne = (UnloadSceneSuccessEventArgs)e;
            string sceneAssetName = ne.SceneAssetName;
            if (s_UnLoadSceneTcsDict.TryGetValue(sceneAssetName, out AutoResetUniTaskCompletionSource taskCompletionSource))
            {
                s_UnLoadSceneTcsDict.Remove(sceneAssetName);
                taskCompletionSource.TrySetResult();
            }
        }

        private static void OnUnloadSceneFailure(object sender, GameEventArgs e)
        {
            UnloadSceneFailureEventArgs ne = (UnloadSceneFailureEventArgs)e;
            string sceneAssetName = ne.SceneAssetName;
            if (s_UnLoadSceneTcsDict.TryGetValue(sceneAssetName, out AutoResetUniTaskCompletionSource taskCompletionSource))
            {
                s_UnLoadSceneTcsDict.Remove(sceneAssetName);
                Log.Error($"Unload scene {ne.SceneAssetName} failure.");
                taskCompletionSource.TrySetException(new GameFrameworkException($"Unload scene {ne.SceneAssetName} failure."));
            }
        }

        /// <summary>
        /// 加载资源（可等待）
        /// </summary>
        public static UniTask<T> LoadAssetAsync<T>(this ResourceComponent resourceComponent, string assetName,
        int priority = 0, object userData = null, CancellationTokenSource cts = null) where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            TipsSubscribeEvent();
#endif
            var tcs = AutoResetUniTaskCompletionSource<T>.Create();
            CancellationTokenRegistration? ctr = cts?.Token.Register(() =>
            {
                tcs.TrySetResult(null);
            });
            resourceComponent.LoadAsset(assetName, typeof (T), priority, new LoadAssetCallbacks(
                    (tempAssetName, asset, duration, userdata) =>
                    {
                        T tAsset = asset as T;
                        if (tAsset != null)
                        {
                            if (cts is { IsCancellationRequested: true })
                            {
                                resourceComponent.UnloadAsset(tAsset);
                            }
                            else
                            {
                                ctr?.Dispose();
                                tcs.TrySetResult(tAsset);
                            }
                        }
                        else
                        {
                            Log.Error($"Load asset failure load type is {asset.GetType()} but asset type is {typeof (T)}.");
                            tcs.TrySetException(new GameFrameworkException($"Load asset failure load type is {asset.GetType()} but asset type is {typeof (T)}."));
                        }
                    },
                    (tempAssetName, status, errorMessage, userdata) =>
                    {
                        Log.Error(errorMessage);
                        tcs.TrySetException(new GameFrameworkException(errorMessage));
                    }),
                userData);
            
            return tcs.Task;
        }

        /// <summary>
        /// 增加Web请求任务（可等待）
        /// </summary>
        public static UniTask<WebResult> WebRequestAsync(this WebRequestComponent webRequestComponent, string webRequestUri,
        WWWForm wwwForm = null, string tag = null, int priority = 0, object userData = null, CancellationTokenSource cts = null)
        {
#if UNITY_EDITOR
            TipsSubscribeEvent();
#endif
            var tcs = AutoResetUniTaskCompletionSource<WebResult>.Create();
            int serialId = 0;
            CancellationTokenRegistration? ctr = cts?.Token.Register(() =>
            {
                var taskInfo = webRequestComponent.GetWebRequestInfo(serialId);
                if (taskInfo.Status != TaskStatus.Done)
                {
                    webRequestComponent.RemoveWebRequest(serialId);
                    tcs.TrySetResult(null);
                }
            });
            serialId = webRequestComponent.AddWebRequest(webRequestUri, wwwForm, tag, priority, AwaitDataWrap<WebResult>.Create(userData, tcs, ctr));
            return tcs.Task;
        }

        /// <summary>
        /// 增加Web请求任务（可等待）
        /// </summary>
        public static UniTask<WebResult> WebRequestAsync(this WebRequestComponent webRequestComponent, string webRequestUri,
        byte[] postData, string tag = null, int priority = 0, object userData = null, CancellationTokenSource cts = null)
        {
#if UNITY_EDITOR
            TipsSubscribeEvent();
#endif
            var tcs = AutoResetUniTaskCompletionSource<WebResult>.Create();
            int serialId = 0;
            CancellationTokenRegistration? ctr = cts?.Token.Register(() =>
            {
                var taskInfo = webRequestComponent.GetWebRequestInfo(serialId);
                if (taskInfo.Status != TaskStatus.Done)
                {
                    webRequestComponent.RemoveWebRequest(serialId);
                    tcs.TrySetResult(null);
                }
            });
            serialId = webRequestComponent.AddWebRequest(webRequestUri, postData, tag, priority, AwaitDataWrap<WebResult>.Create(userData, tcs, ctr));
            return tcs.Task;
        }

        private static void OnWebRequestSuccess(object sender, GameEventArgs e)
        {
            WebRequestSuccessEventArgs ne = (WebRequestSuccessEventArgs)e;
            if (ne.UserData is AwaitDataWrap<WebResult> awaitDataWrap)
            {
                awaitDataWrap.CancellationTokenRegistration?.Dispose();
                var taskCompletionSource = awaitDataWrap.TaskCompletionSource;
                WebResult result = WebResult.Create(ne.GetWebResponseBytes(), false, string.Empty, awaitDataWrap.UserData);
                ReferencePool.Release(awaitDataWrap);
                taskCompletionSource.TrySetResult(result);
            }
        }

        private static void OnWebRequestFailure(object sender, GameEventArgs e)
        {
            WebRequestFailureEventArgs ne = (WebRequestFailureEventArgs)e;
            if (ne.UserData is AwaitDataWrap<WebResult> awaitDataWrap)
            {
                awaitDataWrap.CancellationTokenRegistration?.Dispose();
                var taskCompletionSource = awaitDataWrap.TaskCompletionSource;
                WebResult result = WebResult.Create(null, true, ne.ErrorMessage, awaitDataWrap.UserData);
                ReferencePool.Release(awaitDataWrap);
                taskCompletionSource.TrySetResult(result);
            }
        }

        /// <summary>
        /// 增加下载任务（可等待)
        /// </summary>
        public static UniTask<DownloadResult> DownloadAsync(this DownloadComponent downloadComponent, string downloadPath,
        string downloadUri, string tag = null, int priority = 0, object userdata = null, CancellationTokenSource cts = null)
        {
#if UNITY_EDITOR
            TipsSubscribeEvent();
#endif
            var tcs = AutoResetUniTaskCompletionSource<DownloadResult>.Create();
            int serialId = 0;
            CancellationTokenRegistration? ctr = cts?.Token.Register(() =>
            {
                var taskInfo = downloadComponent.GetDownloadInfo(serialId);
                if (taskInfo.Status != TaskStatus.Done)
                {
                    downloadComponent.RemoveDownload(serialId);
                    tcs.TrySetResult(null);
                }
            });
            serialId = downloadComponent.AddDownload(downloadPath, downloadUri, tag, priority, AwaitDataWrap<DownloadResult>.Create(userdata, tcs, ctr));
            return tcs.Task;
        }

        private static void OnDownloadSuccess(object sender, GameEventArgs e)
        {
            DownloadSuccessEventArgs ne = (DownloadSuccessEventArgs)e;
            if (ne.UserData is AwaitDataWrap<DownloadResult> awaitDataWrap)
            {
                awaitDataWrap.CancellationTokenRegistration?.Dispose();
                DownloadResult result = DownloadResult.Create(false, string.Empty, awaitDataWrap.UserData);
                var tcs = awaitDataWrap.TaskCompletionSource;
                ReferencePool.Release(awaitDataWrap);
                tcs.TrySetResult(result);
            }
        }

        private static void OnDownloadFailure(object sender, GameEventArgs e)
        {
            DownloadFailureEventArgs ne = (DownloadFailureEventArgs)e;
            if (ne.UserData is AwaitDataWrap<DownloadResult> awaitDataWrap)
            {
                awaitDataWrap.CancellationTokenRegistration?.Dispose();
                DownloadResult result = DownloadResult.Create(true, ne.ErrorMessage, awaitDataWrap.UserData);
                var tcs = awaitDataWrap.TaskCompletionSource;
                ReferencePool.Release(awaitDataWrap);
                tcs.TrySetResult(result);
            }
        }
    }
}