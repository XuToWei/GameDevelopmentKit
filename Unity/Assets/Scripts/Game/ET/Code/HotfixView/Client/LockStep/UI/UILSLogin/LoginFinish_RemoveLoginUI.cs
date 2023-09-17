using Cysharp.Threading.Tasks;

namespace ET.Client
{
	[Event(SceneType.LockStep)]
	public class LoginFinish_RemoveUILSLogin: AEvent<Scene, LoginFinish>
	{
		protected override async UniTask Run(Scene scene, LoginFinish args)
		{
			await UniTask.CompletedTask;
			scene.GetComponent<UGFUIComponent>().CloseUIForm(UGFUIFormId.UILSLogin);
		}
	}
}
