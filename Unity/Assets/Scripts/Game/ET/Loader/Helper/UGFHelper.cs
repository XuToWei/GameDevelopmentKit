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
            await GameEntry.Scene.LoadSceneAsync(sceneAssetName, Constant.AssetPriority.SceneAsset);
        }

        public static async UniTask UnloadSceneAsync(string sceneAssetName)
        {
            await GameEntry.Scene.UnloadSceneAsync(sceneAssetName);
        }

        public static async UniTask<Transform> ShowEntityAsync(int entityTypeId, CancellationToken token = default)
        {
            UnityGameFramework.Runtime.Entity ugfEntity = await GameEntry.Entity.ShowEntityAsync<ETMonoEntity>(entityTypeId, cancellationToken: token);
            return ugfEntity.Logic.CachedTransform;
        }

        public static async UniTask<Transform> ShowEntityAsync<T>(string entityAssetName, string entityGroupName, CancellationToken token = default, int priority = 0)
        {
            UnityGameFramework.Runtime.Entity ugfEntity = await GameEntry.Entity.ShowEntityAsync(GameEntry.Entity.GenerateSerialId(), typeof(ETMonoEntity), entityAssetName, entityGroupName, priority, token);
            return ugfEntity.Logic.CachedTransform;
        }
    }
}
