using System;
using Cysharp.Threading.Tasks;
using Game;
using UnityGameFramework.Extension;

namespace ET.Client
{
    [Event(SceneType.Demo)]
    public class SceneChangeStart_AddComponent: AEvent<Scene, SceneChangeStart>
    {
        protected override async UniTask Run(Scene root, SceneChangeStart args)
        {
            try
            {
                Scene currentScene = root.CurrentScene();

                // 切换到map场景
                foreach (var sceneAssetName in GameEntry.Scene.GetLoadingSceneAssetNames())
                {
                    await GameEntry.Scene.UnloadSceneAsync(sceneAssetName);
                }
                foreach (var sceneAssetName in GameEntry.Scene.GetLoadedSceneAssetNames())
                {
                    await GameEntry.Scene.UnloadSceneAsync(sceneAssetName);
                }
                await GameEntry.Scene.LoadSceneAsync(AssetUtility.GetSceneAsset(currentScene.Name));
                
                currentScene.AddComponent<OperaComponent>();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

        }
    }
}