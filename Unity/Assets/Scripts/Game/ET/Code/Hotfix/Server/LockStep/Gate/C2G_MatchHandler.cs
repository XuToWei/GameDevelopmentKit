using Cysharp.Threading.Tasks;

namespace ET.Server
{
	[MessageSessionHandler(SceneType.Gate)]
	public class C2G_MatchHandler : MessageSessionHandler<C2G_Match, G2C_Match>
	{
		protected override async UniTask Run(Session session, C2G_Match request, G2C_Match response)
		{
			Player player = session.GetComponent<SessionPlayerComponent>().Player;

			var startSceneConfig = Tables.Instance.DTStartSceneConfig.Match;

			await session.Root().GetComponent<MessageSender>().Call(startSceneConfig.ActorId,
				new G2Match_Match() { Id = player.Id });
		}
	}
}