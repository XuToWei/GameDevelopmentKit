using Cysharp.Threading.Tasks;

namespace ET.Client
{
	[MessageHandler(SceneType.Demo)]
	public class M2C_CreateMyUnitHandler: MessageHandler<Scene, M2C_CreateMyUnit>
	{
		protected override async UniTask Run(Scene root, M2C_CreateMyUnit message)
		{
			// 通知场景切换协程继续往下走
			root.GetComponent<ObjectWait>().Notify(new Wait_CreateMyUnit() {Message = message});
			await UniTask.CompletedTask;
		}
	}
}
