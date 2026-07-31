using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [MessageSessionHandler(SceneType.Gate)]
    public sealed class C2G_SurvivorRequestFullSnapshotHandler:
            MessageSessionHandler<C2G_SurvivorRequestFullSnapshot>
    {
        protected override async UniTask Run(
            Session session,
            C2G_SurvivorRequestFullSnapshot message)
        {
            (session.GetComponent<SurvivorSessionOperationComponent>() ??
                    session.AddComponent<SurvivorSessionOperationComponent>())
                    .RequestFullSnapshot();
            await UniTask.CompletedTask;
        }
    }
}
