using System;
using Cysharp.Threading.Tasks;
using Game;

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
                UGFComponent ugfComponent = root.GetComponent<UGFComponent>();

                // 切换到map场景
                await ugfComponent.UnloadAllScenesAsync();
                await ugfComponent.LoadSceneAsync(AssetUtility.GetSceneAsset(currentScene.Name));
                
                currentScene.AddComponent<OperaComponent>();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

        }
    }
}
