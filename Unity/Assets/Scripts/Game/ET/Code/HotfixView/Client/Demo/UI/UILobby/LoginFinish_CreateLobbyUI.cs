using Cysharp.Threading.Tasks;

namespace ET.Client
{
	[Event(SceneType.Demo)]
	public class LoginFinish_CreateLobbyUI: AEvent<Scene, LoginFinish>
	{
		protected override async UniTask Run(Scene scene, LoginFinish args)
		{
			await scene.GetComponent<UGFUIComponent>().OpenUIFormAsync(UGFUIFormId.UILobby);
		}
	}
}
