using System;
using Cysharp.Threading.Tasks;
using Game;
using UnityGameFramework.Extension;

namespace ET
{
    public static class UGFHelper
    {
        #region Resource
        public static async UniTask<T> LoadAssetAsync<T>(string assetName) where T : UnityEngine.Object
        {
            T asset = await GameEntry.Resource.LoadAssetAsync<T>(assetName);
            return asset;
        }

        public static void UnloadAsset(object asset)
        {
            GameEntry.Resource.UnloadAsset(asset);
        }
        #endregion

        #region Scene
        public static async UniTask LoadSceneAsync(string sceneAssetName)
        {
            await GameEntry.Scene.LoadSceneAsync(sceneAssetName);
        }

        public static async UniTask UnLoadSceneAsync(string sceneAssetName)
        {
            await GameEntry.Scene.UnLoadSceneAsync(sceneAssetName);
        }
        #endregion

        #region Entity
        public static async UniTask<UnityGameFramework.Runtime.Entity> ShowEntityAsync(int entityId, Type entityLogicType, string entityAssetName, string entityGroupName, int priority, object userData)
        {
            return await GameEntry.Entity.ShowEntityAsync(entityId, entityLogicType, entityAssetName, entityGroupName, priority, userData);
        }
        #endregion
    }
}
