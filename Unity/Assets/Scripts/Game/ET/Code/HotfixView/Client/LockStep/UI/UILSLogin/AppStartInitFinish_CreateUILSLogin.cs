using Cysharp.Threading.Tasks;

namespace ET.Client
{
	[Event(SceneType.LockStep)]
	public class AppStartInitFinish_CreateUILSLogin: AEvent<Scene, EventType.AppStartInitFinish>
	{
		protected override async UniTask Run(Scene scene, EventType.AppStartInitFinish args)
		{
			await scene.GetComponent<UIComponent>().OpenUIFormAsync(UGFUIFormId.UILSLogin);
		}
	}
}
