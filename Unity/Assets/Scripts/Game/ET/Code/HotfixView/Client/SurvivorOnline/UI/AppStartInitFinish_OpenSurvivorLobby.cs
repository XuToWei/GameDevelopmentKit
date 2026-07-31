using Cysharp.Threading.Tasks;

namespace ET.Client
{
    [Event(SceneType.Survivor)]
    public class AppStartInitFinish_OpenSurvivorLobby: AEvent<Scene, AppStartInitFinish>
    {
        protected override async UniTask Run(Scene scene, AppStartInitFinish args)
        {
            await SurvivorViewStarter.OpenLobby(scene);
        }
    }
}
