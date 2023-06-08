using Cysharp.Threading.Tasks;

namespace ET.Client
{
	[Event(SceneType.Demo)]
	public class LoginFinish_CreateLobbyUI: AEvent<Scene, EventType.LoginFinish>
	{
		protected override async UniTask Run(Scene scene, EventType.LoginFinish args)
		{
			await scene.GetComponent<UIComponent>().OpenUIFormAsync(UGFUIFormId.UILobby);
		}
	}
}
