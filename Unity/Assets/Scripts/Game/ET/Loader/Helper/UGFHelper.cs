using System.Threading;
using Cysharp.Threading.Tasks;
using Game;
using UnityEngine;
using UnityGameFramework.Extension;
using GameEntry = Game.GameEntry;

namespace ET
{
    public static class UGFHelper
    {
        public static async UniTask<T> LoadAssetAsync<T>(string assetName) where T : UnityEngine.Object
        {
            T asset = await GameEntry.Resource.LoadAssetAsync<T>(assetName);
            return asset;
        }

        public static void UnloadAsset(UnityEngine.Object asset)
        {
            GameEntry.Resource.UnloadAsset(asset);
        }

        public static async UniTask LoadSceneAsync(string sceneAssetName)
        {
            await GameEntry.Scene.LoadSceneAsync(AssetUtility.GetSceneAsset(sceneAssetName), Constant.AssetPriority.SceneAsset);
        }

        public static async UniTask UnloadSceneAsync(string sceneAssetName)
        {
            await GameEntry.Scene.UnloadSceneAsync(sceneAssetName);
        }

        public static async UniTask UnloadAllScenesAsync()
        {
            ListComponent<string> loadingSceneAssetNames = ListComponent<string>.Create();
            ListComponent<UniTask> unloadTasks = ListComponent<UniTask>.Create();
            GameEntry.Scene.GetLoadingSceneAssetNames(loadingSceneAssetNames);
            foreach (var loadingSceneAssetName in loadingSceneAssetNames)
            {
                unloadTasks.Add(GameEntry.Scene.UnloadSceneAsync(loadingSceneAssetName));
            }
            loadingSceneAssetNames.Clear();
            GameEntry.Scene.GetLoadedSceneAssetNames(loadingSceneAssetNames);
            foreach (var loadingSceneAssetName in loadingSceneAssetNames)
            {
                unloadTasks.Add(GameEntry.Scene.UnloadSceneAsync(loadingSceneAssetName));
            }
            loadingSceneAssetNames.Dispose();
            await UniTask.WhenAll(unloadTasks);
            unloadTasks.Dispose();
        }

        public static async UniTask<Transform> ShowEntityAsync(int entityTypeId, CancellationToken token = default)
        {
            UnityGameFramework.Runtime.Entity ugfEntity = await GameEntry.Entity.ShowEntityAsync<ETMonoUGFEntity>(entityTypeId, cancellationToken: token);
            return ugfEntity.Logic.CachedTransform;
        }

        public static async UniTask<Transform> ShowEntityAsync(string entityAssetName, string entityGroupName, CancellationToken token = default, int priority = 0)
        {
            UnityGameFramework.Runtime.Entity ugfEntity = await GameEntry.Entity.ShowEntityAsync(GameEntry.Entity.GenerateSerialId(), typeof(ETMonoUGFEntity), entityAssetName, entityGroupName, priority, cancellationToken: token);
            return ugfEntity.Logic.CachedTransform;
        }
    }
}
