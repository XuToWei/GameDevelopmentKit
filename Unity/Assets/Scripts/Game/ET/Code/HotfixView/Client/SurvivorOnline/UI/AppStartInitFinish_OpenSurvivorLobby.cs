using Cysharp.Threading.Tasks;

namespace ET.Client
{
    [Event(SceneType.Survivor)]
    public sealed class AppStartInitFinish_OpenSurvivorLogin: AEvent<Scene, AppStartInitFinish>
    {
        protected override async UniTask Run(Scene scene, AppStartInitFinish args)
        {
            await scene.GetComponent<UIComponent>().AddUIFormComponentAsync<UIFormSurvivorLoginComponent>(UGFUIFormId.SurvivorLogin);
        }
    }

    /// <summary>
    /// 登录完成后只建立 SurvivorClientComponent。Lobby 界面由 SurvivorViewComponent
    /// 首次观察 Phase 时打开，避免出现第二条 UI 打开路径。
    /// </summary>
    [Event(SceneType.Survivor)]
    public sealed class LoginFinish_StartSurvivorClient: AEvent<Scene, LoginFinish>
    {
        protected override UniTask Run(Scene scene, LoginFinish args)
        {
            scene.GetComponent<UIComponent>().RemoveComponent<UIFormSurvivorLoginComponent>();
            scene.AddComponent<SurvivorClientComponent>();
            return UniTask.CompletedTask;
        }
    }
}
