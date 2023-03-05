using Cysharp.Threading.Tasks;

namespace ET.Client
{
    [Event(SceneType.Client)]
    public class LoginFinish_CreateLobbyUI : AEvent<EventType.LoginFinish>
    {
        protected override async UniTask Run(Scene scene, EventType.LoginFinish args)
        {
            await UIComponent.Instance.OpenUIFormAsync(UGFUIFormId.UILobby);
        }
    }
}