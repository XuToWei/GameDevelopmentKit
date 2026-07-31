using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [MessageSessionHandler(SceneType.Gate)]
    public sealed class C2G_SurvivorJoinRoomHandler:
            MessageSessionHandler<C2G_SurvivorJoinRoom, G2C_SurvivorJoinRoom>
    {
        protected override async UniTask Run(
            Session session,
            C2G_SurvivorJoinRoom request,
            G2C_SurvivorJoinRoom response)
        {
            await (session.GetComponent<SurvivorSessionOperationComponent>() ??
                    session.AddComponent<SurvivorSessionOperationComponent>())
                    .Join(request, response);
        }
    }
}
