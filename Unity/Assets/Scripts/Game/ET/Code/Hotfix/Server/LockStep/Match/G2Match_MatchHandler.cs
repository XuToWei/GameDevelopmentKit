using Cysharp.Threading.Tasks;

namespace ET.Server
{
	[MessageHandler(SceneType.Match)]
	public class G2Match_MatchHandler : MessageHandler<Scene, G2Match_Match, Match2G_Match>
	{
		protected override async UniTask Run(Scene scene, G2Match_Match request, Match2G_Match response)
		{
			MatchComponent matchComponent = scene.GetComponent<MatchComponent>();
			matchComponent.Match(request.Id).Forget();
			await UniTask.CompletedTask;
		}
	}
}