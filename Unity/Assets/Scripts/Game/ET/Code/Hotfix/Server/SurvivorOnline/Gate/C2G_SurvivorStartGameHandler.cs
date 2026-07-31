using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [MessageSessionHandler(SceneType.Gate)]
    public sealed class C2G_SurvivorStartGameHandler:
            MessageSessionHandler<C2G_SurvivorStartGame, G2C_SurvivorStartGame>
    {
        protected override async UniTask Run(
            Session session,
            C2G_SurvivorStartGame request,
            G2C_SurvivorStartGame response)
        {
            await (session.GetComponent<SurvivorSessionOperationComponent>() ??
                    session.AddComponent<SurvivorSessionOperationComponent>())
                    .Start(request, response);
        }
    }
}
