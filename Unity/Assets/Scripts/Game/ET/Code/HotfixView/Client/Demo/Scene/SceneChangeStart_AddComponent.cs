using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace ET.Client
{
    [Event(SceneType.Client)]
    public class SceneChangeStart_AddComponent : AEvent<EventType.SceneChangeStart>
    {
        protected override async UniTask Run(Scene scene, EventType.SceneChangeStart args)
        {
            Scene currentScene = scene.CurrentScene();

            // 加载场景资源

            // 切换到map场景

            await SceneManager.LoadSceneAsync(currentScene.Name);
            
            currentScene.AddComponent<OperaComponent>();
        }
    }
}