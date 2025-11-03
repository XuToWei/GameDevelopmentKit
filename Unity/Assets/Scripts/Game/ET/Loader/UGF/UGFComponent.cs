using System.Threading;
using Cysharp.Threading.Tasks;
using Game;
using UnityEngine;
using UnityGameFramework.Extension;

namespace ET
{
    public class UGFComponent : Singleton<UGFComponent>, ISingletonAwake
    {
        public void Awake()
        {
            
        }
        
        public async UniTask<T> LoadAssetAsync<T>(string assetName) where T : UnityEngine.Object
        {
            T asset = await GameEntry.Resource.LoadAssetAsync<T>(assetName);
            return asset;
        }
        
        public void UnloadAsset(UnityEngine.Object asset)
        {
            GameEntry.Resource.UnloadAsset(asset);
        }
        
        public async UniTask LoadSceneAsync(string sceneAssetName)
        {
            await GameEntry.Scene.LoadSceneAsync(sceneAssetName, Constant.AssetPriority.SceneAsset);
        }

        public async UniTask UnloadSceneAsync(string sceneAssetName)
        {
            await GameEntry.Scene.UnloadSceneAsync(sceneAssetName);
        }
        
        public async UniTask UnloadAllScenesAsync()
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
        
        public async UniTask<Transform> ShowEntityAsync(int entityTypeId, CancellationToken cancellationToken = default)
        {
            UnityGameFramework.Runtime.Entity ugfEntity = await GameEntry.Entity.ShowEntityAsync<ETMonoUGFEntity>(entityTypeId, cancellationToken: cancellationToken);
            return ugfEntity.Logic.CachedTransform;
        }
        
        public async UniTask<Transform> ShowEntityAsync(string entityAssetName, string entityGroupName, CancellationToken cancellationToken = default, int priority = 0)
        {
            UnityGameFramework.Runtime.Entity ugfEntity = await GameEntry.Entity.ShowEntityAsync(GameEntry.Entity.GenerateSerialId(), typeof(ETMonoUGFEntity), entityAssetName, entityGroupName, priority, cancellationToken: cancellationToken);
            return ugfEntity.Logic.CachedTransform;
        }
        
        public async UniTask<T> OpenUIFormAsync<T>(UGFUIForm ugfuiForm, int uiFormTypeId, CancellationToken cancellationToken = default) where T : AETMonoUGFUIForm
        {
            UnityGameFramework.Runtime.UIForm ugfUIForm = await GameEntry.UI.OpenUIFormAsync(uiFormTypeId, ETMonoUGFUIFormData.Create(ugfuiForm), cancellationToken);
            return (T)ugfUIForm.Logic;
        }
        
        public async UniTask<T> OpenUIFormAsync<T>(UGFUIForm ugfuiForm, string uiFormAssetName, string uiGroupName, int priority, bool pauseCoveredUIForm, CancellationToken cancellationToken = default) where T : AETMonoUGFUIForm
        {
            UnityGameFramework.Runtime.UIForm ugfUIForm = await GameEntry.UI.OpenUIFormAsync(uiFormAssetName, uiGroupName, priority, pauseCoveredUIForm,  ETMonoUGFUIFormData.Create(ugfuiForm), cancellationToken);
            return (T)ugfUIForm.Logic;
        }
    }
}
