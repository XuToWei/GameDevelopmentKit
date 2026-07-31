using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [MessageSessionHandler(SceneType.Gate)]
    public sealed class C2G_SurvivorInputHandler: MessageSessionHandler<C2G_SurvivorInput>
    {
        protected override async UniTask Run(Session session, C2G_SurvivorInput message)
        {
            (session.GetComponent<SurvivorSessionOperationComponent>() ??
                    session.AddComponent<SurvivorSessionOperationComponent>())
                    .ForwardInput(message);
            await UniTask.CompletedTask;
        }
    }
}
