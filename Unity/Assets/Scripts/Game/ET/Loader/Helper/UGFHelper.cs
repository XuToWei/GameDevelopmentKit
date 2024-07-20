using Cysharp.Threading.Tasks;
using Game;
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
    }
}
