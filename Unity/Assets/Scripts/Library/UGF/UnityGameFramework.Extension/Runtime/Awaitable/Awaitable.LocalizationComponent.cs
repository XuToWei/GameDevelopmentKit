using System.Threading;
using Cysharp.Threading.Tasks;
using GameFramework;
using GameFramework.Event;
using UnityGameFramework.Runtime;

namespace UnityGameFramework.Extension
{
    public static partial class Awaitable
    {
        public static UniTask ReadDataAsync(this LocalizationComponent localizationComponent, string dataAssetName, int priority, object userData = null, CancellationToken cancellationToken = default)
        {
#if UNITY_EDITOR
            TipsSubscribeEvent();
#endif
            AwaitDataWrap awaitDataWrap = AwaitDataWrap.Create(userData);
            localizationComponent.ReadData(dataAssetName, priority, awaitDataWrap);
            bool IsFinished()
            {
                return awaitDataWrap.IsFinished;
            }
            return UniTask.WaitUntil(IsFinished, PlayerLoopTiming.Update, cancellationToken);
        }
        
        private static void OnLoadDictionarySuccess(object sender, GameEventArgs e)
        {
            LoadDictionarySuccessEventArgs ne = (LoadDictionarySuccessEventArgs)e;
            if (ne.UserData is AwaitDataWrap awaitDataWrap)
            {
                ReferencePool.Release(awaitDataWrap);
            }
            Log.Info("Load dictionary '{0}' OK.", ne.DictionaryAssetName);
        }

        private static void OnLoadDictionaryFailure(object sender, GameEventArgs e)
        {
            LoadDictionaryFailureEventArgs ne = (LoadDictionaryFailureEventArgs)e;
            if (ne.UserData is AwaitDataWrap awaitDataWrap)
            {
                ReferencePool.Release(awaitDataWrap);
            }
            Log.Error("Can not load dictionary '{0}' from '{1}' with error message '{2}'.", ne.DictionaryAssetName, ne.DictionaryAssetName, ne.ErrorMessage);
        }
    }
}
