using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
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

        public static void UnloadAsset(object asset)
        {
            GameEntry.Resource.UnloadAsset(asset);
        }
        #endregion

        #region Scene
        public static async UniTaskVoid LoadSceneAsync(string sceneAssetName)
        {
            await GameEntry.Scene.LoadSceneAsync(sceneAssetName);
        }

        public static async UniTaskVoid UnLoadSceneAsync(string sceneAssetName)
        {
            await GameEntry.Scene.UnLoadSceneAsync(sceneAssetName);
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
