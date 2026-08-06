using Cysharp.Threading.Tasks;

namespace ET.Client
{
    [Event(SceneType.Survivor)]
    public sealed class AppStartInitFinish_OpenSurvivorLogin: AEvent<Scene, AppStartInitFinish>
    {
        protected override async UniTask Run(Scene scene, AppStartInitFinish args)
        {
            await scene.GetComponent<UIComponent>()
                    .AddUIFormComponentAsync<UIFormSurvivorLoginComponent>(UGFUIFormId.SurvivorLogin);
        }
    }

    [Event(SceneType.Survivor)]
    public sealed class LoginFinish_OpenSurvivorLobby: AEvent<Scene, LoginFinish>
    {
        protected override async UniTask Run(Scene scene, LoginFinish args)
        {
            scene.GetComponent<UIComponent>().RemoveComponent<UIFormSurvivorLoginComponent>();
            scene.AddComponent<SurvivorClientComponent>();
            await SurvivorViewStarter.OpenLobby(scene);
        }
    }
}
