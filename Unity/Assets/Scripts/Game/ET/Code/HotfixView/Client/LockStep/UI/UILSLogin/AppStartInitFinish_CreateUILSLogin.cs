using Cysharp.Threading.Tasks;

namespace ET.Client
{
	[Event(SceneType.LockStep)]
	public class AppStartInitFinish_CreateUILSLogin: AEvent<Scene, AppStartInitFinish>
	{
		protected override async UniTask Run(Scene scene, AppStartInitFinish args)
		{
			await scene.GetComponent<UGFUIComponent>().OpenUIFormAsync(UGFUIFormId.UILSLogin);
		}
	}
}
