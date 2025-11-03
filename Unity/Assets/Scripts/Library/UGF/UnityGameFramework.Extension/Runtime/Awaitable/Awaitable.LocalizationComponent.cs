using System;
using Cysharp.Threading.Tasks;
using GameFramework;
using GameFramework.Event;
using UnityGameFramework.Runtime;

namespace UnityGameFramework.Extension
{
    public static partial class Awaitable
    {
        public static UniTask ReadDataAsync(this LocalizationComponent localizationComponent, string dictionaryAssetName,
            int priority = 0, Action<float> updateEvent = null, Action<string> dependencyAsset = null)
        {
#if UNITY_EDITOR
            TipsSubscribeEvent();
#endif
            ReadDataEventData eventData = ReferencePool.Acquire<ReadDataEventData>();
            eventData.UpdateEvent = updateEvent;
            eventData.IsError = false;
            eventData.ErrorMessage = null;
            eventData.DependencyAssetEvent = dependencyAsset;
            eventData.IsFinished = false;
            
            localizationComponent.ReadData(dictionaryAssetName, priority, eventData);
            
            bool MoveNext(ref UniTaskCompletionSourceCore<object> core)
            {
                if (!IsValid)
                {
                    core.TrySetException(new GameFrameworkException("Awaitable is not valid."));
                    return false;
                }
                if (!eventData.IsFinished)
                {
                    return true;
                }
                if (eventData.IsError)
                {
                    core.TrySetException(new GameFrameworkException(eventData.ErrorMessage));
                }
                else
                {
                    core.TrySetResult(null);
                }
                return false;
            }
            
            void ReturnAction()
            {
                ReferencePool.Release(eventData);
            }
            return NewUniTask<object>(MoveNext, returnAction: ReturnAction);
        }
        
        private sealed class ReadDataEventData : IReference
        {
            public Action<float> UpdateEvent;
            public bool IsError;
            public string ErrorMessage;
            public Action<string> DependencyAssetEvent;
            public bool IsFinished;

            public void Clear()
            {
                UpdateEvent = null;
                IsError = false;
                ErrorMessage = null;
                DependencyAssetEvent = null;
                IsFinished = false;
            }
        }
        
        private static void OnLoadDictionarySuccess(object sender, GameEventArgs e)
        {
            LoadDictionarySuccessEventArgs ne = (LoadDictionarySuccessEventArgs)e;
            if (ne.UserData is ReadDataEventData eventData)
            {
                eventData.IsFinished = true;
                eventData.IsError = false;
            }
        }

        private static void OnLoadDictionaryFailure(object sender, GameEventArgs e)
        {
            LoadDictionaryFailureEventArgs ne = (LoadDictionaryFailureEventArgs)e;
            if (ne.UserData is ReadDataEventData eventData)
            {
                eventData.IsFinished = true;
                eventData.IsError = true;
                eventData.ErrorMessage = ne.ErrorMessage;
            }
        }

        private static void OnLoadDictionaryUpdate(object sender, GameEventArgs e)
        {
            LoadDictionaryUpdateEventArgs ne = (LoadDictionaryUpdateEventArgs)e;
            if (ne.UserData is ReadDataEventData eventData)
            {
                eventData.UpdateEvent?.Invoke(ne.Progress);
            }
        }
        
        private static void OnLoadDictionaryDependencyAsset(object sender, GameEventArgs e)
        {
            LoadDictionaryDependencyAssetEventArgs ne = (LoadDictionaryDependencyAssetEventArgs)e;
            if (ne.UserData is ReadDataEventData eventData)
            {
                eventData.DependencyAssetEvent?.Invoke(ne.DependencyAssetName);
            }
        }
    }
}
