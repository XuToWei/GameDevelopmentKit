using System;
using System.Threading.Tasks;
using UnityGameFramework.Extension;
using GameEntry = Game.GameEntry;
using Entity = UnityGameFramework.Runtime.Entity;

namespace ET
{
    public static class UGFHelper
    {
        #region Resource
        public static async ETTask<T> LoadAssetAsync<T>(string assetName) where T : UnityEngine.Object
        {
            T asset = await GameEntry.Resource.LoadAssetAsync<T>(assetName);
            return asset;
        }

        public static async ETTask<T[]> LoadAssetsAsync<T>(string[] assetNames) where T : UnityEngine.Object
        {
            T[] assets = await GameEntry.Resource.LoadAssetsAsync<T>(assetNames);
            return assets;
        }

        public static void UnloadAsset(object asset)
        {
            GameEntry.Resource.UnloadAsset(asset);
        }
        #endregion

        #region Scene
        public static async ETTask<bool> LoadSceneAsync(string sceneAssetName)
        {
            return await GameEntry.Scene.LoadSceneAsync(sceneAssetName);
        }

        public static async Task<bool> UnLoadSceneAsync(string sceneAssetName)
        {
            return await GameEntry.Scene.UnLoadSceneAsync(sceneAssetName);
        }
        #endregion

        #region Entity
        public static async ETTask<UnityGameFramework.Runtime.Entity> ShowEntityAsync(int entityId, Type entityLogicType, string entityAssetName, string entityGroupName, int priority, object userData)
        {
            return await GameEntry.Entity.ShowEntityAsync(entityId, entityLogicType, entityAssetName, entityGroupName, priority, userData);
        }
        #endregion
    }
}
