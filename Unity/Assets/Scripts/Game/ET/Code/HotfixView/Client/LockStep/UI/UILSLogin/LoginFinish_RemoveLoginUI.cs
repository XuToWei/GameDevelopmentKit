using Cysharp.Threading.Tasks;

namespace ET.Client
{
	[Event(SceneType.LockStep)]
	public class LoginFinish_RemoveUILSLogin: AEvent<Scene, EventType.LoginFinish>
	{
		protected override async UniTask Run(Scene scene, EventType.LoginFinish args)
		{
			await UniTask.CompletedTask;
			scene.GetComponent<UIComponent>().CloseUIForm(UGFUIFormId.UILSLogin);
		}
	}
}
