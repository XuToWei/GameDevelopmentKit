using Cysharp.Threading.Tasks;

namespace ET.Client
{
	[Event(SceneType.Demo)]
	public class AppStartInitFinish_CreateLoginUI: AEvent<Scene, EventType.AppStartInitFinish>
	{
		protected override async UniTask Run(Scene scene, EventType.AppStartInitFinish args)
		{
			await scene.GetComponent<UIComponent>().OpenUIFormAsync(UGFUIFormId.UILogin);
		}
	}
}
