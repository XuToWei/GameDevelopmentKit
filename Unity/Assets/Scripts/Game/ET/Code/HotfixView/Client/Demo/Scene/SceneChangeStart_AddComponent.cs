using Cysharp.Threading.Tasks;
using Game;
using UnityGameFramework.Extension;

namespace ET.Client
{
    [Event(SceneType.Client)]
    public class SceneChangeStart_AddComponent : AEvent<EventType.SceneChangeStart>
    {
        protected override async UniTask Run(Scene scene, EventType.SceneChangeStart args)
        {
            Scene currentScene = scene.CurrentScene();
            
            // 切换到map场景\
            foreach (var sceneAssetName in GameEntry.Scene.GetLoadingSceneAssetNames())
            {
                await GameEntry.Scene.UnLoadSceneAsync(sceneAssetName);
            }
            foreach (var sceneAssetName in GameEntry.Scene.GetLoadedSceneAssetNames())
            {
                await GameEntry.Scene.UnLoadSceneAsync(sceneAssetName);
            }
            
            await GameEntry.Scene.LoadSceneAsync(AssetUtility.GetSceneAsset(currentScene.Name));
            currentScene.AddComponent<OperaComponent>();
        }
    }
}