using Cysharp.Threading.Tasks;

namespace ET.Server
{
	[MessageLocationHandler(SceneType.Map)]
	public class G2M_SessionDisconnectHandler : MessageLocationHandler<Unit, G2M_SessionDisconnect>
	{
		protected override async UniTask Run(Unit unit, G2M_SessionDisconnect message)
		{
			await UniTask.CompletedTask;
		}
	}
}