using Cysharp.Threading.Tasks;

namespace ET.Client
{
	[MessageHandler(SceneType.Demo)]
	public class M2C_StartSceneChangeHandler : MessageHandler<M2C_StartSceneChange>
	{
		protected override async UniTask Run(Session session, M2C_StartSceneChange message)
		{
			await SceneChangeHelper.SceneChangeTo(session.ClientScene(), message.SceneName, message.SceneInstanceId);
		}
	}
}
