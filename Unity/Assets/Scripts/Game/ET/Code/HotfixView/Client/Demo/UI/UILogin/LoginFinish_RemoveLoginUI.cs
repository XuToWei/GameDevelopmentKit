using Cysharp.Threading.Tasks;

namespace ET.Client
{
	[Event(SceneType.Demo)]
	public class LoginFinish_RemoveLoginUI: AEvent<Scene, EventType.LoginFinish>
	{
		protected override async UniTask Run(Scene scene, EventType.LoginFinish args)
		{
			await UniTask.CompletedTask;
			scene.GetComponent<UIComponent>().CloseUIForm(UGFUIFormId.UILogin);
		}
	}
}
