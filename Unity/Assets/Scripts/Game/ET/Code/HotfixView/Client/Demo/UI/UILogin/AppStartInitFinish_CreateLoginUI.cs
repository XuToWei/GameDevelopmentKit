using Cysharp.Threading.Tasks;

namespace ET.Client
{
	[Event(SceneType.Demo)]
	public class AppStartInitFinish_CreateLoginUI: AEvent<Scene, AppStartInitFinish>
	{
		protected override async UniTask Run(Scene scene, AppStartInitFinish args)
		{
			await scene.GetComponent<UGFUIComponent>().OpenUIFormAsync(UGFUIFormId.UILogin);
		}
	}
}
